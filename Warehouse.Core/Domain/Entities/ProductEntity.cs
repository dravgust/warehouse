using System.Text.Json.Serialization;
using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Domain.Entities
{
    public class ProductEntity : EntityBase<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        //[JsonConverter(typeof(MacAddressConverter))]
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

}
