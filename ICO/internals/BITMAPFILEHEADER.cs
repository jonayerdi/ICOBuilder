﻿
namespace ICO
{
    internal static class BITMAPFILEHEADER
    {
        public static readonly int BITMAPFILEHEADER_SIGNATURE = 0x4D42;
        public static readonly int BITMAPFILEHEADER_SIZE = 14;
        public static readonly int BITMAPINFOHEADER_SIZE = 40;
        public static readonly int BITMAPINFOHEADER_WIDTH = 4;
        public static readonly int BITMAPINFOHEADER_HEIGHT = 8;
        public static readonly int BITMAPINFOHEADER_BITSPERPIXEL = 14;

        public static bool isStrippedBMP(byte[] imageData)
        {
            int sizeOfHeader = Bytes.FromBytes(imageData, 0, 4);
            if (sizeOfHeader == BITMAPINFOHEADER_SIZE)
            {
                return true;
            }
            return false;
        }

        public static byte[] StripBITMAPFILEHEADER(byte[] BMPData)
        {
            byte[] result = Bytes.Subset(BMPData, BITMAPFILEHEADER_SIZE, BMPData.Length - BITMAPFILEHEADER_SIZE);
            /*
             wikipedia.org/wiki/ICO_(file_format)

             Images with less than 32 bits of color depth follow a particular format: the image is encoded as 
             a single image consisting of a color mask (the "XOR mask") together with an opacity mask (the "AND mask").
             [6][not in citation given] The XOR mask must precede the AND mask inside the bitmap data; 
             if the image is stored in bottom-up order (which it most likely is), the XOR mask would be drawn below the AND mask. 
             The AND mask is 1 bit per pixel, regardless of the color depth specified by the BMP header, and specifies 
             which pixels are fully transparent and which are fully opaque. 
             The XOR mask conforms to the bit depth specified in the BMP header 
             and specifies the numerical color or palette value for each pixel.
             Together, the AND mask and XOR mask make for a non-transparent image representing an image with 1-bit transparency; 
             they also allow for inversion of the background. 

             The height for the image in the ICONDIRENTRY structure of the ICO/CUR file takes on that of the intended image dimensions
             (after the masks are composited), whereas the height in the BMP header takes on that of the two mask images combined 
             (before they are composited). Therefore, the masks must each be of the same dimensions, and the height 
             specified in the BMP header must be exactly twice the height specified in the ICONDIRENTRY structure.
             */
            int bpp = Bytes.FromBytes(result, BITMAPINFOHEADER_BITSPERPIXEL, 2);
            if(bpp < 32)
            {
                int height = Bytes.FromBytes(result, BITMAPINFOHEADER_HEIGHT, 4);
                height *= 2;
                byte[] heightBytes = Bytes.FromInt(height, 4);
                Bytes.Replace(result, heightBytes, BITMAPINFOHEADER_HEIGHT);
                // TODO: Generate AND mask inside the bitmap data
            }
            return result;
        }

        public static byte[] GenerateBITMAPFILEHEADER(ICONDIRENTRY icondirentry, byte[] BMPData)
        {
            byte[] result = new byte[BITMAPFILEHEADER_SIZE + BMPData.Length];
            ByteStream stream = new ByteStream(result);
            // Header field used to identify the BMP: "BM" in ASCII
            stream.Write16(BITMAPFILEHEADER_SIGNATURE);
            // The size of the BMP file in bytes
            stream.Write32(result.Length);
            // Reserved; actual value depends on the application that creates the image
            stream.Write16(0);
            // Reserved; actual value depends on the application that creates the image
            stream.Write16(0);
            // The offset of the byte where the bitmap image data (pixel array) can be found
            stream.Write32(BITMAPFILEHEADER_SIZE + BITMAPINFOHEADER_SIZE);
            // Copy BMPData
            Bytes.Replace(result, BMPData, BITMAPFILEHEADER_SIZE);
            // Fix BITMAPINFOHEADER height
            int bpp = Bytes.FromBytes(result, BITMAPFILEHEADER_SIZE + BITMAPINFOHEADER_BITSPERPIXEL, 2);
            if (bpp < 32)
            {
                int height = Bytes.FromBytes(result, BITMAPFILEHEADER_SIZE + BITMAPINFOHEADER_HEIGHT, 4);
                height /= 2;
                byte[] heightBytes = Bytes.FromInt(height, 4);
                Bytes.Replace(result, heightBytes, BITMAPFILEHEADER_SIZE + BITMAPINFOHEADER_HEIGHT);
                // TODO: Strip AND mask inside the bitmap data?
            }
            return result;
        }
    }
}
