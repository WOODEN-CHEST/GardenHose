using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine;


public record class GHEngineStartupSettings
{
    // Fields.
    public string InternalName { get; init; }
    public string GameName { get; init; }

    public string GameDataRootDirectory { get; init; }
    public string AssetBasePath { get; init; }
    public string? AssetExtraPath { get; init; }

    public Vector2 VirtualSize { get; init; }
    public Vector2? WindowSize { get; init; }
    public bool IsFullScreen { get; init; }
    public bool IsMouseVisible { get; init; }
    public bool AllowAltF4 { get; init; }
    public bool  AllowUserResizing { get; init; }

    public IGameFrame StartupFrame { get; init; }
    public IGameFrame GlobalFrame { get; init; }


    // Constructors.
    public GHEngineStartupSettings()
    {
        InternalName = "unnamed_game";
        GameName = "Unnamed Game";

        GameDataRootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        AssetBasePath = Path.Combine(
            Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName,  "assets");
        AssetExtraPath = null;

        VirtualSize = new(1920f, 1080f);
        WindowSize = null;
        IsFullScreen = false;
        IsMouseVisible = true;
        AllowAltF4 = true;
        AllowUserResizing = true;

        StartupFrame = new GameFrame("Default Startup Frame");
        GlobalFrame = new GameFrame("Default Global Frame");
    }
}