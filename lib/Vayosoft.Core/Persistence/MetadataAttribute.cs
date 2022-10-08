namespace Vayosoft.Core.Persistence
{
    public sealed class MetadataAttribute : Attribute
    {
        public string Name { get; set; }
        public MetadataAttribute(string name)
        {
            Name = name;
        }
    }
}
