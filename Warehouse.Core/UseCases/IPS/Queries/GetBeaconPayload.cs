using Vayosoft.Core.Queries;
using Warehouse.Core.Entities.Models.Payloads;

namespace Warehouse.Core.UseCases.IPS.Queries
{
    public class GetBeaconPayload : IQuery<BeaconPayload>
    {
        public string MacAddress { set; get; }
    }
}
