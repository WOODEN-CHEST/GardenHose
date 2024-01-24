using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO.DataFile;

public interface IWriteableDataCompound
{
    // Fields.
    public int? ID { get; }

    public int ItemCount { get; }

    public byte[] Data { get; }

    public int FormatVersion { get; }


    // Methods.
    public IWriteableDataCompound WriteByte(int id, byte value);

    public IWriteableDataCompound WriteSByte(int id, sbyte value);

    public IWriteableDataCompound WriteShort(int id, short value);

    public IWriteableDataCompound WriteUShort(int id, ushort value);

    public IWriteableDataCompound WriteInt(int id, int value);

    public IWriteableDataCompound WriteUInt(int id, uint value);

    public IWriteableDataCompound WriteLong(int id, long value);

    public IWriteableDataCompound WriteULong(int id, ulong value);

    public IWriteableDataCompound WriteFloat(int id, float value);

    public IWriteableDataCompound WriteDouble(int id, double value);

    public IWriteableDataCompound WriteBool(int id, bool value);

    public IWriteableDataCompound WriteChar(int id, char value);

    public IWriteableDataCompound WriteString(int id, string value);

    public IWriteableDataCompound WriteCompound(IWriteableDataCompound compound);


    public IWriteableDataCompound WriteByteArray(int id, byte[] array);

    public IWriteableDataCompound WriteSByteArray(int id, sbyte[] array);

    public IWriteableDataCompound WriteShortArray(int id, short[] array);

    public IWriteableDataCompound WriteUShortArray(int id, ushort[] array);

    public IWriteableDataCompound WriteIntArray(int id, int[] array);

    public IWriteableDataCompound WriteUIntArray(int id, uint[] array);

    public IWriteableDataCompound WriteLongArray(int id, long[] array);

    public IWriteableDataCompound WriteULongArray(int id, ulong[] array);

    public IWriteableDataCompound WriteFloatArray(int id, float[] array);

    public IWriteableDataCompound WriteDoubleArray(int id, double[] array);

    public IWriteableDataCompound WriteBoolArray(int id, bool[] array);

    public IWriteableDataCompound WriteStringArray(int id, string[] array);

    public IWriteableDataCompound WriteCompoundArray(int id, IWriteableDataCompound[] array);
}