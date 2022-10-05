using MongoDB.Bson.Serialization;

namespace Vayosoft.Data.MongoDB
{
    public abstract class MongoClassMap<T>
    {
        protected MongoClassMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
                BsonClassMap.RegisterClassMap<T>(Map);
        }

        public abstract void Map(BsonClassMap<T> cm);
    }
}
