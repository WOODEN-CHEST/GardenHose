using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        Components[^1].Tint = color;
        Components[^1].Opacity = color.A / (float)byte.MaxValue;
        return this;
    }

    public TextComponentBuilder Font(DynamicFont font)
    {
        Components[^1].TextFont = font;
        return this;
    }

    public TextComponent[] Build() => Components.ToArray();
}