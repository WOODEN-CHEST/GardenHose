using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.IO.DataFile;

internal sealed class DataFileReaderV1 : DataFileReader
{
    // Fields.
    public override int FormatVersion => 1;


    // Constructors.
    internal DataFileReaderV1(BinaryReader reader, string filePath) : base(reader, filePath) { }


    // Methods.
    public override DataFileCompound Read()
    {
        try
        {
            return ReadCompound();
        }
        catch (Exception e) when (e is not DataReadException)
        {
            throw new DataReadException(FilePath, e.ToString());
        }
    }


    // Private methods
    private object ReadItem(int id)
    {
        byte ItemTypeByte = Reader.ReadByte();
        DataType ItemType = (DataType)(ItemTypeByte & ~(int)DataTypeFlag.Array);
        bool IsArray = (ItemTypeByte & (int)DataTypeFlag.Array) != 0;

        object Value = IsArray ? ReadArray(ItemType) : ReadSingleValue(ItemType);

        return Value;
    }

    private object ReadSingleValue(DataType itemType)
    {
        return itemType switch
        {
            DataType.Int8 => Reader!.ReadByte(),
            DataType.SInt8 => Reader!.ReadSByte(),
            DataType.Int16 => Reader!.ReadInt16(),
            DataType.UInt16 => Reader!.ReadUInt16(),
            DataType.Int32 => Reader!.ReadInt32(),
            DataType.UInt32 => Reader!.ReadUInt32(),
            DataType.Int64 => Reader!.ReadInt64(),
            DataType.UInt64 => Reader!.ReadUInt64(),

            DataType.Single => Reader!.ReadSingle(),
            DataType.Double => Reader!.ReadDouble(),

            DataType.Boolean => Reader!.ReadBoolean(),

            DataType.Char => Reader!.ReadChar(),
            DataType.String => Reader!.ReadString(),

            DataType.Compound => ReadCompound(),

            _ => throw new DataReadException($"Unknown or invalid data type found." +
            $"{Environment.NewLine}Enum name: \"{itemType}\". Enum byte value: {(byte)itemType}.")
        };
    }

    private object[] ReadArray(DataType typeOfValue)
    {
        int ArrayLength = Reader!.Read7BitEncodedInt();
        if (ArrayLength < 0)
        {
            throw new DataReadException($"Invalid data array length: {ArrayLength}");
        }

        object[] DataArray = new object[ArrayLength];

        for (int i = 0; i < ArrayLength; i++)
        {
            DataArray[i] = ReadSingleValue(typeOfValue);
        }

        return DataArray;
    }

    private DataFileCompound ReadCompound()
    {
        DataFileCompound Compound = new();

        int ID;
        while ((ID = Reader.Read7BitEncodedInt()) != IDataFile.TERMINATING_ID)
        {
            Compound.Add(ID, ReadItem(ID));
        }

        return Compound;
    }
}