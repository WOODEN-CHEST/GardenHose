using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace GardenHoseEngine.Frame.Item.Buttons;


public class SpriteButton
{
    // Fields.
    public Button Button { get; init; }

    public SpriteItem Sprite { get; init; }

    public Vector2 Position
    {
        get => Sprite.Position;
        set
        {
            Sprite.Position.Vector = value;
            Button.Position = value;
        }
    }

    public float Rotation
    {
        get => Sprite.Rotation;
        set => Sprite.Rotation = value;
    }

    public Vector2 Scale
    {
        get => Sprite.Scale;
        set
        {
            Sprite.Scale.Vector = value;
            Button.Scale = value;
        }
    }


    // Constructors.
    public SpriteButton(ITimeUpdater updater, IVirtualConverter converter, IDrawer drawer,
        AnimationInstance animationInstance, UserInput input, params IButtonComponent[] buttonComponents)
    {
        Button = new(input, updater, buttonComponents);
        Sprite = new(updater, converter, drawer, animationInstance);
    }

    public SpriteButton(ITimeUpdater updater, IVirtualConverter converter, IDrawer drawer,
        SpriteAnimation animation, UserInput input, params IButtonComponent[] buttonComponents)
    {
        Button = new(input, updater, buttonComponents);
        Sprite = new(updater, converter, drawer, animation);
    }

    public SpriteButton(Button button, SpriteItem item)
    {
        Button = button ?? throw new ArgumentNullException(nameof(button));
        Sprite = item ?? throw new ArgumentNullException(nameof(item));
    }
}