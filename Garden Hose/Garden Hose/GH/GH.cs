using GardenHose.Settings;
using GardenHoseEngine;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GardenHose;

internal static class GH
{
    // Static fields.
    internal static Version GameVersion { get; } = new(0, 1, 0, 0);
    internal static GameSettings GameSettings { get; set; }


    /* Global assets. */
    internal static SpriteFont GeeichFont { get; private set; }
    internal static SpriteFont GeeichFontLarge { get; private set; }


    // Internal static methods.
    internal static void LoadGlobalAssets()
    {
        GeeichFont = AssetManager.GetFont(null, "geeich");
        GeeichFontLarge = AssetManager.GetFont(null, "geeich_large");
    }
}