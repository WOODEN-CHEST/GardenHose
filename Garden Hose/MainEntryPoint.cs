using GardenHose;
using GardenHose.Frames;
using GardenHose.Frames.Global;
using GardenHose.Frames.Intro;
using GardenHose.Frames.MainMenu;
using GardenHoseEngine;

using GHEngine Engine = new(new GHEngineStartupSettings()
{
    GameName = "GH",
    InternalName = "gh",
    VirtualSize = new(1920f, 1080f),
    StartupFrame = new IntroFrame("Intro"),
    GlobalFrame = new GlobalFrame("Global"),
    IsMouseVisible = false
});

GH.Engine = Engine;
Engine.Execute();