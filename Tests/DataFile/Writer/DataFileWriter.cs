using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO.DataFile;

public abstract class DataFileWriter : IDataFile
{
    // Fields.
    // Fields.
    public const int LatestVersion = 1;

    public abstract int FormatVersion { get; }


    // Constructors.
    public DataFileWriter() { }


    // Static methods.
    public static DataFileWriter GetWriter(int version = LatestVersion)
    {
        return version switch
        {
            1 => new DataFileWriterV1(),

            _ => throw new ArgumentOutOfRangeException(nameof(version),
                $"The version {version} of {nameof(DataFileWriter)} is not supported."),
        };
    }


    // Methods.
    public void Write(string path, DataFileCompound baseCompound)
    {
        if (baseCompound == null)
        {
            throw new ArgumentNullException(nameof(baseCompound));
        }
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (!Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("Path isn't fully qualified.", nameof(path));
        }


        try
        {
            string TempFilePath = path + " TEMP";
            path = Path.ChangeExtension(path, IDataFile.FileExtension);

            using BinaryWriter Writer = new(File.Open(TempFilePath, FileMode.OpenOrCreate));
            WriteMetaData(Writer);
            WriteData(Writer, baseCompound);

            File.Delete(path);
            Writer.Dispose();
            File.Move(TempFilePath, path);
        }
        catch (Exception e)
        {
            throw new DataWriteException(path, e.ToString());
        }
        
    }


    // Protected methods.
    protected abstract void WriteData(BinaryWriter writer, DataFileCompound baseCompound);


    // Internal methods.
    internal void WriteMetaData(BinaryWriter writer)
    {
        if (writer == null)
        {
            throw new InvalidOperationException("Stream writer is not set.");
        }

        writer.Write(IDataFile.Signature);
        writer.Write(FormatVersion);
    }
}