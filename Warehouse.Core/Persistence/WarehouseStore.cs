using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Persistence
{
    public class WarehouseStore : MongoContextBase, IDisposable
    {
        private readonly IMapper _mapper;
        private readonly ILogger<WarehouseStore> _logger;

        public WarehouseStore(IConfiguration config, IMapper mapper, ILogger<WarehouseStore> logger)
            : base(config)
        {
            _mapper = mapper;
            _logger = logger;
#if DEBUG
            this.TraceLine += OnTrace;
#endif 
        }

        public Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            FirstOrDefaultAsync<T, TResult>(criteria, _mapper, cancellationToken);

        public Task<TResult> SingleOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            SingleOrDefaultAsync<T, TResult>(criteria, _mapper, cancellationToken);

        public Task <List<TResult>> ListAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            ListAsync<T, TResult>(criteria, _mapper, cancellationToken);

        private void OnTrace(string name, string command)
        {
            _logger?.LogDebug($"{name}\r\n{command}");
        }

        public void Dispose()
        {
#if DEBUG
            this.TraceLine -= OnTrace;
#endif
        }
    }
}
