using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace GardenHose.Engine.Frame.UI;


public class DynamicFont : IDisposable
{
    // Fields.
    public SpriteFont FontSprite { get; private set; }

    public float Scale
    {
        get { return _size; }
        set
        {
            _size = Math.Max(0.1f, Math.Min(value, 5f));
        }
    }


    // Private static fields.
    private static readonly Dictionary<string, DynamicFont> s_fonts;


    // Private fields.
    private float _size = 1f;
    private string _relativePath;


    // Constructors.
    public DynamicFont(string relativePath)
    {
        _relativePath = Path.Combine(AssetManager.DIR_FONTS, relativePath);
        FontSprite = AssetManager.GetFont(_relativePath);
    }


    // Static methods.
    public static void AddFont(string name, string relativePath)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException($"Invalid font name: \"{name}\"");

        s_fonts.Add(name, new DynamicFont(relativePath));
    }

    public static void ClearFonts()
    {
        foreach (var Font in s_fonts.Values) Font.Dispose();
        s_fonts.Clear();
    }

    public static DynamicFont GetFont(string name) => s_fonts[name];


    // Inherited methods.
    public void Dispose()
    {
        AssetManager.DisposeFont(_relativePath);
        _relativePath = null;
        FontSprite = null;
    }
}
