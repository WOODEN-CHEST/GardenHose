using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.IO.DataFile;

public sealed class DataFileReaderVersion1 : DataFileReader
{
    // Fields.
    public override int FormatVersion => 1;


    // Private fields.
    private BinaryReader? _reader;


    // Constructors.
    internal DataFileReaderVersion1() { }


    // Methods.
    public override DataCompound? Read(string filePath, Func<int, bool> versionReadCallback)
    {
        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }
        if (!File.Exists(filePath))
        {
            throw new ArgumentException("File not found.", nameof(filePath));
        }

        try
        {
            _reader = new(File.OpenRead(filePath));
            using (_reader)
            {
                if (ReadMetaData(_reader, filePath, versionReadCallback))
                {
                    return null;
                }
                return ReadBase();
            }
        }
        catch (IOException e)
        {
            throw new DataReadException(filePath, $"IOException reading file. {e}");
        }
        catch (DataReadException e)
        {
            throw new DataReadException(filePath, $"Corrupted or wrongly formatted file. {e}");
        }
        catch (UnauthorizedAccessException e)
        {
            throw new DataReadException(filePath, $"Denied permission to read DataFile. {e}");
        }
    }


    // Private methods
    private DataCompound ReadBase()
    {
        DataCompound Base = new();

        try
        {
            while (_reader!.BaseStream.Position < _reader.BaseStream.Length)
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
        int ID = _reader!.Read7BitEncodedInt();

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
            DataType.Int8 => _reader!.ReadByte(),
            DataType.SInt8 => _reader!.ReadSByte(),
            DataType.Int16 => _reader!.ReadInt16(),
            DataType.UInt16 => _reader!.ReadUInt16(),
            DataType.Int32 => _reader!.ReadInt32(),
            DataType.UInt32 => _reader!.ReadUInt32(),
            DataType.Int64 => _reader!.ReadInt64(),
            DataType.UInt64 => _reader!.ReadUInt64(),

            DataType.Single => _reader!.ReadSingle(),
            DataType.Double => _reader!.ReadDouble(),

            DataType.Boolean => _reader!.ReadBoolean(),

            DataType.Char => _reader!.ReadChar(),
            DataType.String => _reader!.ReadString(),

            DataType.Compound => ReadCompound(),

            _ => throw new DataReadException($"Unknown or invalid data type found." +
            $"{Environment.NewLine}Enum name: \"{typeOfValue}\". Enum byte value: {(byte)typeOfValue}.")
        };
    }

    private dynamic[] ReadArray(DataType typeOfValue)
    {
        int ArrayLength = _reader!.Read7BitEncodedInt();
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

    private DataCompound ReadCompound()
    {
        int ItemCount = _reader!.Read7BitEncodedInt();
         DataCompound Compound = new(ItemCount);

        for (; ItemCount > 0; ItemCount--)
        {
            (int ID, object Value) = ReadItem();
            Compound.AddItem(ID, Value);
        }

        return Compound;
    }
}