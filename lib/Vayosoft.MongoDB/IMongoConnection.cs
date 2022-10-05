using MongoDB.Driver;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.MongoDB
{
    public interface IMongoConnection
    {
        IMongoDatabase Database { get; }
        IClientSessionHandle Session { get; }

        Task<IClientSessionHandle> StartSession(CancellationToken cancellationToken = default);

        IMongoDatabase GetDatabase(string db);

        IMongoCollection<T> Collection<T>(CollectionName collectionName = null) where T : IEntity;
    }
}
