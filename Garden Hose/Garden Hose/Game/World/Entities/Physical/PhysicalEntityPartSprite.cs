using GardenHose.Game.AssetManager;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;


namespace GardenHose.Game.World.Entities.Physical;

/* Class servers as a way to specify a part's sprites and their properties without having to have the sprites loaded. */
internal class PhysicalEntityPartSprite
{
    // Internal fields.
    [MemberNotNull(nameof(_sprite))]
    internal SpriteItem Sprite
    {
        get => _sprite ?? throw new InvalidOperationException("Cannot access asset since it hasn't been loaded yet.");
        private set => _sprite = value;
    }

    internal Vector2 Offset { get; set; } = Vector2.Zero;

    internal float Rotation { get; set; } = 0f;

    internal Vector2 Scale { get; set; } = Vector2.One;

    internal Color ColorMask
    {
        get
        {
            if (_sprite != null)
            {
                return Sprite.Mask;
            }

            return _colorMask;
        }
        set
        {
            if (_sprite != null)
            {
                Sprite.Mask = value;
            }
            else
            {
                _colorMask = value;
            }
        }
    }

    internal float Opacity
    {
        get
        {
            if (_sprite != null)
            {
                return Sprite.Opacity;
            }

            return _opacity;
        }
        set
        {
            if (_sprite != null)
            {
                Sprite.Opacity = value;
            }
            else
            {
                _opacity = value;
            }
        }
    }

    internal float Brightness
    {
        get
        {
            if (_sprite != null)
            {
                return Sprite.Brightness;
            }

            return _brightness;
        }
        set
        {
            if (_sprite != null)
            {
                Sprite.Brightness = value;
            }
            else
            {
                _brightness = value;
            }
        }
    }

    internal SpriteEffects Effects 
    {
        get
        {
            if (_sprite != null)
            {
                return Sprite.Effects;
            }

            return _effects;
        }
        set
        {
            if (_sprite != null)
            {
                Sprite.Effects = value;
            }
            else
            {
                _effects = value;
            }
        }
    }

    internal string AssetName { get; private init; }


    // Private fields.
    private SpriteItem _sprite;

    private SpriteEffects _effects = SpriteEffects.None;
    private float _brightness = 1f;
    private float _opacity = 1f;
    private Color _colorMask = Color.White;
    

    // Constructors.
    internal PhysicalEntityPartSprite(string assetName)
    {
        AssetName = assetName ?? throw new ArgumentNullException(nameof(assetName));
    }

    internal PhysicalEntityPartSprite(SpriteAnimation animation) : this(animation.CreateInstance()) { }

    internal PhysicalEntityPartSprite(AnimationInstance animationInstance)
    {
        AssetName = string.Empty;
        Sprite = new(animationInstance);
    }


    // Internal methods.
    internal void Draw(GameWorld world, Vector2 position, float rotation)
    {
        Sprite.Position.Vector = world.ToViewportPosition(position + Offset);
        Sprite.Scale.Vector = Scale * world.Zoom;
        Sprite.Rotation = Rotation + rotation;
        Sprite.Draw();
    }

    internal void Load(GHGameAssetManager assetManager)
    {
        if (_sprite != null)
        {
            return;
        }

        Sprite = new(assetManager.GetAnimation(AssetName));

        if (Sprite == null)
        {
            throw new InvalidOperationException($"Asset \"{AssetName}\" couldn't be loaded since it wasn't found.");
        }

        Sprite.Mask = _colorMask;
        Sprite.Opacity = _opacity;
        Sprite.Brightness = _brightness;
        Sprite.Effects = _effects;
    }
}