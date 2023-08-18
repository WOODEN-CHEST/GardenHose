using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace GardenHose.Engine.IO.DataFile;

public class WriteDataCompound
{
    // Fields.
    public readonly int? ID;
    public int ItemCount => _usedIDs.Count;


    // Private fields.
    private readonly List<byte> _data;
    private readonly HashSet<int> _usedIDs = new();


    // Constructors.
    public WriteDataCompound() : this(null) { }

    public WriteDataCompound(int? id) : this(id, 0) { }

    public WriteDataCompound(int? id, int capacity)
    {
        ID = id;
        _data = new(capacity);
    }


    // Methods.

    /* Single values */
    public WriteDataCompound WriteByte(int id, byte value)
    {
        WriteData(id, DataType.Int8, new byte[] { value });
        return this;
    }

    public WriteDataCompound WriteSByte(int id, sbyte value)
    {
        WriteData(id, DataType.SInt8, new byte[] { (byte)value });
        return this;
    }

    public WriteDataCompound WriteShort(int id, short value)
    {
        WriteData(id, DataType.Int16, BitConverter.GetBytes(value));
        return this;
    }

    public WriteDataCompound WriteUShort(int id, ushort value)
    {
        WriteData(id, DataType.UInt16, BitConverter.GetBytes(value));
        return this;
    }

    public WriteDataCompound WriteInt(int id, int value)
    {
        WriteData(id, DataType.Int32, BitConverter.GetBytes(value));
        return this;
    }

    public WriteDataCompound WriteUInt(int id, uint value)
    {
        WriteData(id, DataType.UInt32, BitConverter.GetBytes(value));
        return this;
    }

    public WriteDataCompound WriteLong(int id, long value)
    {
        WriteData(id, DataType.Int64, BitConverter.GetBytes(value));
        return this;
    }

    public WriteDataCompound WriteULong(int id, ulong value)
    {
        WriteData(id, DataType.UInt64, BitConverter.GetBytes(value));
        return this;
    }

    public WriteDataCompound WriteFloat(int id, float value)
    {
        WriteData(id, DataType.Single, BitConverter.GetBytes(value));
        return this;
    }

    public WriteDataCompound WriteDouble(int id, double value) 
    {
        WriteData(id, DataType.Double, BitConverter.GetBytes(value));
        return this;
    }

    public WriteDataCompound WriteBool(int id, bool value)
    {
        WriteData(id, DataType.Boolean, BitConverter.GetBytes(value));
        return this;
    }

    public WriteDataCompound WriteChar(int id, char value)
    {
        WriteData(id, DataType.Char, Encoding.UTF8.GetBytes(new char[] { value }));
        return this;
    }

    public WriteDataCompound WriteString(int id, string value)
    {
        VerifyAndUseID(id);

        Write7BitEncodedInt(id);
        _data.Add((byte)DataType.String);
        Write7BitEncodedInt(value.Length);
        _data.AddRange(Encoding.UTF8.GetBytes(value));

        return this;
    }

    public WriteDataCompound WriteCompound(WriteDataCompound compound)
    {
        ArgumentNullException.ThrowIfNull(compound);

        if (compound.ID == null)
        {
            throw new InvalidOperationException("ID of a non-base compound is null");
        }

        VerifyAndUseID(compound.ID.Value);

        Write7BitEncodedInt(compound.ID.Value);
        _data.Add((byte)DataType.Compound);
        Write7BitEncodedInt(compound.ItemCount);
        _data.AddRange(compound._data);

        return this;
    }


    /* Arrays */
    public WriteDataCompound WriteByteArray(int id, byte[] array)
    {
        WriteDataArray(id, DataType.Int8, array, array.Length);
        return this;
    }

    public WriteDataCompound WriteSByteArray(int id, sbyte[] array)
    {
        WriteDataArray(id, DataType.SInt8, MemoryMarshal.AsBytes<sbyte>(array).ToArray(), array.Length);
        return this;
    }

    public WriteDataCompound WriteShortArray(int id, short[] array)
    {
        WriteDataArray(id, DataType.Int16, MemoryMarshal.AsBytes<short>(array).ToArray(), array.Length);
        return this;
    }

