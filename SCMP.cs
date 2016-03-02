using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApplication4
{
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
        HMHeader h;
        HMBlock[] blocks;
    }
    class HMBlock
    {
        public HMBlock(byte[] Input)
        {
            heightAdjustment = BitConverter.ToUInt16(Input, 0);
            heightBase = BitConverter.ToUInt16(Input, 2);
            mask = BitConverter.ToUInt16(Input, 4);
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
        }

        char[] magic;
        uint version;
        public uint sizeX;
        public uint sizeY;
        byte[] padding; 
    }

    class PathMap{

    }

    class SCMP
    {
        static void Main(string[] args)
        {
            byte[] hmapbarray = Properties.Resources.t3HeightMap2;
            byte[] header = new byte[32];
            Array.Copy(hmapbarray, header, 32);
            HeightMap f = new HeightMap(hmapbarray);
            f.print();
            f.generateBitMap("hmapOutput");
        }
    }
}
