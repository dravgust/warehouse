using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Persistence
{
    public static class Config
    {
        public static void ConfigureMongoDb()
        {
            AutoRegistration.RegisterClassMap(Assembly.GetExecutingAssembly());
            //db.setProfilingLevel(2,1)
        }

        public class FileEntityClassMap : MongoClassMap<FileEntity>
        {
            public override void Map(BsonClassMap<FileEntity> cm)
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance);
            }
        }

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
}
