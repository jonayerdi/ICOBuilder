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
        public static readonly int BITMAPFILEHEADER_SIZE = 14;
        public static readonly int BITMAPINFOHEADER_SIZE = 40;
        public static readonly int BITMAPINFOHEADER_WIDTH = 4;
        public static readonly int BITMAPINFOHEADER_HEIGHT = 8;

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

        // Do we need this?
        public int BitsPerPixel { get { return 0; } }

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
        public ICOImage(string path) : this(System.Drawing.Image.FromFile(path))
        {
        }
        public ICOImage(byte[] imageData)
        {
            if(Bytes.FromBytes(imageData, 0, 2) == BITMAPINFOHEADER_SIZE)
            {
                //BMP image -> BITMAPFILEHEADER was stripped and must be generated
                imageData = GenerateBITMAPFILEHEADER(imageData);
            }
            MemoryStream stream = new MemoryStream(imageData);
            this.Image = new Bitmap(System.Drawing.Image.FromStream(stream));
            this.Type = ICOImageType.PNG;
            this.HotspotX = 0;
            this.HotspotY = 0;
        }
        public void Serialize()
        {
            MemoryStream stream = new MemoryStream();
            ImageFormat format = null;
            switch (Type)
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
            Image.Save(stream, format);
            serialized = stream.ToArray();
            // For BMP images, strip BITMAPFILEHEADER
            if (Type == ICOImageType.BMP)
            {
                serialized = Bytes.Subset(serialized, BITMAPFILEHEADER_SIZE, serialized.Length - BITMAPFILEHEADER_SIZE);
                // BUG? We need to double the image height in BITMAPINFOHEADER
                int height = Bytes.FromBytes(serialized, BITMAPINFOHEADER_HEIGHT, 4);
                height *= 2;
                byte[] heightBytes = Bytes.FromInt(height, 4);
                Bytes.Replace(serialized, heightBytes, BITMAPINFOHEADER_HEIGHT);
            }
        }
        private byte[] GenerateBITMAPFILEHEADER(byte[] imageData)
        {
            byte[] BMPImage = new byte[BITMAPFILEHEADER_SIZE + imageData.Length];
            //TODO
            return BMPImage;
        }
    }
}
