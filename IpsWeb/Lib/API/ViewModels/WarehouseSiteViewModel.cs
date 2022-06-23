using System.ComponentModel.DataAnnotations;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Lib.API.ViewModels
{
    public class WarehouseSiteViewModel
    {
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; }
        public double TopLength { get; set; }
        public double LeftLength { get; set; }
        public double Error { get; set; }
        public List<GatewayViewModel> Gateways { get; set; }
    }
}
