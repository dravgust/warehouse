using System.Globalization;

namespace Warehouse.Core.Application.PositioningSystem.Domain
{
    public class KSensor
    {
        public int Battery { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }

        public double? X0 { get; set; }
        public double? Y0 { get; set; }
        public double? Z0 { get; set; }

        public static KSensor Parse(string hexString)
        {
            if (string.IsNullOrEmpty(hexString) ||
                hexString.Length < 15 ||
                !hexString.StartsWith("0201060303AAF"))
                return null;

            var bytes = Convert.FromHexString(hexString);
            var buffer = new ArraySegment<byte>(bytes);

            var offset = 13;
            var mask = bytes[offset];

            var result = new KSensor
            {
                Battery = IsBitSet(mask, 0) ? BitConverter.ToInt16(buffer.Skip(offset += 1).Take(2).Reverse().ToArray(), 0) : 0,
                Temperature = IsBitSet(mask, 1) ? double.Parse($"{(int)bytes[offset += 2]}.{(int)bytes[offset + 1]}", CultureInfo.InvariantCulture) : null,
                Humidity = IsBitSet(mask, 2) ? double.Parse($"{(int)bytes[offset += 2]}.{(int)bytes[offset + 1]}", CultureInfo.InvariantCulture) : null,

            };

            if (IsBitSet(mask, 3))
            {
                result.X0 = BitConverter.ToInt16(buffer.Skip(offset += 2).Take(2).Reverse().ToArray(), 0);
                result.Y0 = BitConverter.ToInt16(buffer.Skip(offset += 2).Take(2).Reverse().ToArray(), 0);
                result.Z0 = BitConverter.ToInt16(buffer.Skip(offset + 2).Take(2).Reverse().ToArray(), 0);
            }

            return result;
        }

        private static bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }
    }
}
