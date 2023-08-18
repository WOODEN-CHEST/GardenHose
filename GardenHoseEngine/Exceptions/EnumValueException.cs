using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine;

internal class EnumValueException : ArgumentOutOfRangeException
{
    public EnumValueException(string paramNAme, string enumName, object enumValue, int enumValueInt)
        : base($"Invalid {paramNAme} value for enum type {enumName}: \"{enumValue}\" (int: {enumValueInt})")
    { }
}