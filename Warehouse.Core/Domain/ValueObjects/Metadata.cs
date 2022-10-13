namespace Warehouse.Core.Domain.ValueObjects
{
    public sealed class Metadata : List<MetadataItem>
    { }

    public sealed record MetadataItem(string Key, string Value, string Type, bool IsRequired);
}
