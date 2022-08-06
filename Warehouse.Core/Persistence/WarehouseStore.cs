using System.Linq.Expressions;
using MongoDB.Driver;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Exceptions;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Persistence
{
    public class WarehouseStore
    {
        private readonly IMongoConnection _connection;
        private readonly IMapper _mapper;

        public WarehouseStore(IMongoConnection connection, IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public IQueryable<T> AsQueryable<T>() where T : class, IEntity =>
            _connection.Collection<T>().AsQueryable();

        public Task<T> GetAsync<T>(string id, CancellationToken cancellationToken = default) where T : IEntity =>
            GetAsync<T, string>(id, cancellationToken);

        public Task<T> GetAsync<T, TId>(TId id, CancellationToken cancellationToken = default) where T : IEntity where TId : notnull =>
            FindAsync<T>(id, cancellationToken) ?? throw EntityNotFoundException.For<T>(id);

        public async Task GetAndUpdateAsync<T>(string id, Action<T> action, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            var entity = await GetAsync<T>(id, cancellationToken);
            action(entity);
            await UpdateAsync(entity, cancellationToken);
        }

        public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(criteria).FirstOrDefaultAsync(cancellationToken);

        public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(criteria).SingleOrDefaultAsync(cancellationToken);

        public Task<List<T>> ListAsync<T>(CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(Builders<T>.Filter.Empty).ToListAsync(cancellationToken);

        public Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(criteria).ToListAsync(cancellationToken);

        public Task<IPagedEnumerable<T>> PagedListAsync<T>(IPagingModel<T, object> model, Expression<Func<T, bool>> criteria, CancellationToken cancellationToken)
            where T : class, IEntity => _connection.Collection<T>().AggregateByPage(model, Builders<T>.Filter.Where(criteria), cancellationToken);

        //public async Task<IPagedEnumerable<T>> PagedListAsync2<T>(IPagingModel<T, object> model,
        //    Expression<Func<T, bool>> criteria, CancellationToken cancellationToken) where T : class, IEntity
        //{
        //    var filterDefinition = Builders<T>.Filter.Where(criteria);
        //    var sortDefinition = model.OrderBy.SortOrder == SortOrder.Asc
        //        ? Builders<T>.Sort.Ascending(model.OrderBy.Expression)
        //        : Builders<T>.Sort.Descending(model.OrderBy.Expression);

        //    var data = await Collection<T>().Find(filterDefinition)
        //        .Sort(sortDefinition)
        //        .Skip((model.Page - 1) * model.Take)
        //        .Limit(model.Take)
        //        .ToListAsync(cancellationToken: cancellationToken);

        //    var count = await Collection<T>().CountDocumentsAsync(filterDefinition, cancellationToken: cancellationToken);

        //    return new PagedEnumerable<T>(data, count);
        //}

        public Task<IPagedEnumerable<T>> PagedListAsync<T>(IPagingModel<T, object> model, CancellationToken cancellationToken) where T : class, IEntity =>
            _connection.Collection<T>().AggregateByPage(model, Builders<T>.Filter.Empty, cancellationToken);

        public Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, IMapper mapper, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(criteria).Project(x => mapper.Map<TResult>(x)).FirstOrDefaultAsync(cancellationToken);

        public Task<TResult> SingleOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, IMapper mapper, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(criteria).Project(x => mapper.Map<TResult>(x)).SingleOrDefaultAsync(cancellationToken);

        public Task<List<TResult>> ListAsync<T, TResult>(Expression<Func<T, bool>> criteria, IMapper mapper, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(criteria).Project(x => mapper.Map<TResult>(x)).ToListAsync(cancellationToken);

        public Task<T> FindAsync<T>(object id, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(q => q.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);

        public Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().InsertOneAsync(entity, cancellationToken: cancellationToken);

        public Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().ReplaceOneAsync(e => e.Id.Equals(entity.Id), entity, cancellationToken: cancellationToken);

        public Task UpdateAsync<T>(Expression<Func<T, bool>> criteria, T entity, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().ReplaceOneAsync(criteria, entity, cancellationToken: cancellationToken);

        public Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().DeleteOneAsync(e => e.Id.Equals(entity.Id), cancellationToken: cancellationToken);

        public Task DeleteAsync<T>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken) where T : IEntity =>
            _connection.Collection<T>().DeleteOneAsync(criteria, cancellationToken: cancellationToken);

        public Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            FirstOrDefaultAsync<T, TResult>(criteria, _mapper, cancellationToken);

        public Task<TResult> SingleOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            SingleOrDefaultAsync<T, TResult>(criteria, _mapper, cancellationToken);

        public Task <List<TResult>> ListAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            ListAsync<T, TResult>(criteria, _mapper, cancellationToken);
    }
}
