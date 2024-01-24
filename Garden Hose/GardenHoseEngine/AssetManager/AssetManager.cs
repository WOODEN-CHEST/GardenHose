// Ignore Spelling: SHADERS Overrideable

using GardenHoseEngine.Audio;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Logging;
using GardenHoseEngine.Screen;
using GardenHoseEngine.Translatable;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHoseEngine;

/* It's a bit confusing so explanation, and for me to remember.
 * Monogame assets -> Stored with monogame content manager.
 * Custom assets -> Stored in this asset manager.
 * FullPath = Fully qualified path to file, except its extension. 
 * RelativePath = Relative path of a file in any pack, no extension. 
 * Base path = Fully qualified paths to root of asset paths.
 * Extra path = Fully qualified path to asset packs directory. */
public static class AssetManager
{
    // Static fields.
    public const string DIR_TEXTURES = "textures";
    public const string DIR_SOUNDS = "sounds";
    public const string DIR_FONTS = "fonts";
    public const string DIR_SHADERS = "shaders";
    public const string DIR_LANGUAGES = "languages";


    // Fields.
    public static string BasePath { get; private set; }

    public static string? ExtraPath { get; set; }

    public static readonly HashSet<string> NonOverridableDirectories = new() { DIR_FONTS, DIR_SHADERS };


    // Private fields.
    private const string AUDIO_EXTENSION = ".mp3";
    private const string TEXTURE_EXTENSION = ".png";
    private const string LANGUAGE_EXTENSION = ".lang";

    private static readonly Dictionary<IGameFrame, HashSet<object>> s_users = new(4);
    private static readonly Dictionary<string, object> s_assetEntries = new(); // Key = relative asset path. Value =  asset.
    private static readonly Dictionary<string, string> s_overrideAssets = new(); // Key = relative asset path. Value = pack name.
    private static readonly HashSet<string> s_monogameHandledAssetTypes = new() { DIR_TEXTURES, DIR_SHADERS, DIR_FONTS };


    // Internal static methods.
    internal static void Initialize(string basePath, string? extraPath)
    {
        if (basePath == null)
        {
            throw new ArgumentNullException(nameof(basePath));
        }
        if (!Path.IsPathFullyQualified(basePath))
        {
            throw new ArgumentException($"base path \"{basePath}\" is not fully qualified");
        }
        BasePath = basePath;
        GHEngine.Game.Content.RootDirectory = basePath;

        ExtraPath = extraPath;
        if ((extraPath != null) && (!Path.IsPathFullyQualified(extraPath)))
        {
            throw new ArgumentException($"Extra path \"{extraPath}\" is not fully qualified");
        }
    }

