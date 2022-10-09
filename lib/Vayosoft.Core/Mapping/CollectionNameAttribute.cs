namespace Vayosoft.Core.Mapping
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CollectionNameAttribute : Attribute
    {
        public string Name { get; set; }
        public CollectionNameAttribute(string name)
        {
            Name = name;
        }
    }
}
