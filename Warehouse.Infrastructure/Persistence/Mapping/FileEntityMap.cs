using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Infrastructure.Persistence.Mapping
{
    public class FileEntityClassMap : MongoClassMap<FileEntity>
    {
        public override void Map(BsonClassMap<FileEntity> cm)
        {
            cm.AutoMap();
            cm.MapIdProperty(c => c.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance);
        }
    }
}
