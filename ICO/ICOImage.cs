using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ICO
{
    public enum ICOImageType
    {
        BMP,
        PNG
    }
    public class ICOImage
    {
        private Bitmap image;
        public Bitmap Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = new Bitmap(value.Width, value.Height);
                for (int y = 0; y < value.Height; y++)
                    for (int x = 0; x < value.Width; x++)
                        this.image.SetPixel(x, y, value.GetPixel(x, y));
            }
        }
        public ICOImageType Type { get; set; }

        public int Width { get { return Image.Width; } }
        public int Height { get { return Image.Height; } }
        public Size Size { get { return Image.Size; } set { Image = new Bitmap(Image, value); } }

        internal int BitsPerPixel
        { get
            {
                if (this.Type == ICOImageType.BMP)
                    return BMP.GetBitsPerPixel(this.serialized);
                return 0;
            }
        }

        // These two are only for CUR files
        public int HotspotX { get; set; }
        public int HotspotY { get; set; }

        private byte[] serialized;
        public byte[] Serialized
        {
            get
            {
                return serialized;
            }
        }

        public ICOImage(System.Drawing.Image img)
        {
            this.Image = new Bitmap(img);
            this.Type = ICOImageType.PNG;
            this.HotspotX = 0;
            this.HotspotY = 0;
        }
        public ICOImage(string path) : this(System.Drawing.Image.FromFile(path)) { }
        public ICOImage(byte[] imageData)
        {
            MemoryStream stream = new MemoryStream(imageData);
            this.Image = new Bitmap(System.Drawing.Image.FromStream(stream));
            this.Type = ICOImageType.PNG;
            this.HotspotX = 0;
            this.HotspotY = 0;
        }
        public static ICOImage ReadFromFile(string path)
        {
            return new ICOImage(path);
        }

        internal ICOImage(ICOType type, ICONDIRENTRY icondirentry, byte[] icoData)
        {
            byte[] imageData = Bytes.Subset(icoData, icondirentry.Image.Offset, icondirentry.Image.Size);
            if(BMP.isStrippedBMP(imageData))
            {
                imageData = BMP.FromICO(icondirentry, imageData);
                this.Type = ICOImageType.BMP;
            }
            else
            {
                this.Type = ICOImageType.PNG;
            }
            MemoryStream stream = new MemoryStream(imageData);
            this.Image = new Bitmap(System.Drawing.Image.FromStream(stream));
            this.HotspotX = icondirentry.Image.HotspotX;
            this.HotspotY = icondirentry.Image.HotspotY;
        }

        public void Serialize()
        {
            MemoryStream stream = new MemoryStream();
            ImageFormat format = null;
            switch (this.Type)
            {
                case ICOImageType.BMP:
                    format = ImageFormat.Bmp;
                    break;
                case ICOImageType.PNG:
                    format = ImageFormat.Png;
                    break;
                default:
                    throw new FormatException("Invalid image format type");
            }
            this.Image.Save(stream, format);
            serialized = stream.ToArray();
            // For BMP images, strip BITMAPFILEHEADER + add AND mask
            if (Type == ICOImageType.BMP)
            {
                // Generate AND mask: false for transparent, true for opaque
                bool[] ANDmask = new bool[this.Height * this.Width];
                for (int i = 0; i < ANDmask.Length; i++)
                    ANDmask[i] = ((this.Image.GetPixel(i % this.Width, i / this.Width).A == 0) ? false : true);
                serialized = BMP.ToICO(serialized, ANDmask);
            }
        }
    }
}
