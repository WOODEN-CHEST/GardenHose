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
    public string InternalName { get; set; }
    public string GameName { get; set; }

    public string GameDataRootDirectory { get; set; }
    public string AssetBasePath { get; set; }
    public string? AssetExtraPath { get; set; }

    public Vector2 VirtualSize { get; set; }
    public Vector2? WindowSize { get; set; }
    public bool IsFullScreen { get; set; }
    public bool IsMouseVisible { get; set; }
    public bool AllowAltF4 { get; set; }
    public bool  AllowUserResizing { get; set; }

    public IGameFrame StartupFrame { get; set; }
    public IGameFrame GlobalFrame { get; set; }


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