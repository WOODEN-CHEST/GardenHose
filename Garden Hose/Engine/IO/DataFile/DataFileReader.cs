using GardenHose.Engine.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Engine.IO;

public sealed class DataFileReader : IDisposable
{
    // Fields.
    public int FileFormatVersion { get; private set; }
    public int FileGameVersion { get; private set; }
    public DateTime FileSaveTime { get; private set; }
    public string FilePath { get; private set; }


    // Static fields.
    public const int ReaderVersion = 1;


    // Private static fields.
    private static readonly byte[] s_signatureBytes =
    { 102, 37, 143, 181, 3, 205, 123, 185, 148, 157, 98, 177, 178, 151, 43, 170 };


    // Private fields.
    private BinaryReader _reader;
    private Action<DataFileReader> _onReadStart;


    // Constructors.
    private DataFileReader(string filePath)
    {
        FilePath = filePath;
        _reader = new(File.Open(filePath, FileMode.Open));
    }


    // Static methods.
    public static void ReadFile(string filePath, Action<DataFileReader> onReadStart)
    {
        try
        {
            using DataFileReader FileReader = new(filePath);
            FileReader._onReadStart = onReadStart;
            FileReader.Read();
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to read the file \"{filePath}\". Reason: {e}");
        }
    }


    // Private methods.
    private void Read()
    {
        if (!IsGHDFile())
        {
            throw new Exception("Provided file is not a GHD file.");
        }

        ReadMetaData();
        _onReadStart.Invoke(this);

        if (FileFormatVersion > ReaderVersion)
        {
            throw new Exception("Game file is saved in a newer format than is supported. " +
                $"File version: {FileFormatVersion}; Expected: {ReaderVersion}.\n" +
                "Will not attempt to read the file.");
        }
        else if (FileFormatVersion < ReaderVersion)
        {
            Logger.Warning($"The file \"{FilePath}\" is saved in an older format than expected. " +
                $"Format version: {FileFormatVersion}; Current reader version: {ReaderVersion}.\n" +
                $"Will attempt to read the file using the older format.");
            ReadFileOutdated();
            return;
        }

        while (_reader.PeekChar() != -1)
        {
            ReadItem();
        }
    }

    private bool IsGHDFile()
    {
        byte[] Signature = _reader.ReadBytes(s_signatureBytes.Length);
        return Signature.SequenceEqual(s_signatureBytes);
    }

    private void ReadMetaData()
    {
        FileFormatVersion = _reader.ReadInt32();
        FileGameVersion = _reader.ReadInt32();
        FileSaveTime = new(_reader.ReadInt64());
    }

    private void ReadItem()
    {
        int ID = _reader.ReadInt32();
    }

    private void ReadFileOutdated() { }


    // Inherited methods.
    public void Dispose()
    {
        _reader?.Dispose();
    }
}