
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Text;

internal class FormattedTextComponent
{
    // Fields.
    public string Text { get; set; }

    public Color Color { get; set; } = Color.White;

    public Vector2 Offset { get; set; }

    public DynamicFont Font { get; set; }


    // Constructors.
    public FormattedTextComponent(string text, Color color, Vector2 offset, DynamicFont font)
    {
        Text = text;
        Color = color;
        Offset = offset;
        Font = font;
    }
}