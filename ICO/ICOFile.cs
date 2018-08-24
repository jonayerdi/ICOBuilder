using System;
using System.Collections.Generic;
using System.IO;

namespace ICO
{
    public enum ICOType
    {
        ICO,
        CUR
    }
    public class ICOFile
    {
        public ICOType Type { get; set; }
        public List<ICOImage> Images { get; set; }

        public ICOFile(ICOType type)
        {
            this.Images = new List<ICOImage>();
            this.Type = type;
        }

        public ICOFile(byte[] data, bool ignoreErrors)
        {
            this.Images = new List<ICOImage>();
            this.Deserialize(data, ignoreErrors);
        }
        public ICOFile(byte[] data) : this(data, true) { }
        public static ICOFile ReadFromFile(string path, bool ignoreErrors)
        {
            return new ICOFile(File.ReadAllBytes(path), ignoreErrors);
        }
        public static ICOFile ReadFromFile(string path) { return ReadFromFile(path, true); }

        public void WriteToFile(string path)
        {
            File.WriteAllBytes(path, this.Serialize());
        }

        private int OffsetOfImage(ICOImage image)
        {
            int headersSize = ICONDIR.SIZE + (ICONDIRENTRY.SIZE * Images.Count);
            int imagesOffset = 0;
            for(int i = 0; i < Images.Count; i++)
            {
                if (Images[i] == image)
                    break;
                imagesOffset += Images[i].Serialized.Length;
            }
            return headersSize + imagesOffset;
        }

        private byte[] SerializeHeaders()
        {
            ByteStream stream = new ByteStream(ICONDIR.SIZE + (ICONDIRENTRY.SIZE * Images.Count));
            stream.Write(ICONDIR.Serialize(this.Type, this.Images.Count));
            foreach(ICOImage image in Images)
                stream.Write(ICONDIRENTRY.Serialize(this.Type, image, this.OffsetOfImage(image)));
            return stream.Buffer;
        }

        public byte[] Serialize()
        {
            List<byte> ico = new List<byte>();
            foreach (ICOImage image in Images)
                image.Serialize();
            ico.AddRange(SerializeHeaders());
            foreach (ICOImage image in Images)
                ico.AddRange(image.Serialized);
            return ico.ToArray();
        }

        private void Deserialize(byte[] data, bool ignoreErrors)
        {
            // Deserialize headers
            ByteStream stream = new ByteStream(data);
            ICONDIR icondir = new ICONDIR(stream);
            ICONDIRENTRY[] icondirentries = new ICONDIRENTRY[icondir.ImageCount];
            for (int i = 0; i < icondirentries.Length; i++)
                icondirentries[i] = new ICONDIRENTRY(icondir.Type, stream);
            this.Type = icondir.Type;
            // Deserialize images
            foreach (ICONDIRENTRY icondirentry in icondirentries)
            {
                try
                {
                    this.Images.Add(new ICOImage(this.Type, icondirentry, data));
                } catch(Exception e)
                {
                    if (!ignoreErrors) throw e;
                }
            }
        }
    }
}
