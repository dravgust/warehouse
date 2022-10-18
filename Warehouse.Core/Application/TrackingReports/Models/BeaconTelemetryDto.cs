﻿namespace Warehouse.Core.Application.TrackingReports.Models
{
    public class BeaconTelemetryDto
    {
        public string MacAddress { get; set; }
        public DateTime ReceivedAt { get; set; }
        public double RSSI { get; set; }
        public double TxPower { get; set; }
        public int Battery { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public double? X0 { get; set; }
        public double? Y0 { get; set; }
        public double? Z0 { get; set; }
    }
}
