using GardenHose.Engine.Frame.UI.Animation;
using GardenHose.Engine.Frame.UI.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace GardenHose.Engine.Frame.UI.Item;

public class TextComponent : ColoredItem
{
    // Fields.
    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? throw new ArgumentNullException(nameof(value));
            UpdateTextSize();
        }
    }

    public DynamicFont TextFont
    {
        get => _textFont;
        set
        {
            _textFont = value ?? throw new ArgumentNullException(nameof(value));
            UpdateTextSize();
        }
    }

    public Vector2 TextSizePixels { get => _textSizePixels; }
    public Vector2 TextOrigin = new();


    // Private fields.
    private string _text;
    private DynamicFont _textFont;
    private Vector2 _textSizePixels;


    // Constructors.
    public TextComponent(string text, DynamicFont font)
    {
        Text = text;
        TextFont = font;
    }


    // Methods.
    public void SetTextOrigin(TextureOrigin origin)
    {
        TextOrigin.X = ((int)origin % 3) switch
        {
            0 => 0f,
            1 => TextSizePixels.X / 2f,
            2 => TextSizePixels.X,
            _ => 0f
        };

        TextOrigin.Y = ((int)origin / 3) switch
        {
            0 => 0f,
            1 => TextSizePixels.Y / 2f,
            2 => TextSizePixels.Y,
            _ => 0f
        };
    }


    // Private methods.
    private void UpdateTextSize() => _textSizePixels = _textFont.MeasureString(_text);


    // Inherited methods.
    public override void Draw()
    {
        base.Draw();
        if (!ShouldRender) return;

        GameFrame.DrawBatch.DrawString(
            _textFont.FontAsset,
            _text,
            RealPosition,
            RealColorMask,
            Rotation,
            TextOrigin,
            RealScale,
            SpriteEffects.None,
            1f);
    }
}