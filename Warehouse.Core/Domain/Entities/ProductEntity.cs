using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Domain.Entities
{
    public class ProductEntity : EntityBase<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string MacAddress { get; set; }

        public ProductMetadata Metadata { get; set; }
    }

    public class ProductMetadata : List<ProductMetadataItem> { }

    public class ProductMetadataItem
    {
        public string? Key { set; get; }
        public string? Value { set; get; }
        public string? Type { set; get; }
        public bool IsRequired { set; get; }
    }

    public class FileEntity : IEntity<string>
    {
        object IEntity.Id => Id;

        public string Id { get; }
        public string Content { set; get; }
    }

}
