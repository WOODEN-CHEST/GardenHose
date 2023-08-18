using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.Frame.UI.Item;

/* Represents a simple text box.  */
public class TextBox : TextComponent
{
    // Fields.
    public override string Text
    { 
        get => base.Text;
        set
        {

            if (value.Length > MaxCharacters) value = value[..MaxCharacters];
            if (_isTextWrapped) value = LimitTextByPixelSize(value);

            base.Text = value;
        }
    }

    public Vector2 MaxSizePixels
    {
        get => _maxSizePixels;
        set
        {
            _maxSizePixels.X = MathF.Min(1f, value.X);
            _maxSizePixels.Y = MathF.Min(1f, value.Y);
        }
    }

    public int MaxCharacters
    {
        get => _maxCharacters;
        set
        {
            _maxCharacters = Math.Max(0, value);
        }
    }

    public bool IsTextWrapped
    {
        get => _isTextWrapped;
        set
        {
            _isTextWrapped = value;
        }
    }


    // Private fields.
    private Vector2 _maxSizePixels = new(300f, 300f);
    private int _maxCharacters = int.MaxValue;
    private bool _isTextWrapped = true;

    private string _realText;

    // Constructors.
    public TextBox(string text, DynamicFont font) : this(text, font, Color.White) { }

    public TextBox(string text, DynamicFont font, Color color) : base(text, font, color) { }


    // Methods.


    // Private methods.
    private string LimitTextByPixelSize(string text)
    {
        StringBuilder NewText = new(text);
        int StartIndex = 0;
        int SpaceIndex = -1;
        Vector2 LineSize;
        float TextHeight = 0f;
        char Letter;

        for (int Index = 0; Index < NewText.Length; Index++)
        {
            Letter = NewText[Index];

            if (Letter is ' ' or '\t') SpaceIndex = Index;

            LineSize = TextFont.MeasureString(text[(StartIndex)..Index]);



            if ((LineSize.X > MaxSizePixels.X) && (SpaceIndex != -1))
            {
                NewText[SpaceIndex] = '\n';
                StartIndex = SpaceIndex + 1;
                SpaceIndex = -1;
                TextHeight += LineSize.Y;
                continue;
            }


            if (Letter == '\n')
            {
                StartIndex = SpaceIndex + 1;
                SpaceIndex = -1;
                TextHeight += LineSize.Y;
                continue;
            }
        }

        return NewText.ToString();
    }

    private void UpdateText()
    {

    }


    // Inherited methods.
    
}