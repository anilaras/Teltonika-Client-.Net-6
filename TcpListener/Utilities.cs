using System.Linq;

namespace TcpListener
{
    public class Utilities
    {
        public static string ToHexString(byte[] buffer, int offset, int count)
        {
            return string.Join("", buffer.Skip(offset).Take(count).Select(x => x.ToString("X2")).ToArray());
        }
    }
}