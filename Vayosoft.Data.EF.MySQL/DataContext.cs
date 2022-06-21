using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Exceptions;

namespace Vayosoft.Data.EF.MySQL
{
    public sealed class DataContext : DbContext, ILinqProvider, IUnitOfWork
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<DataContext> _logger;

        public DataContext(DbContextOptions options, ILoggerFactory loggerFactory) : base(options)
        {
            this._loggerFactory = loggerFactory;
            this._logger = loggerFactory.CreateLogger<DataContext>();

            //this.ChangeTracker.LazyLoadingEnabled = false;

            Database.EnsureCreated();
        }

        public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IEntity
        {
            return Set<TEntity>();
        }

        public new void Add<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
           base.Entry(entity).State = entity.Id != null 
               ? EntityState.Modified 
               : EntityState.Added;
        }
        
        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            Set<TEntity>().Remove(entity);
        }

        public TEntity Find<TEntity>(object id) where TEntity : class, IEntity
        {
            var entity = Set<TEntity>().SingleOrDefault(x => x.Id == id);
            if (entity == null)
                throw EntityNotFoundException.For<TEntity>(id);

            return entity;
        }

        public void Commit() => SaveChanges();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(this._loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var typesToRegister = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(type => !string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType is { IsGenericType: true } && type.BaseType.GetGenericTypeDefinition() == typeof(EntityConfigurationMapper<>));

            foreach (var type in typesToRegister)
            {
                dynamic configInstance = Activator.CreateInstance(type)!;
                modelBuilder.ApplyConfiguration(configInstance);
            }
        }
    }
}
