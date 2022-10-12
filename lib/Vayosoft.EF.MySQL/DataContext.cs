using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Exceptions;
using Vayosoft.Core.Specifications;

namespace Vayosoft.EF.MySQL
{
    public class DataContext : DbContext, ILinqProvider, IUnitOfWork
    {
        private readonly ILoggerFactory _loggerFactory;

        public DataContext(DbContextOptions options, ILoggerFactory loggerFactory) : base(options)
        {
            this._loggerFactory = loggerFactory;
        }

        public IQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class, IEntity {
            return Set<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> AsQueryable<TEntity>(ISpecification<TEntity> specification) where TEntity : class, IEntity {
            return new SpecificationEvaluator<TEntity>().Evaluate(AsQueryable<TEntity>(), specification);
        }

        public new void Add<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
           base.Entry(entity).State = EntityState.Added;
        }

        public new void Update<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            base.Entry(entity).State = EntityState.Modified;
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            base.Entry(entity).State = EntityState.Deleted;
        }

        public TEntity Find<TEntity>(object id) where TEntity : class, IEntity
        {
            return Set<TEntity>().SingleOrDefault(x => x.Id == id);
        }

        public TEntity Get<TEntity>(object id) where TEntity : class, IEntity
        {
            var entity = Find<TEntity>(id);
            if (entity == null)
                throw EntityNotFoundException.For<TEntity>(id);

            return entity;
        }

        public Task<TEntity> FindAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class, IEntity
        {
            return Set<TEntity>().SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
        }

        public async Task<TEntity> GetAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class, IEntity
        {
            var entity = await FindAsync<TEntity>(id, cancellationToken);
            if (entity == null)
                throw EntityNotFoundException.For<TEntity>(id);

            return entity;
        }

        public void Commit() => SaveChanges();
        public Task CommitAsync() => SaveChangesAsync();

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
