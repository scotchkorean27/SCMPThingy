using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;

namespace ConsoleApplication4
{
    class HPMap
    {
        public HPMap(HeightMap height, PathMap path, RSet r){
            h = height;
            p = path;
            sX = p.h.sizeX;
            if (h.h.sizeX > p.h.sizeX)
            {
                sX = h.h.sizeX;
            }
            sY = p.h.sizeY;
            if (h.h.sizeY > p.h.sizeY)
            {
                sY = h.h.sizeY;
            }
            blocks = new HPBlock[sX * sY];
            int hi = 0;
            int pi = 0;
            for (int i = 0; i < sX * sY; i++)
            {
                HMBlock hb = new HMBlock();
                PMBlock pb = new PMBlock();
                if (i % sX < h.h.sizeX && i / sX < h.h.sizeY)
                {
                    hb = h.blocks[hi];
                    hi++;
                }
                if (i % sX < p.h.sizeX && i / sX < p.h.sizeY)
                {
                    pb = p.blocks[pi];
                    pi++;
                }
                blocks[i] = new HPBlock(hb, pb);
                resources = r;
            }
        }
        public void generateBitMap(string filename)
        {
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap((int)sX, (int)sY);
            int i = 0;
            for (int y = 0; y < sY; y++)
            {
                for (int x = 0; x < sX; x++)
                {
                    
                    bm.SetPixel((int)sX - 1 - x, y, blocks[i].getColor());
                    i++;
                }
            }
            Console.WriteLine();

            foreach (Resource r in resources.list())
            {
                Color c = Color.Black;
                if (r.type == 1)
                {
                    c = Color.Blue;
                }
                else if (r.type == 2)
                {
                    c = Color.Green;
                }
                else
                {
                    c = Color.Yellow;
                }
                bm.SetPixel((int) sX - 1 - r.x, r.y, c);
            }

            bm.Save(filename + ".bmp", ImageFormat.Bmp);
        }
        uint sX;
        uint sY;
        HeightMap h;
        PathMap p;
        RSet resources;
        HPBlock[] blocks;
    }
    class HPBlock
    {
        public HPBlock(HMBlock hb, PMBlock pb)
        {
            h = hb;
            p = pb;
        }
        public Color getColor(){
            int r;
            int g;
            int b;

            r = (int)h.mask * 50;
            g = r;
            b = g;

            if (p.bit1)
            {
                r = 255;
            }
            return Color.FromArgb(r, g, b);
        }
        public HMBlock h;
        public PMBlock p;
    }


