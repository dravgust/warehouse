using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Persistence
{
    public class WarehouseStore : DataStore
    {
        private readonly IMapper _mapper;

        public WarehouseStore(IConfiguration config, ILoggerFactory loggerFactory, IMapper mapper)
            : base(config, loggerFactory)
        {
            _mapper = mapper;
        }

        public Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            FirstOrDefaultAsync<T, TResult>(criteria, _mapper, cancellationToken);

        public Task<TResult> SingleOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            SingleOrDefaultAsync<T, TResult>(criteria, _mapper, cancellationToken);

        public Task <List<TResult>> ListAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            ListAsync<T, TResult>(criteria, _mapper, cancellationToken);
    }
}
