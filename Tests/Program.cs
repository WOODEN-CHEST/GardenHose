
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
            Write();
            Read();
            Thread.Sleep(100000000);
        }

        static void Write()
        {
            DataFileCompound Compound = new();
            DataFileCompound CompoundToAdd = new DataFileCompound()
                    .Add(1, 1L);
        }
    }
}