using GardenHose.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose;

internal static class GH
{
    // Static fields.
    internal const int GameVersion = 1;

    internal static GHEngine Engine { get; set; }

    internal static GameSettings GameSettings { get; set; }
}