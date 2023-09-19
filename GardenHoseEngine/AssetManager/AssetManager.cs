// Ignore Spelling: SHADERS Overrideable

using GardenHoseEngine.Audio;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;

namespace GardenHoseEngine;

/* It's a bit confusing so explanation, and for me to remember.
 * Monogame assets -> Stored with monogame content manager.
 * Custom assets -> Stored in this asset manager.
 * FullPath = Fully qualified path to file, except its extension. 
 * RelativePath = Relative path of a file in any pack, no extension. 
 * Base path = Fully qualified paths to root of asset paths.
 * Extra path = Fully qualified path to asset packs directory. */
public class AssetManager
{
    // Static fields.
    public const string DIR_TEXTURES = "textures";
    public const string DIR_SOUNDS = "sounds";
    public const string DIR_FONTS = "fonts";
    public const string DIR_SHADERS = "shaders";
    public const string DIR_LANGUAGES = "languages";
    public const string DIR_MODELS = "models";


    // Fields.
    public readonly string BasePath;
    public readonly string? ExtraPath;
    public readonly HashSet<string> NonOverridableDirectories = new() { DIR_FONTS, DIR_SHADERS, DIR_MODELS };


    // Private fields.
    private const string AUDIO_EXTENSION = ".mp3";
    private const string TEXTURE_EXTENSION = ".png";
    private const string LANGUAGE_EXTENSION = ".lang";

    private readonly ContentManager _monogameContent;
    private readonly Audio.AudioEngine _audioEngine;
    private readonly Logger _logger;
    private readonly GraphicsDeviceManager _graphicsManager;

    private readonly Dictionary<GameFrame, HashSet<object>> _users = new(4);
    private readonly Dictionary<string, object> _assetEntries = new(); // Key = relative asset path. Value =  asset.
    private readonly Dictionary<string, string> _overrideAssets = new(); // Key = relative asset path. Value = pack name.
    private readonly HashSet<string> _monogameHandledAssetTypes = new() { DIR_TEXTURES, DIR_SHADERS, DIR_FONTS };


