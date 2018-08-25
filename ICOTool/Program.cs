using ICO;
using System;

namespace ICOTool
{
    class Program
    {
        static void Main(string[] args)
        {
            DumpInfo("samples/rw.ico");
        }
        static void DumpInfo(string path)
        {
            ICOFile ico = ICOFile.ReadFromFile(path, false);
            foreach (ICOImage img in ico.Images)
                Console.WriteLine(string.Format("{0}: {1}x{2}", Enum.GetName(typeof(ICOImageType), img.Type), img.Width, img.Height));
        }
        static void CreateFromImage(string imagePath, string outputPath, ICOImageType imageType)
        {
            ICOFile ico = new ICOFile(ICOType.ICO);
            ico.Images.Add(ICOImage.ReadFromFile(imagePath));
            ico.Images[0].Type = imageType;
            ico.WriteToFile(outputPath);
        }
    }
}
