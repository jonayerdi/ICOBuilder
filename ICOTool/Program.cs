using System;
using System.IO;
using System.Drawing.Imaging;

using ICO;

namespace ICOTool
{
    class Program
    {
        static void Main(string[] args)
        {
            //DumpInfo("samples/invader.ico");
            //CreateFromImage("samples/invader.png", "samples/invader.ico", ICOImageType.BMP);
            //ExportFromICO("samples/clock.ico", "samples/clock");
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
        static void ExportFromICO(string ICOPath, string outpath)
        {
            DirectoryInfo di = new DirectoryInfo(outpath);
            if (!di.Exists)
                di.Create();
            ICOFile ico = ICOFile.ReadFromFile(ICOPath, false);
            for (int i = 0; i < ico.Images.Count; i++)
            {
                ICOImage img = ico.Images[i];
                img.Image.Save(Path.Combine(di.FullName, string.Format("Image{0}.png", i)), ImageFormat.Png);
            }
        }
    }
}
