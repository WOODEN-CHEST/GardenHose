using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Text;

public class TextComponent
{
    // Fields.
    public string Text { get; set; }

    public int Length => Text.Length;

    public Vector2 PixelSize => Font.MeasureString(Text);

    public Color Color { get; set; }

    public DynamicFont Font { get; set; }


    // Constructors.
    public TextComponent(string text, DynamicFont font, Color color)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Font = font ?? throw new ArgumentNullException(nameof(font));
        Color = color;
    }
}