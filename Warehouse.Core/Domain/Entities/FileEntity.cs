using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Domain.Entities
{
    public class FileEntity : IEntity<string>, IAggregateRoot
    {
        object IEntity.Id => Id;

        public string Id { get; }
        public string Content { set; get; }
    }
}
