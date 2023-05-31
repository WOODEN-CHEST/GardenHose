using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace GardenHose.Engine.Frame.UI.Animation;

public struct AnimationFrame : IDisposable
{
    // Fields.
    public Vector2 Origin;
    public Texture2D Texture;


    // Private fields.
    private readonly string _relativePath;


    // Contructors.
    public AnimationFrame(Vector2? origin, string relativePath)
    {
        _relativePath = Path.Combine(AssetManager.DIR_TEXTURES, relativePath);
        Texture = AssetManager.GetTexture(_relativePath);

        if (origin.HasValue)
        {
            Origin = origin.Value;
            return;
        }

        Origin.X = Texture.Width / 2;
        Origin.Y = Texture.Height / 2;
    }


    // Inherited methods.
    public void Dispose()
    {
        AssetManager.DisposeTexture(_relativePath);
        Texture = null;
    }
}