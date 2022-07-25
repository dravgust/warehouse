using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Positioning.Models;

namespace Warehouse.Host
{
    internal class WarehouseStore
    {
        public async Task<IEnumerable<T>> GetAsync<T>(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetAsync<T>(string id, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task SetAsync(IndoorPositionStatusEntity status, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task SetAsync<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken)
        {
        }

        public async Task AddAsync(BeaconEventEntity p0, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync<T>(Func<T, bool> func, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public void AddBeaconEntity(BeaconTelemetryDto entity, BeaconType bType)
        {
            throw new NotImplementedException();
        }

        public DolavGatewayPayload GetGwPayload(string mac)
        {
            throw new NotImplementedException();
        }
    }
}
