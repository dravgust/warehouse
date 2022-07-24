using Vayosoft.IPS.Configuration;

namespace Vayosoft.IPS.Domain
{
    public class GenericSite
    {
        private readonly HashSet<string> _inBound =  new();
        private readonly HashSet<string> _outBound = new();

        public string Id { get; set; }
        public double TopLength { get; set; }
        public double LeftLength { get; set; }
        public double Error { get; set; }
        public List<GenericGateway> Gateways { get; set; } = new();
        public ICalculationSettings Settings { get; set; }

        public IndoorPositionStatus Status =>
            new()
            {
                In = _inBound,
                Out = _outBound
            };

        public IEnumerable<string> GetBeaconMacAddressList()
        {
            if(_inBound == null || _outBound == null) yield break;

            foreach (var b in _inBound) yield return b;
            foreach (var b in _outBound) yield return b;
        }
        public GenericSite(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            Id = id;
        }

        public GenericSite(string id, double topLength, double leftLength, ICalculationSettings settings): this(id)
        {
            TopLength = topLength;
            LeftLength = leftLength;
            Settings = settings;
        }

        public void AddGateway(GenericGateway gateway) => Gateways.Add(gateway);

        public void CalcBeaconsPosition()
        {
            _inBound.Clear();
            _outBound.Clear();

            foreach (var ipsGateway in Gateways)
            {
                ipsGateway.CalcCircumscribedRadius(TopLength, LeftLength);
                
                ipsGateway.CalcBeaconsDistance(Settings);

                var radius = ipsGateway.CircumscribedRadius - Error;
                CheckInboundStatus(ipsGateway.Gauge, radius, _outBound, _inBound);
                foreach (var gwBeacon in ipsGateway.Beacons)
                {
                    CheckInboundStatus(gwBeacon, radius, _outBound, _inBound);
                }
            }
        }

        private static void CheckInboundStatus(IBeacon gwBeacon, double radius, ISet<string> outBound, ISet<string> inBound)
        {
            if (gwBeacon.Radius <= radius)
            {
                if (outBound.Contains(gwBeacon.MacAddress))
                    inBound.Remove(gwBeacon.MacAddress);
                if (!inBound.Contains(gwBeacon.MacAddress))
                    inBound.Add(gwBeacon.MacAddress);
            }
            else
            {
                if (!outBound.Contains(gwBeacon.MacAddress))
                    outBound.Add(gwBeacon.MacAddress);
            }
        }
    }
}
