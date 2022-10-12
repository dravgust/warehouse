using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.MongoDB;
using Warehouse.Core.Application.Common.Persistence;

namespace Warehouse.Infrastructure.Persistence
{
    public sealed class AggregateRepository<T> : MongoRepositoryBase<T> ,IRepository<T> where T : class, IAggregate<string>
    {
        private readonly IEventBus _eventBus;

        public AggregateRepository(IMongoConnection connection, IMapper mapper, IEventBus eventBus) : base(connection, mapper)
        {
            _eventBus = eventBus;
        }
    }
}
