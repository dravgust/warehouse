namespace Warehouse.Core.Application.PositioningReports.Models
{
    public class TelemetryViewModel
    {
        public string MacAddress { get; set; }
        public Dictionary<DateTime, double> Temperature { get; set; }
        public Dictionary<DateTime, double> Humidity { get; set; }
    }
}
