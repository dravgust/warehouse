using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Data.MongoDB
{
    public interface IMongoContext
    {
        IMongoDatabase Database { get; }
        IClientSessionHandle Session { get; }

        Task<IClientSessionHandle> StartSession(CancellationToken cancellationToken = default);

        IMongoDatabase GetDatabase(string db);

        IMongoCollection<T> Collection<T>(CollectionName collectionName = null) where T : IEntity;
    }
}
