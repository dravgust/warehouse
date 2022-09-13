using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Entities.Models
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
