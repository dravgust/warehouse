using System.Diagnostics;
using System.Linq.Expressions;
using MongoDB.Driver;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Vayosoft.MongoDB.Extensions
{
    using static String;
    public static class CollectionExtensions
    {
        public static IMongoCollection<T> GetDocumentCollection<T>(this IMongoDatabase database, CollectionName collectionName = null)
            where T : IEntity
            => GetDocumentCollection<T>(database, collectionName ?? CollectionName.For<T>(), null);

        public static IMongoCollection<T> GetDocumentCollection<T>(
            this IMongoDatabase database,
            MongoCollectionSettings settings
        ) where T : IEntity
            => GetDocumentCollection<T>(database, CollectionName.For<T>(), settings);

        public static IMongoCollection<T> GetDocumentCollection<T>(
            this IMongoDatabase database,
            CollectionName collectionName,
            MongoCollectionSettings settings
        ) where T : IEntity
            => database.GetCollection<T>(collectionName == null ? CollectionName.For<T>() : collectionName, settings);

        public static Task<bool> DocumentExists<T>(
            this IMongoCollection<T> collection,
            string id,
            CancellationToken cancellationToken = default
        ) where T : IEntity<string>
        {
            if (IsNullOrWhiteSpace(id)) throw new ArgumentException("Document Id cannot be null or whitespace.", nameof(id));

            return collection
                .Find(x => x.Id == id)
                .AnyAsync(cancellationToken);
        }

        public static Task<T> LoadDocument<T>(
            this IMongoCollection<T> collection,
            string id,
            CancellationToken cancellationToken = default
        ) where T : IEntity<string>
        {
            if (IsNullOrWhiteSpace(id)) throw new ArgumentException("Document Id cannot be null or whitespace.", nameof(id));

            return collection
                .Find(x => x.Id == id)
                .Limit(1)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public static Task<TResult> LoadDocumentAs<T, TResult>(
            this IMongoCollection<T> collection,
            string id,
            Expression<Func<T, TResult>> projection,
            CancellationToken cancellationToken = default
        ) where T : IEntity<string>
        {
            if (IsNullOrWhiteSpace(id)) throw new ArgumentException("Document Id cannot be null or whitespace.", nameof(id));

            if (projection == null) throw new ArgumentNullException(nameof(projection));

            return collection
                .Find(x => x.Id == id)
                .Limit(1)
                .Project(projection)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public static Task<List<T>> LoadDocuments<T>(
            this IMongoCollection<T> collection,
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default
        ) where T : IEntity
        {
            var idsList = ids.ToList();

            if (ids == null || idsList.Count == 0 || idsList.Any(IsNullOrWhiteSpace))
                throw new ArgumentException("Document ids collection cannot be empty or contain empty values", nameof(ids));

            return collection
                .Find(Builders<T>.Filter.In(x => x.Id, idsList))
                .ToListAsync(cancellationToken);
        }

        public static Task<List<TResult>> LoadDocumentsAs<T, TResult>(
            this IMongoCollection<T> collection,
            IEnumerable<string> ids,
            Expression<Func<T, TResult>> projection,
            CancellationToken cancellationToken = default
        ) where T : IEntity
        {
            var idsList = ids.ToList();

            if (ids == null || idsList.Count == 0 || idsList.Any(IsNullOrWhiteSpace))
                throw new ArgumentException("Document ids collection cannot be empty or contain empty values", nameof(ids));

            if (projection == null) throw new ArgumentNullException(nameof(projection), "Projection must be specified");

            return collection
                .Find(Builders<T>.Filter.In(x => x.Id, idsList))
                .Project(projection)
                .ToListAsync(cancellationToken);
        }

        public static Task<TResult> LoadDocumentAs<T, TResult>(
            this IMongoCollection<T> collection,
            string id,
            ProjectionDefinition<T, TResult> projection,
            CancellationToken cancellationToken = default
        ) where T : IEntity<string>
        {
            if (IsNullOrWhiteSpace(id)) throw new ArgumentException("Document Id cannot be null or whitespace.", nameof(id));

            if (projection == null) throw new ArgumentNullException(nameof(projection));

            return collection
                .Find(x => x.Id == id)
                .Limit(1)
                .Project(projection)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public static Task<TResult> LoadDocumentAs<T, TResult>(
            this IMongoCollection<T> collection,
            string id,
            Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> projection,
            CancellationToken cancellationToken = default
        ) where T : IEntity<string>
            => collection.LoadDocumentAs<T, TResult>(id, projection(Builders<T>.Projection), cancellationToken);

        /// <summary>
        /// Replaces the document and by default inserts a new one if no matching document by id is found.
        /// </summary>
        public static async Task ReplaceDocument<T>(
            this IMongoCollection<T> collection,
            T document,
            Action<ReplaceOptions> configure,
            CancellationToken cancellationToken = default
        ) where T : IEntity
        {
            if (document == null) throw new ArgumentNullException(nameof(document), "Document cannot be null.");

            var options = new ReplaceOptions { IsUpsert = true };

            configure?.Invoke(options);

            var result = await collection.ReplaceOneAsync(
                x => x.Id == document.Id,
                document,
                options,
                cancellationToken
            );
        }

        /// <summary>
        /// Replaces the document and by default inserts a new one if no matching document by id is found.
        /// </summary>
        public static Task ReplaceDocument<T>(
            this IMongoCollection<T> collection,
            T document,
            CancellationToken cancellationToken = default
        ) where T : IEntity
            => collection.ReplaceDocument(document, null, cancellationToken);

        public static async Task<bool> DeleteDocument<T, TKey>(
            this IMongoCollection<T> collection,
            TKey id,
            CancellationToken cancellationToken = default
        ) where T : IEntity<TKey>
        {
            if (id != null && !default(TKey)!.Equals(id)) throw new ArgumentException("Document Id cannot be null or whitespace.", nameof(id));

            var result = await collection.DeleteOneAsync(x => x.Id.Equals(id), cancellationToken);

            return result.DeletedCount == 1;
        }

        public static async Task<long> DeleteManyDocuments<T>(
            this IMongoCollection<T> collection,
            FilterDefinition<T> filter,
            CancellationToken cancellationToken = default
        ) where T : IEntity
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            var result = await collection.DeleteManyAsync(filter, cancellationToken);

            return result.DeletedCount;
        }

        public static Task<long> DeleteManyDocuments<T>(
            this IMongoCollection<T> collection,
            Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter,
            CancellationToken cancellationToken = default
        ) where T : IEntity
            => collection.DeleteManyDocuments(filter(Builders<T>.Filter), cancellationToken);

        public static async Task<long> BulkUpdateDocuments<T>(
            this IMongoCollection<T> collection,
            IEnumerable<T> documents,
            Func<T, FilterDefinitionBuilder<T>, FilterDefinition<T>> filter,
            Func<T, UpdateDefinitionBuilder<T>, UpdateDefinition<T>> update,
            Action<BulkWriteOptions> configure,
            CancellationToken cancellationToken = default
        ) where T : IEntity
        {
            var options = new BulkWriteOptions();

            configure(options);

            var models = documents.Select(
                document => new UpdateOneModel<T>(
                    filter(document, Builders<T>.Filter),
                    update(document, Builders<T>.Update)
                )
            );

            var result = await collection.BulkWriteAsync(models, options, cancellationToken);

            return result.ModifiedCount;
        }

        public static async Task<BulkWriteResult> BulkWriteDocuments<T>(
            this IMongoCollection<T> collection,
            IEnumerable<T> documents,
            Func<T, WriteModel<T>> write,
            Action<BulkWriteOptions> configure,
            CancellationToken cancellationToken = default
        ) where T : IEntity
        {
            var options = new BulkWriteOptions();

            configure(options);

            return await collection
                    .BulkWriteAsync(documents.Select(write), options, cancellationToken);
        }

        public static Task<long> BulkUpdateDocuments<T>(
            this IMongoCollection<T> collection,
            IEnumerable<T> documents,
            Func<T, FilterDefinitionBuilder<T>, FilterDefinition<T>> filter,
            Func<T, UpdateDefinitionBuilder<T>, UpdateDefinition<T>> update,
            CancellationToken cancellationToken = default
        ) where T : IEntity
            => collection.BulkUpdateDocuments(documents, filter, update, null, cancellationToken);

        public static Task<string> CreateDocumentIndex<T>(
            this IMongoCollection<T> collection,
            Func<IndexKeysDefinitionBuilder<T>, IndexKeysDefinition<T>> index,
            Action<CreateIndexOptions> configure = null
        ) where T : IEntity
        {
            var options = new CreateIndexOptions();

            configure?.Invoke(options);

            return collection.Indexes.CreateOneAsync(
                new CreateIndexModel<T>(
                    index(Builders<T>.IndexKeys),
                    options
                )
            );
        }

        public static async Task<string> CreateDocumentIndex<T>(
            this IMongoCollection<T> collection,
            Func<IndexKeysDefinitionBuilder<T>, IndexKeysDefinition<T>> index,
            Action<CreateIndexOptions> configure,
            CancellationToken cancellationToken
        ) where T : IEntity
        {
            var options = new CreateIndexOptions();

            configure?.Invoke(options);

            try
            {
                return await CreateIndex();
            }
            catch (MongoCommandException ex) when (ex.Message.Contains("already exists"))
            {
                Trace.TraceError("Index already exists {0}", ex.Result);
            }

            return Empty;

            Task<string> CreateIndex()
                => collection.Indexes.CreateOneAsync(
                    new CreateIndexModel<T>(
                        index(Builders<T>.IndexKeys),
                        options
                    ),
                    cancellationToken: cancellationToken
                );
        }

        #region . Update .

        /// <summary>
        /// Updates a document and by default inserts a new one if no matching document is found.
        /// </summary>
        public static async Task UpdateDocument<T, TKey>(
            this IMongoCollection<T> collection,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            Action<UpdateOptions> configure,
            CancellationToken cancellationToken = default
        ) where T : IEntity<TKey>
        {
            var options = new UpdateOptions { IsUpsert = true };

            configure?.Invoke(options);

            var result = await collection.UpdateOneAsync(
                filter,
                update,
                options,
                cancellationToken
            );
        }

        /// <summary>
        /// Updates a document and by default inserts a new one if no matching document is found.
        /// </summary>
        public static Task UpdateDocument<T, TKey>(
            this IMongoCollection<T> collection,
            Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter,
            Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> update,
            Action<UpdateOptions> configure,
            CancellationToken cancellationToken = default
        ) where T : IEntity<TKey>
            => collection.UpdateDocument<T, TKey>(
                filter(Builders<T>.Filter),
                update(Builders<T>.Update),
                configure,
                cancellationToken
            );

        /// <summary>
        /// Updates a document and by default inserts a new one if no matching document is found.
        /// </summary>
        public static Task UpdateDocument<T, TKey>(
            this IMongoCollection<T> collection,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            CancellationToken cancellationToken = default
        ) where T : IEntity<TKey>
            => collection.UpdateDocument<T, TKey>(filter, update, null, cancellationToken);

        /// <summary>
        /// Updates a document and by default inserts a new one if no matching document is found.
        /// </summary>
        public static Task UpdateDocument<T, TKey>(
            this IMongoCollection<T> collection,
            Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter,
            Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> update,
            CancellationToken cancellationToken = default
        ) where T : IEntity<TKey>
            => collection.UpdateDocument<T, TKey>(
                filter(Builders<T>.Filter),
                update(Builders<T>.Update),
                null,
                cancellationToken
            );

        /// <summary>
        /// Updates a document and by default inserts a new one if no matching document by id is found.
        /// </summary>
        public static Task UpdateDocument<T, TKey>(
            this IMongoCollection<T> collection,
            TKey id,
            UpdateDefinition<T> update,
            Action<UpdateOptions> configure,
            CancellationToken cancellationToken = default
        ) where T : IEntity<TKey>
        {
            if (id != null && !default(TKey)!.Equals(id)) throw new ArgumentException("Document Id cannot be null or whitespace.", nameof(id));

            return collection.UpdateDocument<T, TKey>(
                Builders<T>.Filter.Eq(x => x.Id, id),
                update,
                configure,
                cancellationToken
            );
        }

        /// <summary>
        /// Updates a document and by default inserts a new one if no matching document by id is found.
        /// </summary>
        public static Task UpdateDocument<T, TKey>(
            this IMongoCollection<T> collection,
            TKey id,
            Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> update,
            Action<UpdateOptions> configure,
            CancellationToken cancellationToken = default
        ) where T : IEntity<TKey>
            => collection.UpdateDocument(
                id,
                update(Builders<T>.Update),
                configure,
                cancellationToken
            );

        /// <summary>
        /// Updates a document and by default inserts a new one if no matching document by id is found.
        /// </summary>
        public static Task UpdateDocument<T, TKey>(
            this IMongoCollection<T> collection,
            TKey id,
            UpdateDefinition<T> update,
            CancellationToken cancellationToken = default
        ) where T : IEntity<TKey>
            => collection.UpdateDocument(id, update, null, cancellationToken);

        /// <summary>
        /// Updates a document and by default inserts a new one if no matching document by id is found.
        /// </summary>
        public static Task UpdateDocument<T, TKey>(
            this IMongoCollection<T> collection,
            TKey id,
            Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> update,
            CancellationToken cancellationToken = default
        ) where T : IEntity<TKey>
            => collection.UpdateDocument(id, update, null, cancellationToken);

        #endregion

        #region . UpdateMany .

        /// <summary>
        /// Updates documents and by default inserts new ones if no matching documents are found.
        /// </summary>
        public static async Task<long> UpdateManyDocuments<T>(
            this IMongoCollection<T> collection,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            Action<UpdateOptions> configure,
            CancellationToken cancellationToken = default
        ) where T : IEntity
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (update == null) throw new ArgumentNullException(nameof(update));

            var options = new UpdateOptions { IsUpsert = true };

            configure?.Invoke(options);

            var result = await collection.UpdateManyAsync(filter, update, options, cancellationToken);

            return result.ModifiedCount;
        }

        /// <summary>
        /// Updates documents and by default inserts new ones if no matching documents are found.
        /// </summary>
        public static Task<long> UpdateManyDocuments<T>(
            this IMongoCollection<T> collection,
            Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter,
            Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> update,
            Action<UpdateOptions> configure,
            CancellationToken cancellationToken = default
        ) where T : IEntity
            => collection.UpdateManyDocuments(filter(Builders<T>.Filter), update(Builders<T>.Update), configure, cancellationToken);

        /// <summary>
        /// Updates documents and by default inserts new ones if no matching documents are found.
        /// </summary>
        public static Task<long> UpdateManyDocuments<T>(
            this IMongoCollection<T> collection,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            CancellationToken cancellationToken = default
        ) where T : IEntity
            => collection.UpdateManyDocuments(filter, update, null, cancellationToken);

        /// <summary>
        /// Updates documents and by default inserts new ones if no matching documents by filter are found.
        /// </summary>
        public static Task<long> UpdateManyDocuments<T>(
            this IMongoCollection<T> collection,
            Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter,
            Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> update,
            CancellationToken cancellationToken = default
        ) where T : IEntity
            => collection.UpdateManyDocuments(filter(Builders<T>.Filter), update(Builders<T>.Update), null, cancellationToken);

        #endregion

        public static Task<IPagedEnumerable<T>> AggregateByPage<T>(
            this IMongoCollection<T> collection,
            IPagingModel<T, object> model,
            FilterDefinition<T> filterDefinition,
            CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            var sortDefinition = model.OrderBy.SortOrder == SortOrder.Asc 
                ? Builders<T>.Sort.Ascending(model.OrderBy.Expression) 
                : Builders<T>.Sort.Descending(model.OrderBy.Expression);

            return collection.AggregateByPage(filterDefinition, sortDefinition, model.Page, model.Size, cancellationToken);
        }

        public static async Task<IPagedEnumerable<T>> AggregateByPage<T>(
            this IMongoCollection<T> collection,
            FilterDefinition<T> filterDefinition,
            SortDefinition<T> sortDefinition,
            int page,
            int pageSize, CancellationToken cancellationToken = default)
            where T : IEntity
        {
            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<T, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<T>()
                }));

            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<T, T>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(sortDefinition),
                    PipelineStageDefinitionBuilder.Skip<T>((page - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<T>(pageSize),
                }));


            var aggregation = await collection.Aggregate()
                .Match(filterDefinition)
                .Facet(countFacet, dataFacet)
                .ToListAsync(cancellationToken);

            var count = aggregation.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()
                ?.FirstOrDefault()
                ?.Count ?? 0;

            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<T>();

            return new PagedCollection<T>(data, count);
        }
    }
}
