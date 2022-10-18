namespace Warehouse.Core.Application.TrackingReports.Models
{
    public record TelemetryTickReport(DateTime DateTime)
    {
        public double? Temperature { get; init; }
        public double? Humidity { get; init; }
    }
}
