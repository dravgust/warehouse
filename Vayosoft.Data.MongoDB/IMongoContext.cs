using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Vayosoft.Data.MongoDB
{
    public interface IMongoContext
    {
        IClientSessionHandle Session { get; }

        Task<IClientSessionHandle> StartSession(CancellationToken cancellationToken = default);

        IMongoDatabase GetDatabase(string db);

        IMongoCollection<T> GetCollection<T>(string db, string collectionName);

        IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}