    // Constructors.
    internal AssetManager(string basePath, string? extraPath, Audio.AudioEngine audioEngine,
        ContentManager contentManager, GraphicsDeviceManager graphicsManager, Logger logger)
    {
        _audioEngine = audioEngine ?? throw new ArgumentNullException(nameof(audioEngine));
        _monogameContent = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
        _graphicsManager = graphicsManager ?? throw new ArgumentNullException(nameof(graphicsManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        if (basePath == null)
        {
            throw new ArgumentNullException(nameof(basePath));
        }
        if (!Path.IsPathFullyQualified(basePath))
        {
            throw new ArgumentException($"base path \"{basePath}\" is not fully qualified");
        }
        BasePath = basePath;
        _monogameContent.RootDirectory = basePath;

        ExtraPath = extraPath;
        if ((extraPath != null) && (!Path.IsPathFullyQualified(extraPath)))
        {
            throw new ArgumentException($"Extra path \"{extraPath}\" is not fully qualified");
        }
    }


    // Methods.
    /* Managing assets. */
    public void ScanOverrideAssets(string packName)
    {
        // Verify data.
        if (packName == null)
        {
            throw new ArgumentNullException(nameof(packName));
        }
        if (ExtraPath == null)
        {
            throw new InvalidOperationException("Cannot scan extra path since it's null");
        }

        string FullAssetPackRootPath = Path.Combine(ExtraPath, packName);
        if (!Directory.Exists(FullAssetPackRootPath))
        {
            throw new ArgumentException($"Asset pack \"{packName}\" not found.");
        }

        // Scan pack.
        ScanDirectory(FullAssetPackRootPath);

        void ScanDirectory(string fullDirPath)
        {
            if (NonOverridableDirectories.Contains( Path.GetRelativePath(ExtraPath, fullDirPath) ))
            {
                _logger.Info($"Pack \"{FullAssetPackRootPath}\" attempted to override" +
                    $" a non-overridable path \"{fullDirPath}\"");
                return;
            }

            foreach (string fullAssetPathWithExt in Directory.GetFiles(fullDirPath))
            {
                _overrideAssets[Path.GetRelativePath(FullAssetPackRootPath, 
                    Path.ChangeExtension(fullAssetPathWithExt, null))] = packName;
            }
            foreach (string FullSubDirPath in Directory.GetDirectories(fullDirPath))
            {
                ScanDirectory(FullSubDirPath);
            }
        }
    }

    public void UseAssets(GameFrame user, IEnumerable<string> relativePaths)
    {
        ValidateUser(user);

        if (relativePaths == null)
        {
            throw new ArgumentNullException(nameof(relativePaths));
        }

        foreach (var relativeAssetPath in relativePaths)
        {
            UseAsset(user, relativeAssetPath);
        }
    }

    public void UseAssets(GameFrame user, string relativeDir)
    {
        if (string.IsNullOrEmpty(relativeDir))
        {
            throw new ArgumentNullException(nameof(relativeDir));
        }

        if (!Directory.Exists( Path.Combine(BasePath, relativeDir) ))
        {
            throw new DirectoryNotFoundException(
                $"No directory found with the relative path \"{relativeDir}\"");
        }


        UseAssets(user, new DirectoryInfo(relativeDir).GetFiles("*", SearchOption.AllDirectories)
            .Select( file => Path.GetRelativePath(BasePath, Path.ChangeExtension(file.FullName, null)) )
            );
    }

    public void FreeAssets(GameFrame user, IEnumerable<string> relativePaths)
    {
        ValidateUser(user);

        if (relativePaths == null)
        {
            throw new ArgumentNullException(nameof(relativePaths));
        }

        foreach (var assetPath in relativePaths)
        {
            FreeAssetFromUser(user, assetPath);
        }
    }

    public void FreeAsset(GameFrame user, string relativePath)
    {
        ValidateUser(user);

        if (relativePath == null)
        {
            throw new ArgumentNullException(nameof(relativePath));
        }
        FreeAssetFromUser(user, relativePath);
    }


    /* Getting assets. */
    public Texture2D GetTexture(GameFrame user, string relativePath)
    {
        return GetAsset<Texture2D>(user, Path.Combine(DIR_TEXTURES, relativePath));
    }

    public Sound GetSoundEffect(GameFrame user, string relativePath)
    {
        return GetAsset<Sound>(user, Path.Combine(DIR_SOUNDS, relativePath));
    }

    public Effect GetShader(GameFrame user, string relativePath)
    {
        return GetAsset<Effect>(user, Path.Combine(DIR_SHADERS, relativePath));
    }

    public SpriteFont GetFont(GameFrame user, string relativePath)
    {
        return GetAsset<SpriteFont>(user, Path.Combine(DIR_FONTS, relativePath));
    }

    public object GetLanguage(GameFrame user, string relativePath)
    {
        return GetAsset<object>(user, Path.Combine(DIR_LANGUAGES, relativePath));
    }

    public object GetModel(GameFrame user, string relativePath)
    {
        return GetAsset<object>(user, Path.Combine(DIR_MODELS, relativePath));
    }


    // Internal methods.
    internal void RegisterGameFrame(GameFrame gameFrame)
    {
        if (gameFrame == null)
        {
            throw new ArgumentNullException(nameof(gameFrame));
        }

        _users.TryAdd(gameFrame, new());
    }

    internal void UnregisterGameFrame(GameFrame gameFrame)
    {
        if (gameFrame == null)
        {
            throw new ArgumentNullException(nameof(gameFrame));
        }

        _users.Remove(gameFrame);
    }

    internal void FreeUnusedAssets()
    {
        List<string> relativePaths = new(_assetEntries.Count);

        foreach (KeyValuePair<string, object> Asset in _assetEntries)
        {
            if (!IsAssetUsed(Asset.Value))
            {
                relativePaths.Add(Asset.Key);
            }
        }

        foreach (var reltivePath in relativePaths)
        {
            UnloadAsset(reltivePath);
        }
    }


    // Private methods.
    /* Loading. */
    private void LoadAssetDynamic(string relativePath)
    {
        bool IsPathOverridden = _overrideAssets.ContainsKey(relativePath);
        string FullPath = IsPathOverridden
            ? GetFullOverrideAssetPath(relativePath)
            : Path.Combine(BasePath, relativePath);

        _assetEntries[relativePath] = relativePath.Split('\\', '/')[0] switch
        {
            DIR_TEXTURES => LoadTexture(FullPath, IsPathOverridden),

            DIR_SOUNDS => LoadSound(FullPath),

            DIR_SHADERS => LoadShader(FullPath),

            DIR_FONTS => LoadFont(FullPath),

            DIR_LANGUAGES => LoadLanguage(FullPath),

            DIR_MODELS => LoadModel(FullPath),

            _ => throw new ArgumentOutOfRangeException(nameof(relativePath),
                    $"Couldn't resolve type of asset for \"{relativePath}\"")
        };
    }

    private object LoadTexture(string fullPath, bool pathOverridden) 
    {
        if (pathOverridden)
        {
            return Texture2D.FromFile(_graphicsManager.GraphicsDevice, 
                Path.ChangeExtension(fullPath, TEXTURE_EXTENSION));
        }
        return _monogameContent.Load<Texture2D>(fullPath); // Content manager adds extension automatically.
    }

    private object LoadSound(string fullPath)
    {
        return new Sound(Path.ChangeExtension(fullPath, AUDIO_EXTENSION), _audioEngine);
    }

    private object LoadShader(string fullPath) 
    {
        return _monogameContent.Load<Effect>(fullPath);  // Content manager adds extension automatically.
    }

    private object LoadFont(string fullPath) 
    {
        return _monogameContent.Load<SpriteFont>(fullPath); // Content manager adds extension automatically.
    }

    private object LoadLanguage(string fullPath)
    {
        throw new NotImplementedException();
    }

    private object LoadModel(string fullPath)
    {
        throw new NotImplementedException();
    }

    private T GetAsset<T>(GameFrame user, string relativePath)
    {
        ValidateUser(user);
        UseAsset(user, relativePath);
        return (T)_assetEntries[relativePath];
    }


    /* Assets. */
    private bool IsAssetUsed(object relativeAssetPath)
    {
        foreach (var UserAssets in _users.Values)
        {
            if (UserAssets.Contains(relativeAssetPath))
            {
                return true;
            }
        }

        return false;
    }
    
    private void UseAsset(GameFrame user, string relativePath)
    {
        if (!_assetEntries.ContainsKey(relativePath))
        {
            LoadAssetDynamic(relativePath);
        }
        _users[user].Add(relativePath);
    }

    private void FreeAssetFromUser(GameFrame user, string relativePath)
    {
        if (!IsAssetUsed(relativePath))
        {
            UnloadAsset(relativePath);
        }
        _users[user].Remove(relativePath);
    }

    private void UnloadAsset(string reltivePath)
    {
        if (_monogameHandledAssetTypes.Contains(Path.GetPathRoot(reltivePath)!))
        {
            // Monogame stores assets with '/' rather than '\'.
            // Loading an asset does this conversion automatically, but unloading does not.
            if (_overrideAssets.ContainsKey(reltivePath))
            {
                _monogameContent.UnloadAsset(GetFullOverrideAssetPath(reltivePath).Replace('\\', '/'));
            }
            else _monogameContent.UnloadAsset(Path.Combine(BasePath, reltivePath).Replace('\\', '/'));
        }
        _assetEntries.Remove(reltivePath);
    }


    /* Paths. */
    private string GetFullOverrideAssetPath(string relativePath)
    {
        return Path.Combine(ExtraPath!, Path.Combine(_overrideAssets[relativePath], relativePath));
    }


    /* Other */
    private void ValidateUser(GameFrame gameFrame)
    {
        if (gameFrame == null)
        {
            throw new ArgumentNullException(nameof(gameFrame));
        }

        if (!_users.ContainsKey(gameFrame))
        {
            throw new ArgumentException($"{nameof(GameFrame)} \"{gameFrame.Name}\" " +
                $"is not registered as an asset user. ");
        }
    }
}