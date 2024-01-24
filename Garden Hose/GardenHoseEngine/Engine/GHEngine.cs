using GardenHoseEngine.Audio;
using GardenHoseEngine.Logging;
using Microsoft.Xna.Framework;
using System.Diagnostics;


namespace GardenHoseEngine.Engine;


public static class GHEngine
{
    // Static fields.
    public static Game Game { get; private set; }

    public static string GameName { get; private set; }

    public static string InternalName { get; private set; }

    public static string DataRootPath { get; private set; }

    public static GHEngineStartupSettings? StartupSettings;


    // Static methods.
    public static void Execute(GHEngineStartupSettings settings)
    {
        if (Game != null)
        {
            throw new InvalidOperationException("Engine already initialized!");
        }

        StartupSettings = settings ?? throw new ArgumentNullException(nameof(settings));

        GameName = StartupSettings.GameName;
        InternalName = StartupSettings.InternalName;
        DataRootPath = Path.Combine(StartupSettings.GameDataRootDirectory, InternalName);
        if (!Path.IsPathFullyQualified(DataRootPath))
        {
            throw new ArgumentException("Not a fully qualified data root path.", nameof(settings.GameDataRootDirectory));
        }
        Directory.CreateDirectory(DataRootPath);

        Logger.Initialize(Path.Combine(DataRootPath, "logs"));

        try
        {
            using (Game = new GHEngineGame())
            {
                Game.Run();
            }
            Stop();
        }
        catch (Exception e)
        {
            OnCrash(e);
        }
    }

    public static void Exit()
    {
        Game.Exit();
    }


    // Private static methods.
    private static void OnCrash(Exception e)
    {
        Logger.Critical($"Game has crashed! " +
        $"Main thread ID: {Environment.CurrentManagedThreadId}. Info: {e}");
        Stop();

        if (OperatingSystem.IsWindows())
        {
            Process.Start("notepad", Logger.LogPath);
        }
    }

    private static void Stop()
    {
        Logger.Stop();
        AudioEngine.Engine.Dispose();
    }
}