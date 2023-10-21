using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace GardenHoseEngine.Frame.Item.Buttons;


public class SpriteButton : IDrawableItem, ITimeUpdatable
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

    public bool IsVisible
    {
        get => Sprite.IsVisible; 
        set => Sprite.IsVisible = value;
    }

    public Effect? Shader
    {
        get => Sprite.Shader;
        set => Sprite.Shader = value;
    }


    // Constructors.
    public SpriteButton(AnimationInstance animationInstance, params IButtonComponent[] buttonComponents)
    {
        Button = new(buttonComponents);
        Sprite = new(animationInstance);
    }

    public SpriteButton(SpriteAnimation animation, params IButtonComponent[] buttonComponents)
        : this(animation.CreateInstance(), buttonComponents) { }

    public SpriteButton(Button button, SpriteItem item)
    {
        Button = button ?? throw new ArgumentNullException(nameof(button));
        Sprite = item ?? throw new ArgumentNullException(nameof(item));
    }


    // Inherited methods.
    public void Draw()
    {
        Sprite.Draw();
    }

    public void Update()
    {
        Button.Update();
        Sprite.Update();
    }
}