using Vayosoft.Commons;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Events;
using Vayosoft.MongoDB;
using Warehouse.Core.Application.Common.Persistence;

namespace Warehouse.Infrastructure.Persistence
{
    public sealed class AggregateRepository<T> : MongoRepository<T> ,IAggregateRepository<T> where T : class, IAggregate<string>
    {
        private readonly IEventBus _eventBus;

        public AggregateRepository(IMongoConnection connection, IMapper mapper, IEventBus eventBus) : base(connection, mapper)
        {
            _eventBus = eventBus;
        }
    }
}
