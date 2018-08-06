using System;
using System.Collections.Generic;

namespace ICO
{
    public enum ICOType
    {
        ICO,
        CUR
    }
    public class ICOFile
    {
        public static readonly int ICONDIR_SIZE = 6;
        public static readonly int ICONDIRENTRY_SIZE = 16;

        public ICOType Type { get; set; }
        public List<ICOImage> Images { get; set; }

        public ICOFile(ICOType type)
        {
            this.Images = new List<ICOImage>();
            this.Type = type;
        }

        public ICOFile(byte[] data)
        {
            this.Images = new List<ICOImage>();
            this.Deserialize(data);
        }

        private void Deserialize(byte[] data)
        {

        }

        private int OffsetOfImage(ICOImage img)
        {
            int headersSize = ICONDIR_SIZE + (ICONDIRENTRY_SIZE * Images.Count);
            int imagesOffset = 0;
            for(int i = 0; i < Images.Count; i++)
            {
                if (Images[i] == img)
                    break;
                imagesOffset += Images[i].Serialized.Length;
            }
            return headersSize + imagesOffset;
        }

        // ICO file header : 6 bytes
        private byte[] SerializeICONDIR()
        {
            List<byte> ICONDIR = new List<byte>();
            // Reserved, must be 0 : 2 bytes
            ICONDIR.AddRange(Bytes.FromInt(0, 2));
            // Image type -> 1=ICO, 2=CUR : 2 bytes
            switch (Type)
            {
                case ICOType.ICO:
                    ICONDIR.AddRange(Bytes.FromInt(1, 2));
                    break;
                case ICOType.CUR:
                    ICONDIR.AddRange(Bytes.FromInt(2, 2));
                    break;
                default:
                    throw new FormatException("Invalid ICO type");
            }
            // Number of images : 2 bytes
            ICONDIR.AddRange(Bytes.FromInt(Images.Count, 2));
            return ICONDIR.ToArray();
        }

        // Entry per image on the ICO file : 16 bytes
        private byte[] SerializeICONDIRENTRY(ICOImage img)
        {
            List<byte> ICONDIRENTRY = new List<byte>();
            // Width of the image in pixels, 0 means 256 pixels : 1 byte
            if (img.Width < 256)
                ICONDIRENTRY.AddRange(Bytes.FromInt(img.Width, 1));
            else if (img.Width == 256)
                ICONDIRENTRY.AddRange(Bytes.FromInt(0, 1));
            else
                throw new FormatException("Invalid image Width");
            // Height of the image in pixels, 0 means 256 pixels : 1 byte
            if (img.Height < 256)
                ICONDIRENTRY.AddRange(Bytes.FromInt(img.Height, 1));
            else if(img.Height == 256)
                ICONDIRENTRY.AddRange(Bytes.FromInt(0, 1));
            else
                throw new FormatException("Invalid image Height");
            // Number of colors in color palette, 0 if no color palette : 1 byte
            ICONDIRENTRY.AddRange(Bytes.FromInt(0, 1));
            // Reserved, must be 0 : 1 byte
            ICONDIRENTRY.AddRange(Bytes.FromInt(0, 1));
            // The following fields have different meanings for ICO or CUR
            switch (Type)
            {
                case ICOType.ICO:
                    // Specifies color planes, should be 0 or 1 : 2 bytes
                    ICONDIRENTRY.AddRange(Bytes.FromInt(0, 2));
                    // Bits per pixel of the image : 2 bytes
                    ICONDIRENTRY.AddRange(Bytes.FromInt(img.BitsPerPixel, 2));
                    break;
                case ICOType.CUR:
                    // Horizontal coordinates of the hotspot in number of pixels from the left : 2 bytes
                    ICONDIRENTRY.AddRange(Bytes.FromInt(img.HotspotX, 2));
                    // Vertical coordinates of the hotspot in number of pixels from the top : 2 bytes
                    ICONDIRENTRY.AddRange(Bytes.FromInt(img.HotspotY, 2));
                    break;
                default:
                    throw new FormatException("Invalid ICO type");
            }
            // Image data size: 4 bytes
            ICONDIRENTRY.AddRange(Bytes.FromInt(img.Serialized.Length, 4));
            // Offset of image data from the beginning of the ICO file: 4 bytes
            ICONDIRENTRY.AddRange(Bytes.FromInt(OffsetOfImage(img), 4));
            return ICONDIRENTRY.ToArray();
        }

        private byte[] SerializeHeaders()
        {
            List<byte> headers = new List<byte>();
            headers.AddRange(SerializeICONDIR());
            foreach(ICOImage img in Images)
                headers.AddRange(SerializeICONDIRENTRY(img));
            return headers.ToArray();
        }

        public byte[] Serialize()
        {
            List<byte> ico = new List<byte>();
            foreach (ICOImage img in Images)
                img.Serialize();
            ico.AddRange(SerializeHeaders());
            foreach (ICOImage img in Images)
                ico.AddRange(img.Serialized);
            return ico.ToArray();
        }
    }
}
