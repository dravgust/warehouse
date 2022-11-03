using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Vayosoft.Commons.Entities;
using Vayosoft.MongoDB;

namespace Warehouse.Infrastructure.Persistence.Mapping
{
    public class EntityBaseClassMap : MongoClassMap<EntityBase<string>>
    {
        public override void Map(BsonClassMap<EntityBase<string>> cm)
        {
            cm.AutoMap();
            //cm.MapIdProperty(c => c.Id).SetIdGenerator(CombGuidGenerator.Instance);
            //cm.MapIdField(x => x.Id).SetIdGenerator(CombGuidGenerator.Instance);
            //cm.IdMemberMap.SetIdGenerator(CombGuidGenerator.Instance);
            //cm.IdMemberMap.SetIdGenerator(StringObjectIdGenerator.Instance);
            cm.IdMemberMap.SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
        }


    }
}
