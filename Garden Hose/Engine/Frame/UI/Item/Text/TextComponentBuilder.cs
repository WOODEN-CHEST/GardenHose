using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace GardenHose.Engine.Frame.UI.Item;

public class TextComponentBuilder
{
    // Fields.
    public readonly List<TextComponent> Components = new();


    // Constructors
    public TextComponentBuilder(string text, DynamicFont font)
    {
        Components.Add(new TextComponent(text, font));
        
    }


    // Methods.
    public TextComponentBuilder Append(string text)
    {
        Components.Add(new(text, Components[^1].TextFont));
        return this;
    }

    public TextComponentBuilder Color(Color color)
    {
        Components[^1].FullColor = color;
        return this;
    }

    public TextComponentBuilder Tint(Color color)
    {
        Components[^1].Tint = color;
        return this;
    }

    public TextComponentBuilder Brightness(float value)
    {
        Components[^1].Brightness = value;
        return this;
    }

    public TextComponentBuilder Opacity(float value)
    {
        Components[^1].Opacity = value;
        return this;
    }

    public TextComponentBuilder Visibility(bool isVisible)
    {
        Components[^1].IsVisible = isVisible;
        return this;
    }

    public TextComponentBuilder Font(DynamicFont font)
    {
        Components[^1].TextFont = font;
        return this;
    }

    public TextComponent[] Build() => Components.ToArray();
}