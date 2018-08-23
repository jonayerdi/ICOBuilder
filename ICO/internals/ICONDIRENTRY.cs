using System;

namespace ICO
{
    // ICONDIRENTRY info
    internal struct ICOImageInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int HotspotX { get; set; }
        public int HotspotY { get; set; }
        public int BitsPerPixel { get; set; }
        public int Size { get; set; }
        public int Offset { get; set; }
    }
    // Entry per image on the ICO file : 16 bytes
    internal class ICONDIRENTRY
    {
        public static readonly int SIZE = 16;

        public ICOType Type;
        public ICOImageInfo Image;

        public ICONDIRENTRY(ICOType type, ICOImageInfo image)
        {
            this.Type = type;
            this.Image = image;
        }
        public ICONDIRENTRY(ICOType type, ICOImage image, int imageOffset) : this(type, new ICOImageInfo
        {
            Width = image.Width,
            Height = image.Height,
            HotspotX = image.HotspotX,
            HotspotY = image.HotspotY,
            BitsPerPixel = image.BitsPerPixel,
            Size = image.Serialized.Length,
            Offset = imageOffset
        }) { }

        public ICONDIRENTRY(ICOType type, ByteStream stream)
        {
            this.Type = type;
            this.Deserialize(stream);
        }
        public ICONDIRENTRY(ICOType type, byte[] data) : this(type, new ByteStream(data)) { }

        public static byte[] Serialize(ICOType type, ICOImage image, int imageOffset)
        {
            return new ICONDIRENTRY(type, image, imageOffset).Serialize();
        }

        public byte[] Serialize()
        {
            ByteStream stream = new ByteStream(ICONDIRENTRY.SIZE);
            // Width of the image in pixels, 0 means 256 pixels : 1 byte
            if (this.Image.Width < 256)
                stream.Write8(this.Image.Width);
            else if (this.Image.Width == 256)
                stream.Write8(0);
            else
                throw new FormatException("Invalid image Width");
            // Height of the image in pixels, 0 means 256 pixels : 1 byte
            if (this.Image.Height < 256)
                stream.Write8(this.Image.Height);
            else if (this.Image.Height == 256)
                stream.Write8(0);
            else
                throw new FormatException("Invalid image Height");
            // Number of colors in color palette, 0 if no color palette : 1 byte
            stream.Write8(0);
            // Reserved, must be 0 : 1 byte
            stream.Write8(0);
            // The following fields have different meanings for ICO or CUR
            switch (this.Type)
            {
                case ICOType.ICO:
                    // Specifies color planes, should be 0 or 1 : 2 bytes
                    stream.Write16(0);
                    // Bits per pixel of the image : 2 bytes
                    stream.Write16(this.Image.BitsPerPixel);
                    break;
                case ICOType.CUR:
                    // Horizontal coordinates of the hotspot in number of pixels from the left : 2 bytes
                    stream.Write16(this.Image.HotspotX);
                    // Vertical coordinates of the hotspot in number of pixels from the top : 2 bytes
                    stream.Write16(this.Image.HotspotY);
                    break;
                default:
                    throw new FormatException("Invalid ICO type");
            }
            // Image data size: 4 bytes
            stream.Write32(this.Image.Size);
            // Offset of image data from the beginning of the ICO file: 4 bytes
            stream.Write32(this.Image.Offset);
            return stream.Buffer;
        }

        public void Deserialize(ByteStream stream)
        {
            // Width of the image in pixels, 0 means 256 pixels : 1 byte
            this.Image.Width = stream.Read8();
            if (this.Image.Width == 0)
                this.Image.Width = 256;
            else if(this.Image.Width < 0)
                throw new FormatException("Invalid image Width");
            // Height of the image in pixels, 0 means 256 pixels : 1 byte
            this.Image.Height = stream.Read8();
            if (this.Image.Height == 0)
                this.Image.Height = 256;
            else if (this.Image.Height < 0)
                throw new FormatException("Invalid image Height");
            // Number of colors in color palette, 0 if no color palette : 1 byte
            int colors = stream.Read8();
            // Reserved, must be 0 : 1 byte
            int reserved = stream.Read8();
            if (reserved != 0)
                throw new FormatException("Invalid ICONDIRENTRY reserved value");
            // The following fields have different meanings for ICO or CUR
            switch (this.Type)
            {
                case ICOType.ICO:
                    // Specifies color planes, should be 0 or 1 : 2 bytes
                    int colorPlanes = stream.Read16();
                    // Bits per pixel of the image : 2 bytes
                    this.Image.BitsPerPixel = stream.Read16();
                    break;
                case ICOType.CUR:
                    // Horizontal coordinates of the hotspot in number of pixels from the left : 2 bytes
                    this.Image.HotspotX = stream.Read16();
                    // Vertical coordinates of the hotspot in number of pixels from the top : 2 bytes
                    this.Image.HotspotY = stream.Read16();
                    break;
                default:
                    throw new FormatException("Invalid ICO type");
            }
            // Image data size: 4 bytes
            this.Image.Size = stream.Read32();
            // Offset of image data from the beginning of the ICO file: 4 bytes
            this.Image.Offset = stream.Read32();
        }

        public void Deserialize(byte[] data)
        {
            ByteStream stream = new ByteStream(data);
            this.Deserialize(stream);
        }
    }
}
