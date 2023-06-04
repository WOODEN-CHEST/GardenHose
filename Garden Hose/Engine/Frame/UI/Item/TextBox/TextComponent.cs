using GardenHose.Engine.Frame.UI.Item;
using Microsoft.Xna.Framework;
using 
using System;


namespace GardenHose.Engine.Frame.UI.Item;

public class TextComponent : DrawableItem
{
    // Fields.
    public TextComponent NextComponent;

    public string Text
    {
        get => _text;
        set => _text = value ?? throw new ArgumentNullException(nameof(value));
    }

    public DynamicFont TextFont
    {
        get => _textFont;
        set => _textFont = value ?? throw new ArgumentNullException(nameof(value));
    }


    // Private fields.
    private string _text;
    private DynamicFont _textFont;


    // Constructors.
    public TextComponent(string text, DynamicFont font)
    {
        Text = text;
        TextFont = font;
    }


    // Methods.
    public TextComponent AddComponent(string text) => new(text, TextFont);

    public TextComponent SetFont(DynamicFont font)
    {
        TextFont = font;
        return this;
    }

    public TextComponent SetColor(Color color)
    {
        Tint = color;
        return this;
    }


    // Inherited methods.
    public override void Draw()
    {
        base.Draw();
        NextComponent?.Draw();
        if (!ShouldRender) return;
    }
}