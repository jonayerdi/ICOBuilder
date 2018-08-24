using ICO;
using System;

namespace ICOTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 1)
            {
                ICOFile ico = ICOFile.ReadFromFile(args[0]);
                foreach (ICOImage img in ico.Images)
                    Console.WriteLine(string.Format("{0}: {1}x{2}", Enum.GetName(typeof(ICOImageType), img.Type), img.Width, img.Height));
            }
        }
    }
}
