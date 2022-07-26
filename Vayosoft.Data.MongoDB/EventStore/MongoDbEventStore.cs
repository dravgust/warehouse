using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Data.MongoDB.EventStore
{
    public class MongoDbEventStore : IEventStore
    {
        private readonly IEventBus _publisher;

        private readonly IMongoDbContext _context;

        private readonly IMongoCollection<EventData> _events;

        private const string EventsCollection = "events";

        public MongoDbEventStore(IEventBus publisher, IMongoDbContext context)
        {
            _publisher = publisher;
            _context = context;
            _events = _context.Database.GetCollection<EventData>(EventsCollection);
        }

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            var filterBuilder = Builders<EventData>.Filter;
            var filter = filterBuilder.Eq(EventData.StreamIdFieldName, aggregateId) &
                        filterBuilder.Gte(EventData.VersionFieldName, fromVersion);

            var result = await _events.FindAsync(filter, cancellationToken: cancellationToken);
            return (await result.ToListAsync(cancellationToken)).Select(x => x.PayLoad);
        }

        public async Task Save(IAggregate aggregate, CancellationToken cancellationToken = default)
        {
            var events = aggregate.DequeueUncommittedEvents();
            if (!events.Any()) return;

            using var session = await _context.StartSession(cancellationToken);
            var transactionOptions = new TransactionOptions(ReadConcern.Snapshot, ReadPreference.Primary, WriteConcern.WMajority);
            session.StartTransaction(transactionOptions);
            try
            {
                var bulkOps = new List<WriteModel<EventData>>();
                foreach (var @event in events)
                {
                    var eventData = new EventData
                    {
                        Id = GuidGenerator.New(),
                        StreamId = aggregate.Id,
                        TimeStamp = DateTimeOffset.UtcNow,
                        Version = aggregate.Version,
                        AssemblyQualifiedName = @event.GetType().AssemblyQualifiedName,
                        PayLoad = @event
                    };
                    bulkOps.Add(new InsertOneModel<EventData>(eventData));
                    await _publisher.Publish(@event);
                }
                await _events.BulkWriteAsync(session, bulkOps, cancellationToken: cancellationToken).ConfigureAwait(false);

                await session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await session.AbortTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
