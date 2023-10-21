using System.Runtime.InteropServices;



namespace GardenHoseEngine.IO.DataFile;

/* Hell is a place on earth. */
internal sealed class DataFileWriterV1 : DataFileWriter
{
    // Public fields.
    public override int FormatVersion { get; } = 1;


    // Constructors.
    internal DataFileWriterV1() { }


    // Protected methods.
    protected override void WriteData(BinaryWriter writer, DataFileCompound dataCompound)  
    {
        foreach (KeyValuePair<int, object> Item in dataCompound)
        {
            WriteItem(writer, Item.Key, Item.Value);
        }

        writer.Write7BitEncodedInt(IDataFile.TERMINATING_ID);
    }


    // Private methods.
    private void WriteItem(BinaryWriter writer, int id, object item)
    {
        writer.Write7BitEncodedInt(id);

        if (item is Array)
        {
            WriteItemArray(writer, id, item);
        }
        else
        {
            WriteSingleItem(writer, id, item);
        }
        
    }

    private void WriteSingleItem(BinaryWriter writer, int id, object item)
    {
        if (item is byte)
        {
            WriteInt8(writer, (byte)item);
        }
        else if (item is sbyte)
        {
            WriteSInt8(writer, (sbyte)item);
        }
        else if (item is short)
        {
            WriteInt16(writer, (short)item);
        }
        else if (item is ushort)
        {
            WriteUInt16(writer, (ushort)item);
        }
        else if (item is int)
        {
            WriteInt32(writer, (int)item);
        }
        else if (item is uint)
        {
            WriteUInt32(writer, (uint)item);
        }
        else if (item is long)
        {
            WriteInt64(writer, (long)item);
        }
        else if (item is ulong)
        {
            WriteUInt64(writer, (ulong)item);
        }
        else if (item is float)
        {
            WriteFloat(writer, (float)item);
        }
        else if (item is double)
        {
            WriteDouble(writer, (double)item);
        }
        else if (item is bool)
        {
            WriteBool(writer, (bool)item);
        }
        else if (item is char)
        {
            WriteChar(writer, (char)item);
        }
        else if (item is string)
        {
            WriteString(writer, (string)item);
        }
        else if (item is DataFileCompound)
        {
            WriteCompound(writer, (DataFileCompound)item);
        }
        else
        {
            throw new ArgumentException(
                $"Unknown type for object of id {id}: \"{item.GetType()}\"",
                nameof(item));
        }
    }

    private void WriteItemArray(BinaryWriter writer, int id, object array)
    {
        if (array is byte[])
        {
            WriteInt8Array(writer, (byte[])array);
        }
        else if (array is sbyte[])
        {
            WriteSInt8Array(writer, (sbyte[])array);
        }
        else if (array is short[])
        {
            WriteInt16Array(writer, (short[])array);
        }
        else if (array is ushort[])
        {
            WriteUInt16Array(writer, (ushort[])array);
        }
        else if (array is int[])
        {
            WriteInt32Array(writer, (int[])array);
        }
        else if (array is uint[])
        {
            WriteUInt32Array(writer, (uint[])array);
        }
        else if (array is long[])
        {
            WriteInt64Array(writer, (long[])array);
        }
        else if (array is ulong[])
        {
            WriteUInt64Array(writer, (ulong[])array);
        }
        else if (array is float[])
        {
            WriteFloatArray(writer, (float[])array);
        }
        else if (array is double[])
        {
            WriteDoubleArray(writer, (double[])array);
        }
        else if (array is bool[])
        {
            WriteBoolArray(writer, (bool[])array);
        }
        else if (array is char[])
        {
            WriteCharArray(writer, (char[])array);
        }
        else if (array is string[])
        {
            WriteStringArray(writer, (string[])array);
        }
        else if (array is DataFileCompound[])
        {
            WriteCompoundArray(writer, (DataFileCompound[])array);
        }
        else
        {
            throw new ArgumentException(
                $"Unknown type for object of id {id}: \"{array.GetType()}\"",
                nameof(array));
        }
    }

    private void WriteInt8(BinaryWriter writer, byte value)
    {
        writer.Write((byte)DataType.Int8);
        writer.Write(value);
    }

    private void WriteSInt8(BinaryWriter writer, sbyte value)
    {
        writer.Write((byte)DataType.SInt8);
        writer.Write(value);
    }

    private void WriteInt16(BinaryWriter writer, short value)
    {
        writer.Write((byte)DataType.Int16);
        writer.Write(value);
    }

