using Vayosoft.Core.SharedKernel.ValueObjects;
using Warehouse.Core.Application.PositioningSystem.Settings;
using Warehouse.Core.Application.PositioningSystem.Filters;

namespace Warehouse.Core.Application.PositioningSystem.Domain
{
    public class GenericGateway : IComparable<GenericGateway>
    {
        public MacAddress MacAddress { get; }
        public double CircumscribedRadius { get; set; }
        public LocationAnchor Location { get; set; }
        public IBeacon Gauge { set; get; }
        public List<IBeacon> Beacons { get; } = new();

        //https://web.mst.edu/~kosbar/ee3430/ff/transmissionlines/propagation_coefficient/att_const/index.html#:~:text=The%20attenuation%20constant%20will%20be,move%20away%20from%20the%20source.
        public int EnvFactor { set; get; } = 1; //attenuation constant [1..4]

        public GenericGateway(MacAddress mac)
        {
            MacAddress = mac;
        }

        public void AddBeacon(IBeacon beacons) => Beacons.Add(beacons);
        public void AddBeacons(IEnumerable<IBeacon> beacons) => Beacons.AddRange(beacons);

        public void CalcCircumscribedRadius(double topLength, double leftLength)
        {
            CircumscribedRadius = Location switch
            {
                LocationAnchor.Unknown => 0,
                LocationAnchor.Center => Math.Round(Math.Pow(Math.Pow(topLength / 2, 2) + Math.Pow(leftLength / 2, 2), 0.5), 1),
                LocationAnchor.TopCenter or LocationAnchor.BottomCenter => Math.Round(Math.Pow(Math.Pow(topLength / 2, 2) + Math.Pow(leftLength, 2), 0.5), 1),
                LocationAnchor.CenterLeft or LocationAnchor.CenterRight => Math.Round(Math.Pow(Math.Pow(topLength, 2) + Math.Pow(leftLength / 2, 2), 0.5), 1),
                LocationAnchor.TopLeft or LocationAnchor.TopRight or LocationAnchor.BottomLeft or LocationAnchor.BottomRight => Math.Round(Math.Pow(Math.Pow(topLength, 2) + Math.Pow(leftLength, 2), 0.5), 1),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public void CalcBeaconsDistance(ICalculationSettings settings)
        {
            if(Gauge == null) return;

            Gauge.RssiFilter ??= settings.Resolve<IRssiFilter>(settings.RssiFilter);
            Gauge.ApplyBufferFilter();
            Gauge.CalcMethod ??= settings.CalcMethod;
            if (Gauge.TxPower == 0)
            {
                Gauge.Radius = Gauge.Radius > 0 ? Gauge.Radius : CircumscribedRadius;
                Gauge.CalcTxPower(EnvFactor);
            }
            else
            {
                Gauge.CalcRadius(EnvFactor);
            }

            foreach (var b in Beacons)
            {
                b.RssiFilter ??= settings.Resolve<IRssiFilter>(settings.RssiFilter);
                b.ApplyBufferFilter();
                b.CalcMethod ??= settings.CalcMethod;
                b.TxPower = Gauge.TxPower;
                b.CalcRadius(EnvFactor);
            }
        }

        public int CompareTo(GenericGateway other) =>
            this.MacAddress.CompareTo(other.MacAddress);
    }
}
