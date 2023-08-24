using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Immutable;
using System.IO;

namespace GardenHoseEngine;

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
    private readonly ContentManager _content;
    private readonly GHEngine _ghEngine;

    private readonly Dictionary<GameFrame, HashSet<object>> _users = new(4);
    private readonly Dictionary<string, object> _assetEntries = new();
    private readonly HashSet<string> _overrideAssetPaths = new();


    // Constructors.
    internal AssetManager(string basePath, string? extraPath, GHEngine engine, Game game)
    {
        _ghEngine = engine;
        _content = game.Content;

        BasePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        if (!Path.IsPathFullyQualified(basePath))
        {
            throw new ArgumentException($"Base path \"{basePath}\" is not fully qualified");
        }

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

        string AssetPackPath = Path.Combine(ExtraPath, packName);
        if (!Directory.Exists(AssetPackPath))
        {
            throw new ArgumentException($"Asset pack \"{packName}\" not found.");
        }

        // Scan pack.
        ScanDirectory(AssetPackPath);

        void ScanDirectory(string directory)
        {
            if (NonOverridableDirectories.Contains( Path.GetRelativePath(ExtraPath, directory) ))
            {
                _ghEngine.Logger.Info($"Pack \"{AssetPackPath}\" attempted to override" +
                    $" a non-overridable path \"{directory}\"");
                return;
            }

            foreach (string AssetPath in Directory.GetFiles(directory))
            {
                _overrideAssetPaths.Add(Path.GetRelativePath(AssetPackPath, AssetPath));
            }
            foreach (string SubDirectory in Directory.GetDirectories(directory))
            {
                ScanDirectory(SubDirectory);
            }
        }
    }

    public void UseAssets(GameFrame user, IEnumerable<string> paths)
    {
        ThrowIfUnregistered(user);

        if (paths == null)
        {
            throw new ArgumentNullException(nameof(paths));
        }

        foreach (var assetPath in paths)
        {
            UseAsset(user, assetPath);
        }
    }

    public void UseAssets(GameFrame user, string directory)
    {
        if (string.IsNullOrEmpty(directory))
        {
            throw new ArgumentNullException(nameof(directory));
        }

        if (!Directory.Exists( Path.Combine(BasePath, directory) ))
        {
            throw new DirectoryNotFoundException(
                $"No directory found with the relative path \"{directory}\"");
        }


        UseAssets(user, new DirectoryInfo(directory).GetFiles("", SearchOption.AllDirectories)
            .Select(file => Path.GetRelativePath(BasePath, GetFilePathWithoutExtension(file.FullName))) );
    }


    /* Getting assets. */
    public Texture2D GetTexture(GameFrame user, string path)
    {
        return GetAsset<Texture2D>(user, path, LoadTexture);
    }

    public SoundEffect GetSoundEffect(GameFrame user, string path)
    {
        return GetAsset<SoundEffect>(user, path, LoadSound);
    }

    public Effect GetShader(GameFrame user, string path)
    {
        return GetAsset<Effect>(user, path, LoadDefault);
    }

    public SpriteFont GetFont(GameFrame user, string path)
    {
        return GetAsset<SpriteFont>(user, path, LoadDefault);
    }

    public SpriteFont GetLanguage(GameFrame user, string path)
    {
        return GetAsset<SpriteFont>(user, path, LoadDefault);
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
        List<string> AssetsToUnload = new(_assetEntries.Count);

        foreach (KeyValuePair<string, object> Asset in _assetEntries)
        {
            if (!IsAssetUsed(Asset.Value))
            {
                AssetsToUnload.Add(Asset.Key);
            }
        }

        foreach (var Asset in AssetsToUnload)
        {
            _assetEntries.Remove(Asset);
        }
    }


    // Private methods.
    /* Loading */
    private void LoadAsset(string path)
    {
        switch (Path.GetPathRoot(path))
        {
            case DIR_TEXTURES:
                LoadTexture(path);
                break;

            case DIR_SOUNDS:
                LoadSound(path);
                break;

            case DIR_SHADERS:
                LoadShader(path);
                break;

            case DIR_FONTS:
                LoadFont(path);
                break;

            case DIR_LANGUAGES:
                LoadLanguage(path);
                break;

            case DIR_MODELS:
                LoadModel(path);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(path), 
                    $"Couldn't resolve type of asset for \"{path}\"");
        }
    }

    private void LoadMonoGameAsset(string path, Func<object>? overrideCase)
    {
        if (_overrideAssetPaths.Contains(path) && (overrideCase != null))
        {
            _assetEntries[path] = overrideCase.Invoke();
        }
        else
        {
            _assetEntries[path] = _content.Load<object>(path);
        }
    }

    private void LoadTexture(string path)
    {
        LoadMonoGameAsset(path, () => Texture2D.FromFile(_ghEngine.GraphicsDeviceManager.GraphicsDevice, path));
    }

    private void LoadSound(string path)
    {
        LoadMonoGameAsset(path, () => SoundEffect.FromFile(path));
    }

    private void LoadShader(string path)
    {
        LoadMonoGameAsset(path, null);
    }

    private void LoadFont(string path)
    {
        LoadMonoGameAsset(path, null);
    }

    private void LoadLanguage(string path)
    {
        throw new NotImplementedException();
    }

    private void LoadModel(string path)
    {
        throw new NotImplementedException();
    }

    private T GetAsset<T>(GameFrame user, string path, Action<string, AssetEntry> loadFunc)
    {
        ThrowIfUnregistered(user);
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }
        EnsureAssetEntry(user, path);

        var Asset = _assetEntries[path];
        if (Asset.Data == null)
        {
            loadFunc.Invoke(path, Asset);
        }
        return (T)Asset.Data!;
    }


    /* Assets */
    private void UseAsset(GameFrame user, string path)
    {
        if (_assetEntries.ContainsKey(path))
        {
            Asset = new();
            _assetEntries.Add(path, Asset);
        }

        _users[user].Add(Asset);
    }

    private void EnsureAssetEntry(GameFrame user, string path)
    {
        if (!_assetEntries.ContainsKey(path))
        {
            AssetEntry Asset = new();
            _assetEntries.Add(path, Asset);
            _users[user].Add(Asset);
        }
    }

    private bool IsAssetUsed(object asset)
    {
        foreach (var UserAssets in _users.Values)
        {
            if (UserAssets.Contains(asset))
            {
                return true;
            }
        }

        return false;
    }


    /* Other */
    private void ThrowIfUnregistered(GameFrame gameFrame)
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

    private string GetFilePathWithoutExtension(string path)
    {
        return Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path));
    }
}