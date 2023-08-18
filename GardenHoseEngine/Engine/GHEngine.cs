using GardenHose;
using GardenHoseEngine.Logging;
using Microsoft.Xna.Framework;
using System.Diagnostics;


namespace GardenHoseEngine;

public sealed class GHEngine : IDisposable
{
    // Fields.
    public Logger Logger { get; init; }

    public string GameName
    {
        get => _gameName;
        set => _gameName = value ?? throw new ArgumentNullException(nameof(value));
    }


    // Internal fields.
    internal readonly string InternalName;
    internal GHGame Game { get; init; }
    internal GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
    internal Test AssetManager { get; init; }


    // Private fields.
    private string _gameName;


    // Constructors.
    public GHEngine(string internalName,
        string gameName,
        string logDirectory,
        string assetBasePath,
        string? assetExtraPath)
    {
        if (string.IsNullOrWhiteSpace(internalName))
        {
            throw new ArgumentNullException(nameof(internalName));
        }

        InternalName = internalName;
        Logger = new(logDirectory, InternalName);

        try 
        {
            GameName = gameName;
            Game = new();
            GraphicsDeviceManager = new(Game);
            AssetManager = new(assetBasePath, assetExtraPath, this, Game);
        }
        catch (Exception e)
        {
            OnCrash(e);
            throw new Exception($"Failed to create instance of GHEngine. {e}");
        }
    }


    // Methods.
    public void Execute()
    {
        try
        {
            Game.Run();
        }
        catch (Exception e)
        {
            OnCrash(e);
        }
        finally
        {
            Dispose();
        }
    }


    // Private methods.
    private void OnCrash(Exception e)
    {
        Logger.Critical($"Game has crashed! " +
        $"Main thread ID: {Environment.CurrentManagedThreadId}. Info: {e}");
        Dispose();

        if (OperatingSystem.IsWindows())
        {
            Process.Start("notepad", Logger.LogPath);
        }
    }


    // Inherited methods.
    public void Dispose()
    {
        Game?.Dispose();
        Logger?.Dispose();
    }
}