    class HeightMap
    {
        public HeightMap(byte[] input)
        {
            byte[] temp32 = new byte[32];
            Array.Copy(input, temp32, 32);
            h = new HMHeader(temp32);
            h.print();
            input = input.Skip(32).ToArray();
            blocks = new HMBlock[h.sizeX * h.sizeY]; 
            for(int i = 0; i < blocks.Length; i++){
                byte[] temp48 = new byte[6];
                Array.Copy(input, 6 * i, temp48, 0, 6);
                blocks[i] = new HMBlock(temp48);
            }

        }
        public void print(){
            for (int i = 0; i < blocks.Length; i++)
            {
                //Console.WriteLine("Block " + i + ": ");
                //if(blocks[i].mask == 2) blocks[i].print();
            }
        }
        public void generateBitMap(string filename)
        {
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap((int)h.sizeX, (int)h.sizeY);
            int i = 0;
            for (int y = 0; y < h.sizeY; y++)
            {
                for (int x = 0; x < h.sizeX; x++)
                {
                    int mmult = 50 * (int) blocks[i].mask;
                    Color p = Color.FromArgb(mmult, mmult, mmult);
                    bm.SetPixel((int)h.sizeX - 1 - x, y, p);
                    i++;
                }
            }
            bm.Save(filename + ".bmp", ImageFormat.Bmp);
        }
        public void generateH2BitMap(string filename)
        {
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap((int)h.sizeX, (int)h.sizeY);
            
            int max = 0;
            int min = 100000;
            for (int j = 0; j < blocks.Length; j++)
            {
                if (blocks[j].heightAdjustment > max)
                {
                    max = (int) blocks[j].heightAdjustment;
                }
                if (blocks[j].heightAdjustment < min)
                {
                    min = (int)blocks[j].heightAdjustment;
                }
            }
            int i = 0;
            for (int y = 0; y < h.sizeY; y++)
            {
                for (int x = 0; x < h.sizeX; x++)
                {
                    float norm = (blocks[i].heightAdjustment - min) / (max - min);
                    int mmult =(int)( 255 * norm);
                    Color p = Color.FromArgb(mmult, mmult, mmult);
                    bm.SetPixel((int)h.sizeX - 1 - x, y, p);
                    i++;
                }
            }
            bm.Save(filename + ".bmp", ImageFormat.Bmp);
        }
        public void generateH3BitMap(string filename)
        {
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap((int)h.sizeX, (int)h.sizeY);

            int max = 0;
            int min = 100000;
            for (int j = 0; j < blocks.Length; j++)
            {
                if (blocks[j].heightBase > max)
                {
                    max = (int)blocks[j].heightBase;
                }
                if (blocks[j].heightBase < min)
                {
                    min = (int)blocks[j].heightBase;
                }
            }
            int i = 0;
            for (int y = 0; y < h.sizeY; y++)
            {
                for (int x = 0; x < h.sizeX; x++)
                {
                    float norm = (blocks[i].heightBase - min) / (max - min);
                    int mmult = (int)(255 * norm);
                    Color p = Color.FromArgb(mmult, mmult, mmult);
                    bm.SetPixel((int)h.sizeX - 1 - x, y, p);
                    i++;
                }
            }
            bm.Save(filename + ".bmp", ImageFormat.Bmp);
        }
        public HMHeader h;
        public HMBlock[] blocks;
    }
    class HMBlock
    {
        public HMBlock(byte[] Input)
        {
            heightAdjustment = BitConverter.ToUInt16(Input, 0);
            heightBase = BitConverter.ToUInt16(Input, 2);
            mask = BitConverter.ToUInt16(Input, 4);
        }
        public HMBlock()
        {
            heightAdjustment = 0;
            heightBase = 0;
            mask = 0;
        }
        public void print()
        {
            Console.WriteLine("heightAdjustment: " + heightAdjustment);
            Console.WriteLine("heightBase: " + heightBase);
            Console.WriteLine("mask: " + mask);
        }
        public uint heightAdjustment;
        public uint heightBase;
        public uint mask;
    }
    class HMHeader
    {
        public HMHeader(byte[] Input){
            magic = new char[4];
            for (int i = 0; i < 4; i++)
            {
                magic[i] = (char) Input[i];
            }

            version = BitConverter.ToUInt32(Input, 4);
            sizeX = BitConverter.ToUInt32(Input, 8);
            sizeY = BitConverter.ToUInt32(Input, 12);
            padding = new byte[16];
            for (int i = 16; i < 32; i++)
            {
                padding[i - 16] = Input[i];
            }      
        }
        public void print()
        {
            for (int i = 0; i < magic.Length; i++)
            {
                Console.Write(magic[i]);
            }
            Console.WriteLine();
            Console.WriteLine("Version: " + version);
            Console.WriteLine("SizeX: " + sizeX);
            Console.WriteLine("SizeY: " + sizeY);
            for (int i = 0; i < 16; i++)
            {
                Console.Write(padding[i]);
            }
            Console.WriteLine();
        }

        char[] magic;
        uint version;
        public uint sizeX;
        public uint sizeY;
        byte[] padding; 
    }

