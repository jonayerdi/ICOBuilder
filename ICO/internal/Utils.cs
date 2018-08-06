
namespace ICO
{
    internal static class Bytes
    {
        public static byte[] FromInt(int num, int byteCount)
        {
            byte[] result = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                result[i] = (byte)(num >> (8 * i));
            return result;
        }
        public static int FromBytes(byte[] bytes, int start, int count)
        {
            int result = 0;
            for (int i = 0; i < count; i++)
                result += ((int)bytes[start + i]) << (8 * i);
            return result;
        }
        public static byte[] Subset(byte[] bytes, int start, int count)
        {
            byte[] result = new byte[count];
            for (int i = 0; i < count; i++)
                result[i] = bytes[start + i];
            return result;
        }
        public static void Replace(byte[] original, byte[] replacement, int offset)
        {
            for (int i = 0; i < replacement.Length; i++)
                original[offset + i] = replacement[i];
        }
    }
}
