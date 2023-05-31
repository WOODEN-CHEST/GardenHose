using System;
using System.Threading;
using GardenHose;
using GardenHose.Engine.Logging;

MainGame Game = new GardenHose.MainGame();

try
{
    Logger.Initialize();
    Game.Run();
}
catch (Exception e)
{
    Logger.Critical($"Game has crashed! {e.ToString()}");
}
finally
{
    Game.Dispose();
    Logger.Info("Program ended.");
    Logger.Dispose();
}