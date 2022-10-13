namespace Warehouse.Core.Application.PositioningReports.Models
{
    public record TelemetryReport(DateTime DateTime)
    {
        public double? Temperature { get; init; }
        public double? Humidity { get; init; }
    }
}