    // Methods.
    /* Managing assets. */
    public static void ScanOverrideAssets(string packName)
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
                Logger.Info($"Pack \"{FullAssetPackRootPath}\" attempted to override" +
                    $" a non-overridable path \"{fullDirPath}\"");
                return;
            }

            foreach (string fullAssetPathWithExt in Directory.GetFiles(fullDirPath))
            {
                s_overrideAssets[Path.GetRelativePath(FullAssetPackRootPath, 
                    Path.ChangeExtension(fullAssetPathWithExt, null))] = packName;
            }
            foreach (string FullSubDirPath in Directory.GetDirectories(fullDirPath))
            {
                ScanDirectory(FullSubDirPath);
            }
        }
    }

    public static void UseAssets(IGameFrame user, IEnumerable<string> relativePaths)
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

    public static void UseAssets(IGameFrame user, string relativeDir)
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

    public static void FreeAssets(IGameFrame user, IEnumerable<string> relativePaths)
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

    public static void FreeAsset(IGameFrame user, string relativePath)
    {
        ValidateUser(user);

        if (relativePath == null)
        {
            throw new ArgumentNullException(nameof(relativePath));
        }
        FreeAssetFromUser(user, relativePath);
    }


    /* Getting assets. */
    public static Texture2D GetTexture(IGameFrame user, string relativePath)
    {
        return GetAsset<Texture2D>(user, Path.Combine(DIR_TEXTURES, relativePath));
    }

    public static Sound GetSoundEffect(IGameFrame user, string relativePath)
    {
        return GetAsset<Sound>(user, Path.Combine(DIR_SOUNDS, relativePath));
    }

    public static Effect GetShader(IGameFrame user, string relativePath)
    {
        return GetAsset<Effect>(user, Path.Combine(DIR_SHADERS, relativePath));
    }

    public static SpriteFont GetFont(IGameFrame user, string relativePath)
    {
        return GetAsset<SpriteFont>(user, Path.Combine(DIR_FONTS, relativePath));
    }

    public static object GetLanguage(IGameFrame user, string relativePath)
    {
        return GetAsset<Language>(user, Path.Combine(DIR_LANGUAGES, relativePath));
    }


    // Internal methods.
    internal static void RegisterGameFrame(IGameFrame gameFrame)
    {
        if (gameFrame == null)
        {
            throw new ArgumentNullException(nameof(gameFrame));
        }

        s_users.TryAdd(gameFrame, new());
    }

    internal static void UnregisterGameFrame(IGameFrame gameFrame)
    {
        if (gameFrame == null)
        {
            throw new ArgumentNullException(nameof(gameFrame));
        }

        s_users.Remove(gameFrame);
    }

    internal static void FreeUnusedAssets()
    {
        List<string> relativePaths = new(s_assetEntries.Count);

        foreach (KeyValuePair<string, object> Asset in s_assetEntries)
        {
            if (!IsAssetUsed(Asset.Key))
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
    private static void LoadAssetDynamic(string relativePath)
    {
        bool IsPathOverridden = s_overrideAssets.ContainsKey(relativePath);
        string FullPath = IsPathOverridden
            ? GetFullOverrideAssetPath(relativePath)
            : Path.Combine(BasePath, relativePath);

        s_assetEntries[relativePath] = GetPathRoot(relativePath) switch
        {
            DIR_TEXTURES => LoadTexture(FullPath, IsPathOverridden),

            DIR_SOUNDS => LoadSound(FullPath),

            DIR_SHADERS => LoadShader(FullPath),

            DIR_FONTS => LoadFont(FullPath),

            DIR_LANGUAGES => LoadLanguage(FullPath),

            _ => throw new ArgumentOutOfRangeException(nameof(relativePath),
                    $"Couldn't resolve type of asset for \"{relativePath}\"")
        };
    }

    private static object LoadTexture(string fullPath, bool pathOverridden) 
    {
        if (pathOverridden)
        {
            return Texture2D.FromFile(Display.GraphicsManager.GraphicsDevice, 
                Path.ChangeExtension(fullPath, TEXTURE_EXTENSION));
        }
        return GHEngine.Game.Content.Load<Texture2D>(fullPath); // Content manager adds extension automatically.
    }

    private static object LoadSound(string fullPath)
    {
        return new Sound(Path.ChangeExtension(fullPath, AUDIO_EXTENSION), AudioEngine.Engine);
    }

    private static object LoadShader(string fullPath) 
    {
        return GHEngine.Game.Content.Load<Effect>(fullPath);  // Content manager adds extension automatically.
    }

    private static object LoadFont(string fullPath) 
    {
        return GHEngine.Game.Content.Load<SpriteFont>(fullPath); // Content manager adds extension automatically.
    }

    private static object LoadLanguage(string fullPath)
    {
        return new LanguageFileParser().ParseLanguage(fullPath);
    }

    private static object LoadModel(string fullPath)
    {
        throw new NotImplementedException();
    }

    private static T GetAsset<T>(IGameFrame user, string relativePath)
    {
        ValidateUser(user);
        UseAsset(user, relativePath);
        return (T)s_assetEntries[relativePath];
    }


    /* Assets. */
    private static bool IsAssetUsed(string relativeAssetPath)
    {
        foreach (var UserAssets in s_users.Values)
        {
            if (UserAssets.Contains(relativeAssetPath))
            {
                return true;
            }
        }

        return false;
    }
    
    private static void UseAsset(IGameFrame user, string relativePath)
    {
        if (!s_assetEntries.ContainsKey(relativePath))
        {
            LoadAssetDynamic(relativePath);
        }
        s_users[user].Add(relativePath);
    }

    private static void FreeAssetFromUser(IGameFrame user, string relativePath)
    {
        if (!IsAssetUsed(relativePath))
        {
            UnloadAsset(relativePath);
        }
        s_users[user].Remove(relativePath);
    }

    private static void UnloadAsset(string relativePath)
    {
        if (s_monogameHandledAssetTypes.Contains(GetPathRoot(relativePath)))
        {
            // Monogame stores assets with '/' rather than '\'.
            // Loading an asset does this conversion automatically, but unloading does not.
            if (s_overrideAssets.ContainsKey(relativePath))
            {
                GHEngine.Game.Content.UnloadAsset(GetFullOverrideAssetPath(relativePath).Replace('\\', '/'));
            }
            else GHEngine.Game.Content.UnloadAsset(Path.Combine(BasePath, relativePath).Replace('\\', '/'));
        }
        s_assetEntries.Remove(relativePath);
    }


    /* Paths. */
    private static string GetFullOverrideAssetPath(string relativePath)
    {
        return Path.Combine(ExtraPath!, Path.Combine(s_overrideAssets[relativePath], relativePath));
    }

    private static string GetPathRoot(string relativePath) => relativePath.Split('/', '\\')[0];


    /* Other */
    private static void ValidateUser(IGameFrame gameFrame)
    {
        if (gameFrame == null)
        {
            throw new ArgumentNullException(nameof(gameFrame));
        }

        if (!s_users.ContainsKey(gameFrame))
        {
            throw new ArgumentException($"{nameof(GameFrame)} \"{gameFrame.Name}\" " +
                $"is not registered as an asset user. ");
        }
    }
}