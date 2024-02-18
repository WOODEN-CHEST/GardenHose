using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Material;
using GardenHose.Game.World.Player;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GardenHose.Game.World.Entities.Physical;


internal class PartSprite
{
    // Internal fields.
    internal GHGameAnimationName UndamagedName { get; private init; }
    internal GHGameAnimationName SlightlyDamagedName { get; private init; }
    internal GHGameAnimationName DamagedName { get; private init; }
    internal GHGameAnimationName HeavilyDamagedName { get; private init; }
    internal Vector2 Offset { get; set; } = Vector2.Zero;
    internal float Rotation { get; set; } = 0f;
    internal Vector2 Size { get; set; } = new Vector2(50f);

    internal Color ColorMask
    {
        get => _colorMask.Mask;
        set
        {
            _colorMask.Mask = value;
            ApplyColorMask();
        }
    }

    internal float Opacity
    {
        get => _colorMask.Opacity;
        set
        {
            _colorMask.Opacity = value;
            ApplyOpacity();
        }
    }

    internal float Brightness
    {
        get => _colorMask.Brightness;
        set
        {
             _colorMask.Brightness = value;
             ApplyBrightness();
        }
    }

    internal SpriteEffects Effects 
    {
        get => _effects;
        set
        {
            _effects = value;
            ApplyEffects();
        }
    }


    // Private fields.
    private SpriteItem _undamagedSprite;
    private SpriteItem _slightlyDamagedSprite;
    private SpriteItem _damagedSprite;
    private SpriteItem _heavilyDamagedSprite;
    private SpriteItem _activeSprite;



    ColorMask _colorMask = new();
    private SpriteEffects _effects = SpriteEffects.None;


    // Constructors.
    internal PartSprite(GHGameAnimationName allSpriteNames) : this(allSpriteNames, allSpriteNames, allSpriteNames, allSpriteNames) { }

    internal PartSprite(GHGameAnimationName undamagedName,
        GHGameAnimationName slightlyDamagedName,
        GHGameAnimationName damagedName,
        GHGameAnimationName heavilyDamagedName)
    {
        UndamagedName = undamagedName;
        SlightlyDamagedName = slightlyDamagedName;
        DamagedName = damagedName;
        HeavilyDamagedName = heavilyDamagedName;
    }


    // Internal methods.
    internal void Draw(IDrawInfo info, IWorldCamera camera, PhysicalEntityPart part)
    {
        _activeSprite.Position = camera.ToViewportPosition(part.Position + Offset);
        _activeSprite.Size = Size * camera.Zoom;
        _activeSprite.Rotation = Rotation + part.CombinedRotation;
        _activeSprite.Draw(info);
    }

    internal void Load(GHGameAssetManager assetManager)
    {
        if (_undamagedSprite != null)
        {
            return;
        }

        _undamagedSprite = new(assetManager.GetAnimation(UndamagedName).CreateInstance());
        _slightlyDamagedSprite = new(assetManager.GetAnimation(SlightlyDamagedName).CreateInstance());
        _damagedSprite = new(assetManager.GetAnimation(DamagedName).CreateInstance());
        _heavilyDamagedSprite = new(assetManager.GetAnimation(HeavilyDamagedName).CreateInstance());

        ApplyColorMask();
        ApplyOpacity();
        ApplyBrightness();
        ApplyEffects();

        _activeSprite = _undamagedSprite;
    }

    internal void SetActiveSprite(WorldMaterialStage stage)
    {
        _activeSprite = stage switch
        {
            WorldMaterialStage.Undamaged => _undamagedSprite,
            WorldMaterialStage.SlightlyDamaged => _slightlyDamagedSprite,
            WorldMaterialStage.Damaged => _damagedSprite,
            WorldMaterialStage.HeavilyDamaged => _heavilyDamagedSprite,
            _ => throw new EnumValueException(nameof(stage), stage)
        };
    }

    internal PartSprite CreateClone()
    {
        return CloneDataToObject(new PartSprite(UndamagedName, SlightlyDamagedName, DamagedName, HeavilyDamagedName));
    }

    internal PartSprite CloneDataToObject(PartSprite sprite)
    {
        sprite.Offset = Offset;
        sprite.Rotation = Rotation;
        sprite.Size = Size;
        sprite.ColorMask = ColorMask;
        sprite.Opacity = Opacity;
        sprite.Brightness = Brightness;
        sprite.Effects = Effects;
        sprite._undamagedSprite = (SpriteItem)_undamagedSprite.Clone();
        sprite._slightlyDamagedSprite = (SpriteItem)_slightlyDamagedSprite.Clone();
        sprite._damagedSprite = (SpriteItem)_damagedSprite.Clone();
        sprite._heavilyDamagedSprite = (SpriteItem)_heavilyDamagedSprite.Clone();

        if (_activeSprite == _undamagedSprite)
        {
            sprite._activeSprite = sprite._undamagedSprite;
        }
        else if (_activeSprite == _slightlyDamagedSprite)
        {
            sprite._activeSprite = sprite._slightlyDamagedSprite;
        }
        else if (_activeSprite == _damagedSprite)
        {
            sprite._activeSprite = sprite._damagedSprite;
        }
        else if (_activeSprite == _heavilyDamagedSprite)
        {
            sprite._activeSprite = sprite._heavilyDamagedSprite;
        }

        return sprite;
    }


    // Private methods.
    private void ApplyColorMask()
    {
        if (_undamagedSprite != null)
        {
            _undamagedSprite.Mask = _colorMask.Mask;
            _slightlyDamagedSprite.Mask = _colorMask.Mask;
            _damagedSprite.Mask = _colorMask.Mask;
            _heavilyDamagedSprite.Mask = _colorMask.Mask;
        }
    }

    private void ApplyOpacity()
    {
        if (_undamagedSprite != null)
        {
            _undamagedSprite.Opacity = _colorMask.Opacity;
            _slightlyDamagedSprite.Opacity = _colorMask.Opacity;
            _damagedSprite.Opacity = _colorMask.Opacity;
            _heavilyDamagedSprite.Opacity = _colorMask.Opacity;
        }
    }

    private void ApplyBrightness()
    {
        if (_undamagedSprite != null)
        {
            _undamagedSprite.Brightness = _colorMask.Brightness;
            _slightlyDamagedSprite.Brightness = _colorMask.Brightness;
            _damagedSprite.Brightness = _colorMask.Brightness;
            _heavilyDamagedSprite.Brightness = _colorMask.Brightness;
        }
    }

    private void ApplyEffects()
    {
        if (_undamagedSprite != null)
        {
            _undamagedSprite.Effects = _effects;
            _slightlyDamagedSprite.Effects = _effects;
            _damagedSprite.Effects = _effects;
            _heavilyDamagedSprite.Effects = _effects;
        }
    }
}