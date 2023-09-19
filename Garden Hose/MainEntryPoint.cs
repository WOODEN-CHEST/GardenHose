using GardenHose;
using GardenHose.Frames;
using GardenHoseEngine;

using GHEngine Engine = new(new GHEngineStartupSettings()
{
    GameName = "GH",
    InternalName = "gh",
    VirtualSize = new(1920f, 1080f),
    StartupFrame = new TestFrame("test")
});

GH.Engine = Engine;
Engine.Execute();