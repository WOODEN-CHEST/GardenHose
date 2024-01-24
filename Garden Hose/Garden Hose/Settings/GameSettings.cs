using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Settings;

internal class GameSettings : ICloneable
{
    // Fields.
    public required bool IsFullScreen { get; set; }


    // Inherited methods.
    public object Clone()
    {

        return new GameSettings()
        {
            IsFullScreen = IsFullScreen
        };
    }
}