
using GardenHoseEngine.IO.DataFile;
using NVorbis;
using System.Collections;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Test();
            Thread.Sleep(100000000);
        }

        static void Test()
        {
            DataFileCompound[] Compounds = new DataFileCompound[1_000_000];
            for (int i = 0; i < Compounds.Length; i++)
            {
                Compounds[i] = new DataFileCompound().Add(1, 57L);
            }
        }
    }
}