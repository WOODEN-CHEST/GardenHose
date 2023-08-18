using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Engine.IO.DataFile;

public sealed class DataFileReader : IDisposable
{
    // Fields.
    public int FileFormatVersion { get; private set; }
    public int GameVersion { get; private set; }
    public string FilePath { get; private set; }


    // Private fields.
    private readonly BinaryReader _reader;
    private readonly Action<DataFileReader> _onReadStart;


    // Constructors.
    private DataFileReader(string filePath, Action<DataFileReader> onReadStart)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        if (!File.Exists(filePath))
        {
            filePath = Path.ChangeExtension(filePath, DataFileWriter.FileExtension);
        }

        FilePath = filePath;
        _reader = new(File.Open(FilePath, FileMode.Open));
        _onReadStart = onReadStart;
    }


    // Static methods.
    public static ReadDataCompound ReadFile(string filePath, Action<DataFileReader> onReadStart)
    {
        try
        {
            using DataFileReader FileReader = new(filePath, onReadStart);
            return FileReader.Read();
        }
        catch (IOException e)
        {
            throw new DataReadException($"IOException reading file \"{filePath}\". {e}");
        }
        catch (DataReadException e)
        {
            throw new DataReadException($"Corrupted or wrongly formatted file \"{filePath}\". {e}");
        }
        catch (UnauthorizedAccessException e)
        {
            throw new DataReadException($"Denied permission to write DataFile. {e}");
        }
    }


    // Private methods.
    private ReadDataCompound Read()
    {
        // Read metadata
        ReadMetaData();

        // Invoke callback on read start.
        _onReadStart?.Invoke(this);
        
        // Test version.
        if (FileFormatVersion > DataFileWriter.CurrentFormatVersion)
        {
            throw new DataReadException($"Game file is saved in a newer format than is supported.{Environment.NewLine}" +
                $"Format version: {FileFormatVersion}; Current reader version: {DataFileWriter.CurrentFormatVersion}.{Environment.NewLine}");
        }
        else if (FileFormatVersion < DataFileWriter.CurrentFormatVersion)
        {
            Console.WriteLine($"The file \"{FilePath}\" is saved in an older format than expected.{Environment.NewLine}" +
                $"Format version: {FileFormatVersion}; Current reader version: {DataFileWriter.CurrentFormatVersion}.{Environment.NewLine}" +
                $"Will attempt to read the file using the older format.");
            ReadFileOutdated();
        }

        // Read items.
        return ReadBase();
    }

    private void ReadFileOutdated()
    {
        throw new NotImplementedException("File reader for outdated versions data file is not yet implemented.");
    }


    /* Metadata */
    private bool IsGHDFile()
    {
        byte[] Signature = _reader.ReadBytes(DataFileWriter.s_signatureBytes.Length);
        return Signature.SequenceEqual(DataFileWriter.s_signatureBytes);
    }

    private void ReadMetaData()
    {
        if (!IsGHDFile())
        {
            throw new DataReadException("File is not a GHD file.");
        }

        FileFormatVersion = _reader.ReadInt32();
        GameVersion = _reader.ReadInt32();
    }


    /* Items. */
    private ReadDataCompound ReadBase()
    {
        ReadDataCompound Base = new(16384);

        try
        {
            while (_reader.BaseStream.Position < _reader.BaseStream.Length)
            {
                (int ID, dynamic Value) = ReadItem();
                Base.AddItem(ID, Value);
            }

            return Base;
        }
        catch (EndOfStreamException e)
        {
            throw new DataReadException($"Data stream unexpectedly ended. {e}");
        }
    }

    private (int ID, dynamic Value) ReadItem()
    {
        int ID = _reader.Read7BitEncodedInt();

        byte FullValueData = _reader.ReadByte();
        DataType TypeOfValue = (DataType)(FullValueData & ~(int)DataTypeFlag.Array);

        bool IsArray = (FullValueData & (int)DataTypeFlag.Array) != 0;

        dynamic Value = IsArray ? ReadArray(TypeOfValue) : ReadSingleValue(TypeOfValue);

        return (ID, Value);
    }

    private dynamic ReadSingleValue(DataType typeOfValue)
    {
        return typeOfValue switch
        {
            DataType.Int8 => _reader.ReadByte(),
            DataType.SInt8 => _reader.ReadSByte(),
            DataType.Int16 => _reader.ReadInt16(),
            DataType.UInt16 => _reader.ReadUInt16(),
            DataType.Int32 => _reader.ReadInt32(),
            DataType.UInt32 => _reader.ReadUInt32(),
            DataType.Int64 => _reader.ReadInt64(),
            DataType.UInt64 => _reader.ReadUInt64(),

            DataType.Single => _reader.ReadSingle(),
            DataType.Double => _reader.ReadDouble(),

            DataType.Boolean => _reader.ReadBoolean(),

            DataType.Char => _reader.ReadChar(),
            DataType.String => _reader.ReadString(),

            DataType.Compound => ReadCompound(),

            _ => throw new DataReadException($"Unknown or invalid data type found." +
            $"{Environment.NewLine}Enum name: \"{typeOfValue}\". Enum byte value: {(byte)typeOfValue}.")
        };
    }

    private dynamic[] ReadArray(DataType typeOfValue)
    {
        int ArrayLength = _reader.Read7BitEncodedInt();
        if (ArrayLength < 0)
        {
            throw new DataReadException($"Invalid data array length: {ArrayLength}");
        }

        dynamic[] DataArray = new dynamic[ArrayLength];

        for (int i = 0; i < ArrayLength; i++)
        {
            DataArray[i] = ReadSingleValue(typeOfValue);
        }

        return DataArray;
    }

    private ReadDataCompound ReadCompound()
    {
        int ItemCount = _reader.Read7BitEncodedInt();
        ReadDataCompound Compound = new(ItemCount);

        for (; ItemCount > 0; ItemCount--)
        {
            (int ID, object Value) = ReadItem();
            Compound.AddItem(ID, Value);
        }

        return Compound;
    }


    // Inherited methods.
    public void Dispose()
    {
        _reader?.Dispose();
    }
}