using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Persistence
{
    public static class Config
    {
        public static void ConfigureMongoDb()
        {
            AutoRegistration.RegisterClassMap(Assembly.GetExecutingAssembly());
            //db.setProfilingLevel(2,1)
        }

        public class AggregateClassMap : MongoClassMap<EntityBase<string>>
        {
            public override void Map(BsonClassMap<EntityBase<string>> cm)
            {
                cm.AutoMap();
                //cm.MapIdProperty(c => c.Id).SetIdGenerator(CombGuidGenerator.Instance);
                //cm.MapIdField(x => x.Id).SetIdGenerator(CombGuidGenerator.Instance);
                //cm.IdMemberMap.SetIdGenerator(CombGuidGenerator.Instance);
                cm.IdMemberMap.SetIdGenerator(StringObjectIdGenerator.Instance);
            }
        }
    }
}
