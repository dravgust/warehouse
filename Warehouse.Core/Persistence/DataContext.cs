using System;
using System.Linq;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Persistence
{
    public class DataContext : IUnitOfWork, ILinqProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DataContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public TEntity Find<TEntity>(object id) where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

        public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }
    }
}
