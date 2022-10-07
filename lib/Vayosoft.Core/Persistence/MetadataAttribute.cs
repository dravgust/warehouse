namespace Vayosoft.Core.Persistence
{
    public class MetadataAttribute : Attribute
    {
        public string Name { get; set; }
        public MetadataAttribute(string name)
        {
            Name = name;
        }
    }
}
