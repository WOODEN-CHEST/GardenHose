// Ignore Spelling: SHADERS Overrideable

using GardenHoseEngine.Assets;
using GardenHoseEngine.Audio;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHoseEngine;

/* It's a bit confusing so explanation, and for me to remember.
 * Monogame assets -> Stored with monogame content manager.
 * Custom assets -> Stored in this asset manager.
 * FullPath = Fully qualified path to file, except its extension. 
 * RelativePath = Relative path of a file in any pack, no extension. 
 * Base path = Fully qualified paths to root of asset paths. */
public static class AssetManager
{
    // Static fields.
    public const string DIR_TEXTURES = "textures";
    public const string DIR_SOUNDS = "sounds";
    public const string DIR_FONTS = "fonts";
    public const string DIR_SHADERS = "shaders";

    public const string EXTENSION_AUDIO_FILE = "mp3";


    // Fields.
    public static string BasePath { get; private set; }

    public static readonly HashSet<string> NonOverridableDirectories = new() { DIR_FONTS, DIR_SHADERS };


    // Private fields.
    private const char WRONG_PATH_SEPARATOR = '\\';
    private const char CORRECT_PATH_SEPARATOR = '/';

    private static readonly Dictionary<string, GameAsset> s_assets = new();


    // Internal static methods.
    internal static void Initialize(string basePath)
    {
        if (basePath == null)
        {
            throw new ArgumentNullException(nameof(basePath));
        }
        if (!Path.IsPathFullyQualified(basePath))
        {
            throw new ArgumentException($"base path \"{basePath}\" is not fully qualified");
        }
        BasePath = FormatPath(basePath);
        GHEngine.Game.Content.RootDirectory = basePath;
    }


    // Methods.
    public static void FreeAsset(IGameFrame user, string relativePath)
    {
        relativePath = FormatPath(relativePath);

        s_assets.TryGetValue(relativePath, out var Asset);
        if (Asset != null)
        {
            Asset.RemoveUser(user, GHEngine.Game.Content);
        }
    }

    public static void FreeAllUserAssets(IGameFrame user)
    {
        foreach (GameAsset Asset in s_assets.Values)
        {
            Asset.RemoveUser(user, GHEngine.Game.Content);
        }
    }


    /* Getting assets. */
    // Passing null to any GetAsset method will load the asset perpetually. This is intended, use with care.
    public static Texture2D GetTexture(IGameFrame? user, string relativePath)
    {
        return (Texture2D)GetAsset(user, $"{DIR_TEXTURES}{CORRECT_PATH_SEPARATOR}{relativePath}", AssetType.Texture);
    }

    public static Sound GetSoundEffect(IGameFrame? user, string relativePath)
    {
        return (Sound)GetAsset(user, 
            Path.ChangeExtension($"{DIR_SOUNDS}{CORRECT_PATH_SEPARATOR}{relativePath}", EXTENSION_AUDIO_FILE), AssetType.Sound);
    }

    public static Effect GetShader(IGameFrame? user, string relativePath)
    {
        return (Effect)GetAsset(user, $"{DIR_SHADERS}{CORRECT_PATH_SEPARATOR}{relativePath}", AssetType.Shader);
    }

    public static SpriteFont GetFont(IGameFrame? user, string relativePath)
    {
        return (SpriteFont)GetAsset(user, $"{DIR_FONTS}{CORRECT_PATH_SEPARATOR}{relativePath}", AssetType.Font);
    }


    // Private methods.
    private static object GetAsset(IGameFrame? user, string relativePath, AssetType type)
    {
        relativePath = FormatPath(relativePath);

        GameAsset AssetEntry;

        if (!s_assets.ContainsKey(relativePath))
        {
            AssetEntry = new($"{BasePath}{CORRECT_PATH_SEPARATOR}{relativePath}", type);
            s_assets[relativePath] = AssetEntry;
        }

        AssetEntry = s_assets[relativePath];
        AssetEntry.AddUser(user, GHEngine.Game.Content);
        return AssetEntry.Asset!;
    }


    // IMPORTANT: Monogame stores assets with '/' rather than '\'.
    // Monogame's content manager converts the paths to use '/' when loading the asset and stores the converted paths,
    // BUT the unload function does not do this conversion and thus can fail if it contains '\', so all '\' are replaced with '/'.
    private static string FormatPath(string relativePath) => relativePath.Replace(WRONG_PATH_SEPARATOR, CORRECT_PATH_SEPARATOR);
}