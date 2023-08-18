using GardenHose.Engine.IO.DataFile;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] Data = File.ReadAllBytes(@"C:\Users\User\Desktop\haha.txt");
            ZipArchive Archive = new(File.Open(@"C:\Users\User\Desktop\haha.zip", FileMode.OpenOrCreate), ZipArchiveMode.Create);
            ZipArchiveEntry Entry = Archive.CreateEntry("haha.txt", CompressionLevel.SmallestSize);
            Stream fileStream = Entry.Open();
            fileStream.Write(Data);
            fileStream.Dispose();
            Archive.Dispose();
        }

        static void Write(DataFileWriter writer)
        {
            int Index = 1;
            int TargetIndex = 1;
            for (TargetIndex = Index + 1_000; Index <= TargetIndex; Index++)
            {
                writer.BaseCompound.WriteCompound(new WriteDataCompound(Index)

                    .WriteByte(1, byte.MaxValue)
                    .WriteSByte(2, sbyte.MaxValue)
                    .WriteShort(3, short.MaxValue)
                    .WriteUShort(4, ushort.MaxValue)
                    .WriteInt(5, int.MaxValue)
                    .WriteUInt(6, uint.MaxValue)
                    .WriteLong(7, long.MaxValue)
                    .WriteULong(8, ulong.MaxValue)
                    .WriteFloat(9, float.MaxValue)
                    .WriteDouble(10, double.MaxValue)
                    .WriteBool(11, false)
                    .WriteBool(12, true)
                    .WriteChar(13, 'h')
                    .WriteString(14, "Hello World!")


                    .WriteByteArray(15, Enumerable.Repeat(byte.MaxValue, 100).ToArray())
                    .WriteSByteArray(16, Enumerable.Repeat(sbyte.MaxValue, 100).ToArray())
                    .WriteShortArray(17, Enumerable.Repeat(short.MaxValue, 100).ToArray())
                    .WriteUShortArray(18, Enumerable.Repeat(ushort.MaxValue, 100).ToArray())
                    .WriteIntArray(19, Enumerable.Repeat(int.MaxValue, 100).ToArray())
                    .WriteUIntArray(20, Enumerable.Repeat(uint.MaxValue, 100).ToArray())
                    .WriteLongArray(21, Enumerable.Repeat(long.MaxValue, 100).ToArray())
                    .WriteULongArray(22, Enumerable.Repeat(ulong.MaxValue, 100).ToArray())
                    .WriteFloatArray(23, Enumerable.Repeat(float.MaxValue, 100).ToArray())
                    .WriteDoubleArray(24, Enumerable.Repeat(double.MaxValue, 100).ToArray())
                    .WriteBoolArray(25, Enumerable.Repeat(false, 100).ToArray())
                    .WriteCompoundArray(27, Enumerable.Repeat(
                        new WriteDataCompound().WriteDouble(1, 98765.4321d).WriteChar(2, 'a'), 10_000).ToArray())
                    .WriteStringArray(26, Enumerable.Repeat("String Array", 100).ToArray()));


            }

            writer.WriteToFile(34);
        }
    }
}