using System.ComponentModel.DataAnnotations;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Application.UseCases.Management.Models
{
    public class WarehouseSiteDto: IEntity<string>
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public double TopLength { get; set; }
        public double LeftLength { get; set; }
        public double Error { get; set; }
        public List<GatewayDto> Gateways { get; set; }
        object IEntity.Id => Id;
    }
}
