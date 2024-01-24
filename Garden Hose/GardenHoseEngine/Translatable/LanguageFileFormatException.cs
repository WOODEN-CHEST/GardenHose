using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Translatable;

internal class LanguageFileFormatException : Exception
{
    public LanguageFileFormatException(string message) 
        : base($"Badly formatted file: {message}") { }
}