using GardenHose;
using GardenHose.Engine.Logging;
using System;
using System.Diagnostics;

MainGame Game = new();

try
{
    Logger.Initialize();
    Game.Run();
}
catch (Exception e)
{
    Logger.Critical($"Game has crashed! " +
        $"Main thread ID: {Environment.CurrentManagedThreadId}. Info: {e}");

    Process.Start(Logger.FilePath);
}
finally
{
    Game.Dispose();
    Logger.Info("Game closed.");
    Logger.Dispose();
}