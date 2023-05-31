using GardenHose.Engine.Frame.UI.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;


namespace GardenHose.Engine.Frame.UI.Item;

public class TextBox : DrawableItem
{
    // Fields.
    public Color TextColor;

    public string Text
    {
        get { return _text; }
        set
        {
            // Invalid value fixes and early exits.
            if (value == null) value = "";
            else if (value.Contains('\t'))
            {
                throw new ArgumentException("Tabs break the TextBox's wrapping (visually). Won't fix.");
            }
            _text = value;

            FoldText();
        }
    }

    public int MaxPixelsPerLine
    {
        get => _pixelsPerLine;
        set => _pixelsPerLine = Math.Max(1, value);
    }

    public DynamicFont Font
    {
        get => _font;
        set => _font = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Vector2 RotationOrigin
    {
        get => _rotationOrigin;
        set => _rotationOrigin = value;
    }

    public Vector2 TextSizePixels { get => _font.FontSprite.MeasureString(_text); }


    // Private fields.
    private string _text;
    private int _pixelsPerLine;
    private DynamicFont _font;
    private Vector2 _rotationOrigin;


    // Constructors.
    public TextBox(
        Vector2 position,
        Vector2 scale,
        float rotation,

        DynamicFont font,
        string text,
        Color color,
        int maxPixelsPerLine)
        : base (position, scale, rotation)
    {
        Font = font;
        Text = text;
        TextColor = color;
        MaxPixelsPerLine = maxPixelsPerLine;
    }


    // Methods.
    public void SetTextureOrigin(TextureOrigin origin)
    {
        _rotationOrigin.X = ((int)origin % 3) switch
        {
            0 => 0f,
            1 => TextSizePixels.X / 2f,
            2 => TextSizePixels.X,
            _ => 0f
        };

        _rotationOrigin.Y = ((int)origin / 3) switch
        {
            0 => 0f,
            1 => TextSizePixels.Y / 2f,
            2 => TextSizePixels.Y,
            _ => 0f
        };
    }


    // Private methods.
    private void FoldText()
    {
        if (!_text.Contains(' ')) return;

        // Fully wrapping the text.
        StringBuilder Text = new(_text);
        int i = 0;
        int LineStartIndex = 0;
        int PixelsInLine = 0;
        bool CurLineHasSpace = false;

        for (; i < Text.Length; i++)
        {
            // Set info for line.
            PixelsInLine = (int)Font.FontSprite.MeasureString(
                _text.Substring(LineStartIndex, i - LineStartIndex)).X;
            if (!CurLineHasSpace && Text[i] == ' ') CurLineHasSpace = true;


            // Logic.
            if (PixelsInLine > MaxPixelsPerLine)
            {
                if (CurLineHasSpace) ReplacePrevSpace();
                else ReplaceNextSpace();
                i++;

                LineStartIndex = i;
                CurLineHasSpace = false;
            }

        }

        _text = Text.ToString();


        // Local functions.
        void ReplaceNextSpace()
        {
            for (; i < Text.Length; i++)
            {
                if (Text[i] != ' ') continue;
                Text[i] = '\n';
                break;
            }
        }

        void ReplacePrevSpace()
        {
            for (; i > 0; i--)
            {
                if (Text[i] != ' ') continue;
                Text[i] = '\n';
                break;
            }
        }
    }


    // Inherited methods.
    public override void Draw()
    {
        if (IsVisible) GameFrame.DrawBatch.DrawString(
            _font.FontSprite,
            _text,
            RealPosition,
            TextColor,
            Rotation,
            _rotationOrigin,
            RealScale,
            SpriteEffects.None,
            1f);
    }
}