    public WriteDataCompound WriteUShortArray(int id, ushort[] array)
    {
        WriteDataArray(id, DataType.UInt16, MemoryMarshal.AsBytes<ushort>(array).ToArray(), array.Length);
        return this;
    }

    public WriteDataCompound WriteIntArray(int id, int[] array)
    {
        WriteDataArray(id, DataType.Int32, MemoryMarshal.AsBytes<int>(array).ToArray(), array.Length);
        return this;
    }

    public WriteDataCompound WriteUIntArray(int id, uint[] array)
    {
        WriteDataArray(id, DataType.UInt32, MemoryMarshal.AsBytes<uint>(array).ToArray(), array.Length);
        return this;
    }
    public WriteDataCompound WriteLongArray(int id, long[] array)
    {
        WriteDataArray(id, DataType.Int64, MemoryMarshal.AsBytes<long>(array).ToArray(), array.Length);
        return this;
    }

    public WriteDataCompound WriteULongArray(int id, ulong[] array)
    {
        WriteDataArray(id, DataType.UInt64, MemoryMarshal.AsBytes<ulong>(array).ToArray(), array.Length);
        return this;
    }

    public WriteDataCompound WriteFloatArray(int id, float[] array)
    {
        WriteDataArray(id, DataType.Single, MemoryMarshal.AsBytes<float>(array).ToArray(), array.Length);
        return this;
    }

    public WriteDataCompound WriteDoubleArray(int id, double[] array)
    {
        WriteDataArray(id, DataType.Double, MemoryMarshal.AsBytes<double>(array).ToArray(), array.Length);
        return this;
    }

    public WriteDataCompound WriteBoolArray(int id, bool[] array)
    {
        WriteDataArray(id, DataType.Boolean, MemoryMarshal.AsBytes<bool>(array).ToArray(), array.Length);
        return this;
    }

    public WriteDataCompound WriteStringArray(int id, string[] array)
    {
        List<byte> Data = new(array.Length * 128);

        foreach (var Str in array)
        {
            Write7BitEncodedInt(Str.Length, Data);
            Data.AddRange(Encoding.UTF8.GetBytes(Str));
        }

        WriteDataArray(id, DataType.String, Data.ToArray(), array.Length);


        return this;
    }

    public WriteDataCompound WriteCompoundArray(int id, WriteDataCompound[] array)
    {
        List<byte> Data = new(256);

        foreach (var Compound in array)
        {
            Write7BitEncodedInt(Compound._usedIDs.Count, Data);
            Data.AddRange(Compound._data);
        }

        WriteDataArray(id, DataType.Compound, Data.ToArray(), array.Length);
        return this;
    }
    


    /* Other methods. */
    public ReadOnlySpan<byte> GetData() => CollectionsMarshal.AsSpan(_data);


    // Private methods.
    private void Write7BitEncodedInt(int value)
    {
        uint newValue = (uint)value;

        while (newValue >= 0b10000000)
        {
            _data.Add((byte)(newValue | 0b10000000));
            newValue >>>= 7;
        }
        _data.Add((byte)newValue);
    }

    private void Write7BitEncodedInt(int value, List<byte> list)
    {
        uint newValue = (uint)value;

        while (newValue >= 0b10000000)
        {
            list.Add((byte)(newValue | 0b10000000));
            newValue >>>= 7;
        }
        list.Add((byte)newValue);
    }

    private void WriteData(int id, DataType type, byte[] data)
    {
        VerifyAndUseID(id);

        Write7BitEncodedInt(id);
        _data.Add((byte)type);
        _data.AddRange(data);
    }

    private void WriteDataArray(int id, DataType type, byte[] data, int itemCount)
    {
        VerifyAndUseID(id);

        Write7BitEncodedInt(id);
        _data.Add( (byte)((int)type | (int)DataTypeFlag.Array) );
        Write7BitEncodedInt(itemCount);
        _data.AddRange(data);
    }

    private void VerifyAndUseID(int id)
    {
        if (id == 0)
        {
            throw new ArgumentException("ID of an item cannot be 0");
        }
        if (_usedIDs.Contains(id))
        {
            throw new ArgumentException($"An item with the ID {id} already exists");
        }
        
        _usedIDs.Add(id);
    }
}