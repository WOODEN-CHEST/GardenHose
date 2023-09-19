using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine;

internal class EnumValueException : ArgumentOutOfRangeException
{
    public EnumValueException(string paramName, string enumName, object enumValue, int enumValueInt)
        : base(paramName, $"Invalid value for enum type {enumName}: \"{enumValue}\" (int: {enumValueInt})")
    { }
}