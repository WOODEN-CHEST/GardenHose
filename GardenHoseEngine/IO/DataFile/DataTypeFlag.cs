using System;


namespace GardenHose.Engine.IO.DataFile;


[Flags]
public enum DataTypeFlag : byte
{
    Array = 0b10000000
}