using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GardenHoseEngine;
using System;
using System.IO;

namespace GardenHoseEngine.Frame.Animation;

public struct AnimationFrame
{
    // Fields.
    public Vector2 Origin { get; private set; }
    public readonly Texture2D Texture;


    // Constructors.
    internal AnimationFrame(Vector2? origin, Texture2D texture) : this (texture)
    {
        if (origin.HasValue)
        {
            Origin = origin.Value;
        }
        else
        {
            Origin = new(Texture.Width / 2f, Texture.Height / 2f);
        }
    }

    internal AnimationFrame(Origin origin, Texture2D texture) : this(texture)
    {
        SetTextureOrigin(origin);
    }

    private AnimationFrame(Texture2D texture)
    {
        Texture = texture ?? throw new ArgumentNullException(nameof(texture));
    }


    // Methods.
    public void SetTextureOrigin(Vector2 origin) =>  Origin = origin;

    public void SetTextureOrigin(Origin origin)
    {
        Vector2 NewOrigin = Vector2.Zero;

        NewOrigin.X = ((int)(origin) % 3) switch
        {
            0 => 0f,
            1 => Texture.Width / 2,
            2 => Texture.Width,
            _ => throw new EnumValueException(nameof(origin), nameof(GardenHoseEngine.Origin),
                origin.ToString(), (int)origin),
        };

        NewOrigin.Y = ((int)(origin) / 3) switch
        {
            0 => 0f,
            1 => Texture.Height / 2,
            2 => Texture.Height,
            _ => throw new EnumValueException(nameof(origin), nameof(GardenHoseEngine.Origin),
                origin.ToString(), (int)origin),
        };

        Origin = NewOrigin;
    }


    // Operators.
    public static implicit operator Texture2D(AnimationFrame frame) => frame.Texture; 
}