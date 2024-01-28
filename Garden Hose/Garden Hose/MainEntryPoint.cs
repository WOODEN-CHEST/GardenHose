using GardenHose.Frames.Intro;
using GardenHoseEngine.Engine;

GHEngine.Execute(new GHEngineStartupSettings()
{
    GameName = "GH",
    InternalName = "gh",
    VirtualSize = new(1920f, 1080f),
    StartupFrame = new IntroFrame("Intro"),
    IsMouseVisible = false
});