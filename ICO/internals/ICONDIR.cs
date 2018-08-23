using System;

namespace ICO
{
    // ICO file header : 6 bytes
    internal class ICONDIR
    {
        public static readonly int SIZE = 6;

        public ICOType Type;
        public int ImageCount;

        public ICONDIR(ICOType type, int imageCount)
        {
            this.Type = type;
            this.ImageCount = imageCount;
        }

        public ICONDIR(ByteStream stream)
        {
            this.Deserialize(stream);
        }
        public ICONDIR(byte[] data) : this(new ByteStream(data)) { }

        public static byte[] Serialize(ICOType type, int imageCount)
        {
            return new ICONDIR(type, imageCount).Serialize();
        }

        public byte[] Serialize()
        {
            ByteStream stream = new ByteStream(ICONDIR.SIZE);
            // Reserved, must be 0 : 2 bytes
            stream.Write16(0);
            // Image type -> 1=ICO, 2=CUR : 2 bytes
            switch (this.Type)
            {
                case ICOType.ICO:
                    stream.Write16(1);
                    break;
                case ICOType.CUR:
                    stream.Write16(2);
                    break;
                default:
                    throw new FormatException("Invalid ICO type");
            }
            // Number of images : 2 bytes
            stream.Write16(this.ImageCount);
            return stream.Buffer;
        }

        public void Deserialize(ByteStream stream)
        {
            // Reserved, must be 0 : 2 bytes
            int reserved = stream.Read16();
            if (reserved != 0)
                throw new FormatException("Invalid ICONDIR reserved value");
            // Image type -> 1=ICO, 2=CUR : 2 bytes
            int type = stream.Read16();
            switch (type)
            {
                case 1:
                    this.Type = ICOType.ICO;
                    break;
                case 2:
                    this.Type = ICOType.CUR;
                    break;
                default:
                    throw new FormatException("Invalid ICONDIR type value");
            }
            // Number of images : 2 bytes
            this.ImageCount = stream.Read16();
        }

        public void Deserialize(byte[] data)
        {
            ByteStream stream = new ByteStream(data);
            this.Deserialize(stream);
        }
    }
}
