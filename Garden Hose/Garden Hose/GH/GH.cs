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
    internal static GameSettings GameSettings { get; set; }


    /* Global asset paths. */
    internal const string ASSET_GH_FONT = "geeich";
    internal const string ASSET_GH_FONT_LARGE = "geeich_large";
}