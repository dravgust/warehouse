using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Data.MongoDB
{
    public class IdGenerator<T> : IIdGenerator
    {
        public object GenerateId(object container, object document)
        {
            var col = (IMongoCollection<T>)container;
            var sortBy = Builders<T>.Sort.Descending("_id");
            var last = col.Find(Builders<T>.Filter.Empty).Sort(sortBy).FirstOrDefault();
            var id = (last == null) ? 1 : (int)last.ToBsonDocument()["_id"] + 1;
            return id;
        }

        public bool IsEmpty(object id)
        {
            return ((id is int) && (id as int? == 0));
        }
    }

    public class GuidIdGenerator : GuidGenerator, IIdGenerator
    {
        public object GenerateId(object container, object document)
        {
            return New();
        }

        public bool IsEmpty(object id)
        {
            return id == default;
        }
    }
}
