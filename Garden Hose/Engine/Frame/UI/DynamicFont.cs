using Microsoft.Xna.Framework.Graphics
;using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace GardenHose.Engine.Frame.UI;


public sealed class DynamicFont : IDisposable
{
    // Fields.
    public SpriteFont FontAsset { get; private set; }

    public readonly string Name;

    public float Scale
    {
        get { return _scale; }
        set
        {
            _scale = Math.Max(0.1f, Math.Min(value, 5f));
        }
    }


    // Private static fields.
    private static readonly Dictionary<string, DynamicFont> s_fonts = new();


    // Private fields.
    private float _scale = 1f;
    private string _relativePath;


    // Constructors.
    public DynamicFont(string name, string relativePath)
    {
        _relativePath = relativePath;
        FontAsset = AssetManager.GetFont(_relativePath);
        Name = name;
    }


    // Static methods.
    public static void AddFont(string name, string relativePath)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException($"Invalid font name: \"{name}\"");
        s_fonts.Add(name, new DynamicFont(name, relativePath));
    }

    public static void ClearFonts()
    {
        foreach (var Font in s_fonts.Values) Font.Dispose();
        s_fonts.Clear();
    }

    public static DynamicFont GetFont(string name) => s_fonts[name];


    // Methods.
    public Vector2 MeasureString(in string text) => FontAsset.MeasureString(text) * _scale;


    // Inherited methods.
    public void Dispose()
    {
        AssetManager.DisposeFont(_relativePath);
        _relativePath = null;
        FontAsset = null;
    }
}
