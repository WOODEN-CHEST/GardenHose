using GardenHose;
using GardenHose.Engine.Logging;
using System;
using System.Diagnostics;

MainGame Game = new GardenHose.MainGame();

try
{
    Logger.Initialize();
    Game.Run();
}
catch (Exception e)
{
    Logger.Critical($"Game has crashed! {e}");

    using Process Viewer = new();
    Viewer.StartInfo.FileName = "explorer";
    Viewer.StartInfo.Arguments = Logger.FilePath;
    Viewer.Start();
}
finally
{
    Game.Dispose();
    Logger.Info("Program ended.");
    Logger.Dispose();
}