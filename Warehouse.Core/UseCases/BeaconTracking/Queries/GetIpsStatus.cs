﻿using Vayosoft.Core.Queries;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetIpsStatus : IQuery<DashboardBySite>
    {
        public string SiteId { set; get; }
    }
}
