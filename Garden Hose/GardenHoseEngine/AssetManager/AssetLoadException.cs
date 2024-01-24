using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine;

internal class AssetLoadException : Exception
{
    public AssetLoadException(string path, string message) 
        : base($"Failed to load asset \"{path}\". {message}") { }
}