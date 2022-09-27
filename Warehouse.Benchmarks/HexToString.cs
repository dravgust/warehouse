using BenchmarkDotNet.Attributes;

namespace Warehouse.Benchmarks
{
    [MemoryDiagnoser]
    public class HexToString
    {
        private readonly string _hexValue = "0201060303AA";

        [Benchmark]
        public byte[] ToStringFromHex()
        {
            return GetBytes(_hexValue);
        }

        [Benchmark]
        public byte[] GetBytesFromString()
        {
            return Convert.FromHexString(_hexValue);
        }

        public static byte[] GetBytes(string hexString)
        {
            return Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
