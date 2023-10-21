using System;


namespace GardenHoseEngine.IO.DataFile;


[Flags]
public enum DataTypeFlag : byte
{
    Array = 0b10000000
}