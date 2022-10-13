namespace Vayosoft.Core.Mapping
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AggregateNameAttribute : Attribute
    {
        public string Name { get; set; }
        public AggregateNameAttribute(string name)
        {
            Name = name;
        }
    }
}
