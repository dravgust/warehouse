using Vayosoft.Commons.Entities;

namespace Warehouse.Core.Domain.Entities
{
    public class ProviderEntity : EntityBase<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Flags { get; set; }
        public long? Parent { get; set; }
        public string Culture { get; set; }
        public string Alias { get; set; }
    }
}
