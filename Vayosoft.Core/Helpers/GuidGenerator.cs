using System;

namespace Vayosoft.Core.Helpers
{
    /// <summary>
    ///     Comb Guid Id Generation. More info http://www.informit.com/articles/article.aspx?p=25862
    /// </summary>
    public class GuidGenerator : IIdentityGenerator
    {
        /// <summary>
        ///     Returns a new Guid COMB, consisting of a random Guid combined with the provided timestamp.
        /// </summary>
        public static Guid New(DateTimeOffset timestamp) => Create(Guid.NewGuid(), timestamp);

        Guid IIdentityGenerator<Guid>.New() => New();

        public static Guid New() => Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

        private static byte[] DateTimeToBytes(DateTimeOffset timestamp)
        {
            byte[] bytes = BitConverter.GetBytes(timestamp.ToUnixTimeMilliseconds());
            byte[] numArray = new byte[6];
            if (BitConverter.IsLittleEndian)
            {
                Array.Copy(bytes, 2, numArray, 0, 4);
                Array.Copy(bytes, 0, numArray, 4, 2);
            }
            else
                Array.Copy(bytes, 2, numArray, 0, 6);
            return numArray;
        }

        private static DateTimeOffset BytesToDateTime(byte[] value)
        {
            byte[] numArray = new byte[8];
            if (BitConverter.IsLittleEndian)
            {
                Array.Copy(value, 4, numArray, 0, 2);
                Array.Copy(value, 0, numArray, 2, 4);
            }
            else
                Array.Copy(value, 0, numArray, 2, 6);
            long int64 = BitConverter.ToInt64(numArray, 0);
            return DateTimeOffset.FromUnixTimeMilliseconds(0L).AddMilliseconds((double)int64);
        }

        public static Guid Create(Guid value, DateTimeOffset timestamp)
        {
            byte[] byteArray = value.ToByteArray();
            Array.Copy(DateTimeToBytes(timestamp), 0, byteArray, 0, 6);
            return new Guid(byteArray);
        }

        public static DateTimeOffset GetTimestamp(Guid comb) => BytesToDateTime(comb.ToByteArray());
    }
}
