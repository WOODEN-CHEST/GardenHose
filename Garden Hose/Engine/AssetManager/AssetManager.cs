using GardenHose.Engine.Translatable;
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

    private static readonly Dictionary<string, MonoGameAsset<Texture2D>> s_textures = new();
    private static readonly Dictionary<string, MonoGameAsset<SoundEffect>> s_sounds = new();
    private static readonly Dictionary<string, MonoGameAsset<SpriteFont>> s_fonts = new();
    private static readonly Dictionary<string, MonoGameAsset<SpriteEffect>> s_shaders = new();



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
        //foreach (string PackPath in Directory.GetDirectories(s_extraPath))
        //{
        //    ScanPack(PackPath);
        //}
    }


    /* Obtaining or disposing assets */
    public static Texture2D GetTexture(string relativePath)
    {
        return GetAsset(s_textures, Path.Combine(DIR_TEXTURES, relativePath));
    }

    public static SoundEffect GetSound(string relativePath)
    {
        return GetAsset(s_sounds, Path.Combine(DIR_SOUNDS, relativePath));
    }

    public static SpriteFont GetFont(string relativePath)
    {
        return GetAsset(s_fonts, Path.Combine(DIR_FONTS, relativePath));
    }

    public static SpriteEffect GetShader(string relativePath)
    {
        return GetAsset(s_shaders, Path.Combine(DIR_SHADERS, relativePath));
    }


    public static void DisposeTexture(string relativePath)
    {
        DisposeUser(s_textures, Path.Combine(DIR_TEXTURES, relativePath));
    }

    public static void DisposeSound(string relativePath)
    {
        DisposeUser(s_sounds, Path.Combine(DIR_SOUNDS, relativePath));
    }

    public static void DisposeFont(string relativePath)
    {
        DisposeUser(s_fonts, Path.Combine(DIR_FONTS, relativePath));
    }

    public static void DisposeShader(string relativePath)
    {

        DisposeUser(s_shaders, Path.Combine(DIR_SHADERS, relativePath));
    }


    /* Manage memory */
    public static void FreeMemory()
    {
        foreach (var Item in s_textures.Values) if (Item.Users == 0) Item.UnloadAsset();
        foreach (var Item in s_sounds.Values) if (Item.Users == 0) Item.UnloadAsset();
        foreach (var Item in s_fonts.Values) if (Item.Users == 0) Item.UnloadAsset();
        foreach (var Item in s_shaders.Values) if (Item.Users == 0) Item.UnloadAsset();
    }


    // Private methods.
    private static void CreateEntries<EntryType>(in Dictionary<string, MonoGameAsset<EntryType>> entries, in string path)
        where EntryType : class
    {
        foreach (string NewPath in Directory.EnumerateDirectories(path))
        {
            CreateEntries(entries, NewPath);
        }

        foreach (string EntryPath in Directory.EnumerateFiles(path, "*.xnb"))
        {
            string FullPathWithoutExt = Path.Combine(Path.GetDirectoryName(EntryPath), Path.GetFileNameWithoutExtension(EntryPath));

            entries.Add(Path.GetRelativePath(s_basePath, FullPathWithoutExt),
                new MonoGameAsset<EntryType>(FullPathWithoutExt));
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

    private static AssetType GetAsset<AssetType>(in Dictionary<string, MonoGameAsset<AssetType>> assets, string relativePath)
        where AssetType : class
    {
        if (!assets.TryGetValue(relativePath, out MonoGameAsset<AssetType> GameAsset))
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

    private static void DisposeUser<AssetType>(in Dictionary<string, MonoGameAsset<AssetType>> assets, string path)
        where AssetType : class
    {
        if (!assets.TryGetValue(path, out MonoGameAsset<AssetType> GameAsset)) return;
        GameAsset.DisposeUser();
    }
}