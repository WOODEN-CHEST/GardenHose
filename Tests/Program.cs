using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Process(new float[1_000_000_000]);
        }

        static void Process(float[] buffer)
        {
            for (int i = 0; i < buffer.Length - 1; i++)
            {

            }
        }
    }
}