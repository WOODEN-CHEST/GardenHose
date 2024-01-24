using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO.DataFile;

public abstract class DataFileReader : IDataFile, IDisposable
{
    // Fields.
    public abstract int FormatVersion { get; }

    public const int LatestVersion = 1;


    // Protected fields.
    protected string FilePath { get; init; }

    protected BinaryReader Reader { get; init; }


    // Constructors.
    protected DataFileReader(BinaryReader reader, string filePath)
    {
        Reader = reader ?? throw new ArgumentNullException(nameof(reader));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }


    // Static methods.
    public static DataFileReader GetReader(string filePath)
    {
        int Version;

        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File does not exist.", filePath);
        }

        BinaryReader Reader = null!;
        try
        {
            Reader = new(File.Open(filePath, FileMode.Open));
            Version = ReadMetaData(Reader, filePath);
        }
        catch (Exception e) when (e is not DataReadException)
        {
            Reader?.Dispose();
            throw new DataReadException(filePath, e.ToString());
        }

        return Version switch
        {
            1 => new DataFileReaderV1(Reader, filePath),

            _ => throw new ArgumentOutOfRangeException(nameof(Version),
                $"The version {Version} of {nameof(DataFileReader)} is not supported."),
        };
    }


    // Methods.
    public abstract DataFileCompound Read();


    // Private static methods.
    private static int ReadMetaData(BinaryReader reader, string filePath)
    {
        byte[] Signature = new byte[IDataFile.Signature.Length];

        if (reader.Read(Signature, 0, IDataFile.Signature.Length) < IDataFile.Signature.Length)
        {
            throw new DataReadException(filePath!, "Not a KGDF file, signature too short.");
        }

        if (!Signature.SequenceEqual(IDataFile.Signature))
        {
            throw new DataReadException(filePath!, "Not a KGDF file, signature is incorrect.");
        }

        byte[] VersionBytes = new byte[sizeof(int)];
        if (reader.Read(VersionBytes, 0, sizeof(int)) < VersionBytes.Length)
        {
            throw new DataReadException(filePath!, "Corrupted KGDF file, incomplete version information.");
        }

        return BitConverter.ToInt32(VersionBytes);
    }


    // Inherited methods.
    public void Dispose()
    {
        Reader.Dispose();
    }
}