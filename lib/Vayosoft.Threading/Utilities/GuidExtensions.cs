using System;
using System.Buffers.Text;
using System.Runtime.InteropServices;

namespace Vayosoft.Threading.Utilities
{
    public static class GuidExtensions
    {
        private const char Underscore = '_';
        private const char Hyphen = '-';
        private const byte SlashByte = (byte)'/';
        private const byte PlusByte = (byte)'+';

        public static string ToShortUID(this Guid id)
        {
            Span<byte> idBytes = stackalloc byte[16];
            Span<byte> base64Bytes = stackalloc byte[24];

            MemoryMarshal.TryWrite(idBytes, ref id);
            Base64.EncodeToUtf8(idBytes, base64Bytes, out _, out _);

            Span<char> result = stackalloc char[22];
            for (var i = 0; i < 22; i++)
            {
                result[i] = base64Bytes[i] switch
                {
                    SlashByte => Hyphen,
                    PlusByte => Underscore,
                    _ => (char)base64Bytes[i]
                };
            }

            return new string(result);
        }
    }
}
