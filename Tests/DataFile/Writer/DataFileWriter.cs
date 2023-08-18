using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Engine.IO.DataFile;

public class DataFileWriter
{
    // Static fields.
    public const int CurrentFormatVersion = 1;

    public const string FileExtension = ".ghdf";

    public static readonly byte[] s_signatureBytes =
    { 102, 37, 143, 181, 3, 205, 123, 185, 148, 157, 98, 177, 178, 151, 43, 170 };


    // Fields.
    public string FilePath { get; private set; }

    public readonly WriteDataCompound BaseCompound = new(null, 16384);


    // Constructors.
    public DataFileWriter(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        if (Path.GetExtension(filePath) != FileExtension)
        {
            filePath = Path.ChangeExtension(filePath, FileExtension);
        }
        
        FilePath = filePath;
    }


    // Methods.
    public void WriteToFile(int gameVersion)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            using Stream DataStream = File.Create(FilePath);

            WriteMetadata(DataStream, gameVersion);
            DataStream.Write(BaseCompound.GetData());
            DataStream.Flush();
            DataStream.Close();
        }
        catch (IOException e)
        {
            throw new DataWriteException($"IOException writing DataFile. {e}");
        }
        catch (UnauthorizedAccessException e)
        {
            throw new DataWriteException($"Denied permission to write file. {e}");
        }
    }


    // Private methods.
    private void WriteMetadata(Stream dataStream, int gameVersion)
    {
        dataStream.Write(s_signatureBytes);
        dataStream.Write(BitConverter.GetBytes(CurrentFormatVersion));
        dataStream.Write(BitConverter.GetBytes(gameVersion));
    }
}