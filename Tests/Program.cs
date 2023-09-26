
using NVorbis;
using System.Collections;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace Tests
{
    internal class Program
    {
        static double PassedTime = 0d;
        static AutoResetEvent Event = new(false);

        static DateTime InvokeTime;

        static void Main(string[] args)
        {
            float a = MathF.Atan2(4, 1) * (360f / (2*MathF.PI));
        }


        static void Execute()
        {

        }
    }
}