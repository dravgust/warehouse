using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Entities.Models
{
    public class ProductEntity : EntityBase<string>, IProvider<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ProductMetadata Metadata { get; set; }
        public long ProviderId { get; set; }
        object IProvider.ProviderId => ProviderId;
    }

    public class ProductMetadata : List<ProductMetadataItem> { }

    public class ProductMetadataItem
    {
        public string Key { set; get; }
        public string Value { set; get; }
        public string Type { set; get; }
        public bool IsRequired { set; get; }
    }

}
