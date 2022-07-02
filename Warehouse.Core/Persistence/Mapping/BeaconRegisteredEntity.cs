using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Persistence.Mapping
{
    public class BeaconReceivedEntityClassMap : MongoClassMap<BeaconReceivedEntity>
    {
        public override void Map(BsonClassMap<BeaconReceivedEntity> cm)
        {
            cm.AutoMap();
            cm.MapIdProperty(c => c.MacAddress)
                .SetIdGenerator(StringObjectIdGenerator.Instance);
        }
    }

    public class BeaconRegisteredEntityClassMap : MongoClassMap<BeaconRegisteredEntity>
    {
        public override void Map(BsonClassMap<BeaconRegisteredEntity> cm)
        {
            cm.AutoMap();
            cm.MapIdProperty(c => c.MacAddress)
                .SetIdGenerator(StringObjectIdGenerator.Instance);
        }
    }
}
