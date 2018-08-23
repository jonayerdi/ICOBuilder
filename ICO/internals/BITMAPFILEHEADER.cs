using System;

namespace ICO
{
    internal static class BITMAPFILEHEADER
    {
        public static readonly int BITMAPFILEHEADER_SIZE = 14;
        public static readonly int BITMAPINFOHEADER_SIZE = 40;
        public static readonly int BITMAPINFOHEADER_WIDTH = 4;
        public static readonly int BITMAPINFOHEADER_HEIGHT = 8;

        public static byte[] StripBITMAPFILEHEADER(byte[] BMPData)
        {
            byte[] result = Bytes.Subset(BMPData, BITMAPFILEHEADER_SIZE, BMPData.Length - BITMAPFILEHEADER_SIZE);
            // BUG? We need to double the image height in BITMAPINFOHEADER
            int height = Bytes.FromBytes(result, BITMAPINFOHEADER_HEIGHT, 4);
            height *= 2;
            byte[] heightBytes = Bytes.FromInt(height, 4);
            Bytes.Replace(result, heightBytes, BITMAPINFOHEADER_HEIGHT);
            return result;
        }
    }
}
