using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Engine.IO;

public enum DataType : byte
{
    None = 0,

    Int8 = 1,
    UInt8 = 2,
    Int16 = 3,
    UInt16 = 4,
    Int32 = 5,
    UInt32 = 6,
    Int64 = 7,
    UInt64 = 8,

    Single = 9,
    Double = 10,

    Boolean = 11,

    Char = 12,
    String = 13,

    Compound = 14,

    Array = 255
}