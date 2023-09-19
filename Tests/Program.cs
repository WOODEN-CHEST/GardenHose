
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
        }

        static void Test()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                throw new ArgumentException();
            }
            finally
            {
                Console.WriteLine("Finally");
            }
        }
    }
}