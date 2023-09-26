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
    public SpriteButton(IVirtualConverter converter, AnimationInstance animationInstance, 
        UserInput input, params IButtonComponent[] buttonComponents)
    {
        Button = new(input, buttonComponents);
        Sprite = new(converter, animationInstance);
    }

    public SpriteButton(IVirtualConverter converter, SpriteAnimation animation,
        UserInput input, params IButtonComponent[] buttonComponents)
        : this(converter, animation.CreateInstance(), input, buttonComponents) { }

    public SpriteButton(Button button, SpriteItem item)
    {
        Button = button ?? throw new ArgumentNullException(nameof(button));
        Sprite = item ?? throw new ArgumentNullException(nameof(item));
    }


    // Inherited methods.
    public void Draw(float passedTimeSeconds, SpriteBatch spriteBatch)
    {
        Sprite.Draw(passedTimeSeconds, spriteBatch);
    }

    public void Update(float passedTimeSeconds)
    {
        Button.Update(passedTimeSeconds);
        Sprite.Update(passedTimeSeconds);
    }
}