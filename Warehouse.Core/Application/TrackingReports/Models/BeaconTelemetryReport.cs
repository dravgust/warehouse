namespace Warehouse.Core.Application.TrackingReports.Models
{
    public record BeaconTelemetryReport(string MacAddress)
    { 
        public Dictionary<DateTime, double> Temperature { get; init; }
        public Dictionary<DateTime, double> Humidity { get; init; }
    }
}