    class PathMap{
        public PathMap(byte[] input)
        {
            h = new PMHeader(input);
            input = input.Skip(32).ToArray();
            blocks = new PMBlock[h.sizeX * h.sizeY];
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = new PMBlock(input[i]);
            }
        }
        public void GenerateBitMap(string filename)
        {
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap((int)h.sizeX, (int)h.sizeY);
            int i = 0;
            for (int y = 0; y < h.sizeY; y++)
            {
                for (int x = 0; x < h.sizeX; x++)
                {
                    int g = 0;
                    int b = 0;

                    if (blocks[i].bit1)
                    {
                        g = 255;
                    }
                    if (blocks[i].bit2)
                    {
                        b = 255;
                    }
                    bm.SetPixel((int)h.sizeX - 1 - x, y, Color.FromArgb(0, g, b));
                    i++;
                }
            }
            bm.Save(filename + ".bmp", ImageFormat.Bmp);
        }
        public PMHeader h;
        public PMBlock[] blocks;
    }
    class PMBlock
    {
        public PMBlock(byte input)
        {
            bit1 = (input & 0x00000001) > 0;
            bit2 = (input & 0x00000010) > 0;
        }
        public PMBlock()
        {
            bit1 = false;
            bit2 = false;
        }
        public Boolean bit1;
        public Boolean bit2;
    }
    class PMHeader
    {
        public PMHeader(byte[] input)
        {
            magic = new char[4];
            for (int i = 0; i < 4; i++)
            {
                magic[i] = (char)input[i];
            }
            version = BitConverter.ToUInt32(input, 4);
            padding = new byte[16];
            for (int i = 8; i < 24; i++)
            {
                padding[i - 8] = input[i];
            }
            sizeX = BitConverter.ToUInt32(input, 24);
            sizeY = BitConverter.ToUInt32(input, 28);
        }
        public void print()
        {
            for (int i = 0; i < magic.Length; i++)
            {
                Console.Write(magic[i]);
            }
            Console.WriteLine();
            Console.WriteLine("Version: " + version);
            Console.WriteLine("SizeX: " + sizeX);
            Console.WriteLine("SizeY: " + sizeY);
            for (int i = 0; i < 16; i++)
            {
                Console.Write(padding[i]);
            }
        }
        char[] magic;
        uint version;
        public uint sizeX;
        public uint sizeY;
        byte[] padding;
    }

    class RSet
    {
        public RSet(XmlDocument objsheet)
        {
            objs = new List<Resource>();
            XmlNodeList irlist = objsheet.GetElementsByTagName("ObjectUnit");
            foreach (XmlNode node in irlist)
            {
                string type = (node.Attributes["UnitType"].Value);
                if (type.Contains("MineralField") || type.Contains("VespeneGeyser"))
                {
                    objs.Add(new Resource(node, true));
                }
            }
            
            XmlNodeList blist = objsheet.GetElementsByTagName("ObjectPoint");
            foreach (XmlNode node in blist)
            {
                string type = node.Attributes["Type"].Value;
                if (type.Contains("StartLoc"))
                {
                    objs.Add(new Resource(node, false));
                }
            }

        }
        public List<Resource> list()
        {
            return objs;
        }
        List<Resource> objs;
    }
    class Resource
    {
        public Resource(XmlNode input, Boolean uorp)
        {
            string pos = input.Attributes["Position"].Value;
            string[] coords = pos.Split(',');
            x = (int) float.Parse(coords[0]);
            y = (int) float.Parse(coords[1]);
            if (uorp)
            {
                
                string rtype = input.Attributes["UnitType"].Value;
                if (rtype.Contains("MineralField"))
                {
                    type = 1;
                    Console.Write("M");
                }
                else if (rtype.Contains("VespeneGeyser"))
                {
                    type = 2;
                    Console.Write("R");
                }
            }
            else
            {
                type = 3;
                Console.Write("S");
            }
        }
        public int type;
        public int x;
        public int y;
    }

    class SCMP
    {
        static void Main(string[] args)
        {
            string folder = "NAntioch.SC2Map";
            byte[] hmapbarray = System.IO.File.ReadAllBytes(folder + "/t3HeightMap");
            byte[] header = new byte[32];
            Array.Copy(hmapbarray, header, 32);
            HeightMap f = new HeightMap(hmapbarray);
            f.print();
            f.generateBitMap("hmapOutput");

            byte[] path = System.IO.File.ReadAllBytes(folder + "/t3CellFlags");
            PathMap p = new PathMap(path);
            p.h.print();
            p.GenerateBitMap("testpathmap");

            

            XmlDocument objsheet = new XmlDocument();
            objsheet.Load(folder + "/objects");
            RSet resources = new RSet(objsheet);


            HPMap hpm = new HPMap(f, p, resources);
            hpm.generateBitMap("OverlayTest");

            f.generateH3BitMap("HBTestMap");
        }
    }
}
