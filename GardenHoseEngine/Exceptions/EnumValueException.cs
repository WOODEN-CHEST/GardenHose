using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine;

public class EnumValueException : ArgumentOutOfRangeException
{
    public EnumValueException(string paramName, string enumName, object enumValue, int enumValueInt)
        : base(paramName, $"Invalid value for enum type {enumName}: \"{enumValue}\" (int: {enumValueInt})")
    { }

    public EnumValueException(string paramName, object enumValue)
        : base(paramName, $"Invalid value for enum type {enumValue.GetType()}: \"{enumValue}\" (int: {(int)enumValue})") { }
}