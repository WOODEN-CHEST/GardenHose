using GardenHose.Game.AssetManager;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;


namespace GardenHose.Game.World.Entities.Physical;


internal class PhysicalEntityPartSprite
{
    // Internal fields.
    [MemberNotNull(nameof(_sprite))]
    internal SpriteItem Sprite
    {
        get => _sprite ?? throw new InvalidOperationException("Cannot access asset since hasn't been loaded yet.");
        set => _sprite = value ?? throw new ArgumentNullException(nameof(value));
    }

    internal Vector2 Offset { get; set; } = Vector2.Zero;

    internal float Rotation { get; set; } = 0f;

    internal Vector2 Scale { get; set; } = Vector2.One;

    internal Color ColorMask
    {
        get => Sprite.Mask;
        set => Sprite.Mask = value;
    }

    internal float Opacity
    {
        get => Sprite.Opacity; 
        set => Sprite.Opacity = value;
    }

    internal float Brightness
    {
        get => Sprite.Brightness; 
        set => Sprite.Brightness = value;
    }

    internal SpriteEffect Effect

    internal string AssetName { get; private init; }


    // Private fields.
    private SpriteItem _sprite;

    private SpriteEffect _effect;
    private float _brightness;
    private float _opacity;
    private Color _colorMask;
    

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
        if (Sprite != null)
        {
            return;
        }

        Sprite = new(assetManager.GetAnimation(AssetName));

        if (Sprite == null)
        {
            throw new InvalidOperationException($"Asset \"{AssetName}\" couldn't be loaded since it wasn't found.");
        }
    }
}