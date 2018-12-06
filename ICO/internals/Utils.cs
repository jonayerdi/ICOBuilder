
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
        public static byte[] Merge(byte[] first, byte[] second)
        {
            byte[] result = new byte[first.Length + second.Length];
            for (int i = 0; i < first.Length; i++)
                result[i] = first[i];
            for (int i = 0; i < second.Length; i++)
                result[first.Length + i] = second[i];
            return result;
        }
        public static void Replace(byte[] original, byte[] replacement, int offset)
        {
            for (int i = 0; i < replacement.Length; i++)
                original[offset + i] = replacement[i];
        }
    }
    internal class ByteStream
    {
        public byte[] Buffer;
        public int Index;

        public ByteStream(byte[] buffer)
        {
            this.Buffer = buffer;
            this.Index = 0;
        }
        public ByteStream(int size) : this(new byte[size]) { }

        public void Write(byte[] data)
        {
            Bytes.Replace(this.Buffer, data, this.Index);
            this.Index += data.Length;
        }

        public byte[] Read(int count)
        {
            byte[] result = Bytes.Subset(this.Buffer, this.Index, count);
            this.Index += count;
            return result;
        }

        public void Write8(int num)
        {
            this.Write(Bytes.FromInt(num, 1));
        }
        public void Write16(int num)
        {
            this.Write(Bytes.FromInt(num, 2));
        }
        public void Write32(int num)
        {
            this.Write(Bytes.FromInt(num, 4));
        }

        public int Read8()
        {
            return Bytes.FromBytes(this.Read(1), 0, 1);
        }
        public int Read16()
        {
            return Bytes.FromBytes(this.Read(2), 0, 2);
        }
        public int Read32()
        {
            return Bytes.FromBytes(this.Read(4), 0, 4);
        }
    }
}
