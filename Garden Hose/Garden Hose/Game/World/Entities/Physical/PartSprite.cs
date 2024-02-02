using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Player;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;


namespace GardenHose.Game.World.Entities.Physical;

/* Class servers as a way to specify a part's sprites and their properties without having to have the sprites loaded. */
internal class PartSprite
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
    internal Vector2 Size { get; set; } = new Vector2(50f);

    internal Color ColorMask
    {
        get
        {
            if (_sprite != null)
            {
                return Sprite.Mask;
            }

            return _colorMask.Mask;
        }
        set
        {
            if (_sprite != null)
            {
                Sprite.Mask = value;
            }
            else
            {
                _colorMask.Mask = value;
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

            return _colorMask.Opacity;
        }
        set
        {
            if (_sprite != null)
            {
                Sprite.Opacity = value;
            }
            else
            {
                _colorMask.Opacity = value;
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

            return _colorMask.Brightness;
        }
        set
        {
            if (_sprite != null)
            {
                Sprite.Brightness = value;
            }
            else
            {
                _colorMask.Brightness = value;
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

    internal GameAnimationName AnimationName { get; private init; }


    // Private fields.
    private SpriteItem _sprite;
    ColorMask _colorMask = new();
    private SpriteEffects _effects = SpriteEffects.None;
    

    // Constructors.
    internal PartSprite(GameAnimationName animationName)
    {
        AnimationName = animationName;
    }


    // Internal methods.
    internal void Draw(IDrawInfo info, IWorldCamera camera, PhysicalEntityPart part)
    {
        Sprite.Position = camera.ToViewportPosition(part.Position + Offset);
        Sprite.Size = Size * camera.Zoom;
        Sprite.Rotation = Rotation + part.CombinedRotation;
        Sprite.Draw(info);
    }

    internal void Load(GHGameAssetManager assetManager)
    {
        if (_sprite != null)
        {
            return;
        }

        Sprite = new(assetManager.GetAnimation(AnimationName).CreateInstance());

        if (Sprite == null)
        {
            throw new InvalidOperationException($"Asset \"{AnimationName}\" couldn't be loaded since it wasn't found.");
        }

        Sprite.Mask = _colorMask.Mask;
        Sprite.Opacity = _colorMask.Opacity;
        Sprite.Brightness = _colorMask.Brightness;
        Sprite.Effects = _effects;
    }
}