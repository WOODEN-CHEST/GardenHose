using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;


namespace GardenHose.Engine;

public static class AssetManager
{
    // Constants.
    public const string DIR_TEXTURES = "textures";
    public const string DIR_SOUNDS = "sounds";
    public const string DIR_FONTS = "fonts";
    public const string DIR_SHADERS = "shaders";


    // Static fields.
    public static ContentManager Content;
    public static string BasePath
    {
        get => s_basePath;
        set
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentException(nameof(value));
            if (!Directory.Exists(value)) throw new DirectoryNotFoundException(value);

            s_basePath = value;
        }
    }
    public static string ExtraPath
    {
        get => s_extraPath;
        set
        {
            s_extraPath = value;

            if (value != null && !Directory.Exists(value))
                throw new DirectoryNotFoundException(value);
        }
    }


    // Private static fields.
    private static string s_basePath = null;
    private static string s_extraPath = null;

    private static Dictionary<string, Asset<Texture2D>> s_textures = new();
    private static Dictionary<string, Asset<SoundEffect>> s_sounds = new();
    private static Dictionary<string, Asset<SpriteFont>> s_fonts = new();
    private static Dictionary<string, Asset<SpriteEffect>> s_shaders = new();


    // Static constructors.
    static AssetManager()
    {
        Content = MainGame.Instance.Content;
    }


    // Methods.
    public static void CreateAssetEntries()
    {
        if (s_basePath == null) throw new NullReferenceException("Cannot load entries, base path is null");
        ClearEntries();

        ScanPack(s_basePath);

        if (s_extraPath == null) return;
        foreach (string PackPath in Directory.GetDirectories(s_extraPath))
        {
            ScanPack(PackPath);
        }
    }


    /* Obtaining assets */
    public static Texture2D GetTexture(string relativePath) => GetAsset(s_textures, relativePath);

    public static SoundEffect GetSound(string relativePath) => GetAsset(s_sounds, relativePath);

    public static SpriteFont GetFont(string relativePath) => GetAsset(s_fonts, relativePath);

    public static SpriteEffect GetShader(string relativePath) => GetAsset(s_shaders, relativePath);
    

    public static void DisposeTexture(string relativePath) => DisposeUser(s_textures, relativePath);

    public static void DisposeSound(string relativePath) => DisposeUser(s_sounds, relativePath);

    public static void DisposeFont(string relativePath) => DisposeUser(s_fonts, relativePath);

    public static void DisposeShader(string relativePath) => DisposeUser(s_shaders, relativePath);


    /* Manage memory */
    public static void FreeMemory()
    {
        foreach (var Item in  s_textures.Values) if (Item.Users == 0) Item.UnloadAsset();
        foreach (var Item in s_sounds.Values) if (Item.Users == 0) Item.UnloadAsset();
        foreach (var Item in s_fonts.Values) if (Item.Users == 0) Item.UnloadAsset();
        foreach (var Item in s_shaders.Values) if (Item.Users == 0) Item.UnloadAsset();
    }


    // Private methods.
    private static void CreateEntries<EntryType>(in Dictionary<string, Asset<EntryType>> entries, in string path)
        where EntryType : class
    {
        foreach (string NewPath in Directory.EnumerateDirectories(path))
        {
            CreateEntries(entries, NewPath);
        }

        foreach (string EntryPath in Directory.EnumerateFiles(path, "*.xnb"))
        {
            entries.Add(Path.GetRelativePath(s_basePath, EntryPath.Substring(0, EntryPath.LastIndexOf('.'))),
                new Asset<EntryType>(EntryPath.Substring(0, EntryPath.LastIndexOf('.'))) );
        }
    }

    private static void ClearEntries()
    {
        foreach (var Entry in s_textures.Values) Entry.UnloadAsset();
        foreach (var Entry in s_sounds.Values) Entry.UnloadAsset();
        foreach (var Entry in s_fonts.Values) Entry.UnloadAsset();
        foreach (var Entry in s_shaders.Values) Entry.UnloadAsset();

        s_textures.Clear();
        s_sounds.Clear();
        s_fonts.Clear();
        s_shaders.Clear();
    }

    private static AssetType GetAsset<AssetType>(in Dictionary<string, Asset<AssetType>> assets, string relativePath)
        where AssetType : class
    {
        Asset<AssetType> GameAsset;
        if (!assets.TryGetValue(relativePath, out GameAsset))
        {
            throw new KeyNotFoundException($"Asset entry \"{relativePath}\" does not exist");
        }

        return GameAsset.GetAsset();
    }

    private static void ScanPack(string packRootPath)
    {
        string ItemsPath;

        ItemsPath = Path.Combine(packRootPath, DIR_TEXTURES);
        if (Directory.Exists(ItemsPath)) CreateEntries(s_textures, ItemsPath);

        ItemsPath = Path.Combine(packRootPath, DIR_SOUNDS);
        if (Directory.Exists(ItemsPath)) CreateEntries(s_sounds, ItemsPath);

        ItemsPath = Path.Combine(packRootPath, DIR_FONTS);
        if (Directory.Exists(ItemsPath)) CreateEntries(s_fonts, ItemsPath);

        ItemsPath = Path.Combine(packRootPath, DIR_SHADERS);
        if (Directory.Exists(ItemsPath)) CreateEntries(s_shaders, ItemsPath);
    }

    private static void DisposeUser<AssetType>(in Dictionary<string, Asset<AssetType>> assets, string path)
        where AssetType : class
    {
        Asset<AssetType> GameAsset;
        if (!assets.TryGetValue(path, out GameAsset)) return;
        GameAsset.DisposeUser();
    }
}