    private void WriteUInt16(BinaryWriter writer, ushort value)
    {
        writer.Write((byte)DataType.UInt16);
        writer.Write(value);
    }

    private void WriteInt32(BinaryWriter writer, int value)
    {
        writer.Write((byte)DataType.Int32);
        writer.Write(value);
    }

    private void WriteUInt32(BinaryWriter writer, uint value)
    {
        writer.Write((byte)DataType.UInt32);
        writer.Write(value);
    }

    private void WriteInt64(BinaryWriter writer, long value)
    {
        writer.Write((byte)DataType.Int64);
        writer.Write(value);
    }

    private void WriteUInt64(BinaryWriter writer, ulong value)
    {
        writer.Write((byte)DataType.UInt64);
        writer.Write(value);
    }

    private void WriteFloat(BinaryWriter writer, float value)
    {
        writer.Write((byte)DataType.Single);
        writer.Write(value);
    }

    private void WriteDouble(BinaryWriter writer, double value)
    {
        writer.Write((byte)DataType.Double);
        writer.Write(value);
    }

    private void WriteBool(BinaryWriter writer, bool value)
    {
        writer.Write((byte)DataType.Boolean);
        writer.Write(value);
    }

    private void WriteChar(BinaryWriter writer, char value)
    {
        writer.Write((byte)DataType.Char);
        writer.Write(value);
    }

    private void WriteString(BinaryWriter writer, string value)
    {
        writer.Write((byte)DataType.String);
        writer.Write(value);
    }

    private void WriteCompound(BinaryWriter writer, DataFileCompound dataCompound)
    {
        writer.Write((byte)DataType.Compound);

        foreach (KeyValuePair<int, object> Item in dataCompound)
        {
            WriteItem(writer, Item.Key, Item.Value);
        }

        writer.Write7BitEncodedInt(IDataFile.TERMINATING_ID);
    }

    private void WriteInt8Array(BinaryWriter writer, byte[] array)
    {
        writer.Write((byte)((int)DataType.Int8 | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<byte>(array));
    }

    private void WriteSInt8Array(BinaryWriter writer, sbyte[] array)
    {
        writer.Write((byte)((int)DataType.SInt8 | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<sbyte>(array));
    }

    private void WriteInt16Array(BinaryWriter writer, short[] array)
    {
        writer.Write((byte)((int)DataType.Int16 | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<short>(array));
    }

    private void WriteUInt16Array(BinaryWriter writer, ushort[] array)
    {
        writer.Write((byte)((int)DataType.UInt16 | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<ushort>(array));
    }

    private void WriteInt32Array(BinaryWriter writer, int[] array)
    {
        writer.Write((byte)((int)DataType.Int32 | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<int>(array));
    }

    private void WriteUInt32Array(BinaryWriter writer, uint[] array)
    {
        writer.Write((byte)((int)DataType.UInt32 | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<uint>(array));
    }

    private void WriteInt64Array(BinaryWriter writer, long[] array)
    {
        writer.Write((byte)((int)DataType.Int64 | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes <long>(array));
    }

    private void WriteUInt64Array(BinaryWriter writer, ulong[] array)
    {
        writer.Write((byte)((int)DataType.UInt64 | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<ulong>(array));
    }

    private void WriteFloatArray(BinaryWriter writer, float[] array)
    {
        writer.Write((byte)((int)DataType.Single | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<float>(array));
    }

    private void WriteDoubleArray(BinaryWriter writer, double[] array)
    {
        writer.Write((byte)((int)DataType.Double | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<double>(array));
    }

    private void WriteBoolArray(BinaryWriter writer, bool[] array)
    {
        writer.Write((byte)((int)DataType.Boolean | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<bool>(array));
    }

    private void WriteCharArray(BinaryWriter writer, char[] array)
    {
        writer.Write((byte)((int)DataType.Char | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);
        writer.Write(MemoryMarshal.AsBytes<char>(array));
    }

    private void WriteStringArray(BinaryWriter writer, string[] array)
    {
        writer.Write((byte)((int)DataType.String | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);

        foreach (var String in array)
        {
            writer.Write(String);
        }
    }

    private void WriteCompoundArray(BinaryWriter writer, DataFileCompound[] array)
    {
        writer.Write((byte)((int)DataType.Compound | (int)DataTypeFlag.Array));
        writer.Write7BitEncodedInt(array.Length);

        foreach (var Compound in array)
        {
            WriteCompound(writer, Compound);
        }
    }
}