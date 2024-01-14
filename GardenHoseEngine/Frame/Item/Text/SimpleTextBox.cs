using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Text;

/* Called "simple" because of plans to make a more complex and complete text box. */
public class SimpleTextBox : ColoredItem
{
    // Fields.
    [MemberNotNull(nameof(RealText))]
    [MemberNotNull(nameof(FormattedText))]
    public string Text
    {
        get => RealText;
        set
        {
            RealText = value ?? throw new ArgumentNullException();
            FormatText();
        }
    }

    [MemberNotNull(nameof(_font))]
    public SpriteFont Font
    {
        get => _font;
        set
        {
            _font = value ?? throw new ArgumentNullException(nameof(value));
            FormatText();
        }
    }

    public Vector2 MaxSize
    {
        get => _maxSize;
        set
        {
            _maxSize = value;
            FormatText();
        }
    }

    public int MaxCharacters
    {
        get => _maxCharacters;
        set
        {
            _maxCharacters = value;
            FormatText();
        }
    }

    public override Color Mask 
    { 
        get => base.Mask; 
        set
        {
            base.Mask = value;
            if (!_isShadowColorSet)
            {
                SetShadowColorToDefault();
            }
        }
    }

    public bool IsShadowEnabled { get; set; } = false;

    public float ShadowOffset { get; set; } = 0.05f;

    public Color? ShadowColor
    {
        get => _isShadowColorSet ? null : _shadowColor;
        set
        {
            _isShadowColorSet = value.HasValue;

            if (_isShadowColorSet)
            {
                _shadowColor = value.Value;
            }
            else
            {
                SetShadowColorToDefault();
            }
        }
    }

    public Origin Origin
    {
        get => _origin;
        set
        {
            _origin = value;
            Vector2 NewOrigin = Vector2.Zero;
            Vector2 TextSize = Font.MeasureString(FormattedText);

            NewOrigin.X = ((int)value % 3) switch
            {
                0 => 0f,
                1 => TextSize.X / 2f,
                2 => TextSize.X
            };

            NewOrigin.Y = ((int)value / 3) switch
            {
                0 => 0f,
                1 => TextSize.Y / 2f,
                2 => TextSize.Y
            };

            _vectorOrigin = NewOrigin;
        }
    }

    public Vector2 VectorOrigin => _vectorOrigin;

    public new Vector2 Scale
    {
        get => base.Scale.Vector;
        set
        {
            base.Scale.Vector = value;
            UpdateVectorOrigin();
        }
    }

    public AnimVector2 AnimVectorScale => base.Scale;


    public Vector2 RealPixelSize { get; private set; }

    


    // Protected fields.
    protected string RealText;
    protected string FormattedText;


    // Private fields.
    private SpriteFont _font;
    private int _maxCharacters = int.MaxValue;
    private Vector2 _maxSize = new(float.PositiveInfinity, float.PositiveInfinity);

    private bool _isShadowColorSet = false;
    private Color _shadowColor;

    private Origin _origin;
    private Vector2 _vectorOrigin;


    // Constructors.
    public SimpleTextBox(SpriteFont font, string text) : base()
    {
        _font = font ?? throw new ArgumentNullException(nameof(font));
        Text = text;
        ShadowColor = null;
    }


    // Protected methods.
    protected virtual void FormatText()
    {
        if (MaxCharacters < 1)
        {
            FormattedText = string.Empty;
            return;
        }

        string Text = RealText;
        if (Text.Length > MaxCharacters)
        {
            Text = Text[0..MaxCharacters];
            RealText = Text;
        }

        if ((MaxSize.X == float.PositiveInfinity) && (MaxSize.Y == float.PositiveInfinity))
        {
            FormattedText = RealText;
        }
        else
        {
            FormattedText = string.Join('\n', FormatTextBySize(Text));
        }
        RealPixelSize = Font.MeasureString(FormattedText);

        UpdateVectorOrigin();
    }

    // Private methods.
    private IEnumerable<string> FormatTextBySize(string text)
    {
        // Exit if text cannot be formatted.
        if (!text.Contains(' ')) return text.Split('\n');

        if (MaxSize.X <= 0f || MaxSize.Y <= 0f)
        {
            FormattedText = string.Empty;
            return Array.Empty<string>();
        }


        // Format each line by size with the power of unreadable code.
        string[] UnformattedTextLines = text.Split('\n');
        List<string> FormattedTextLines = new(text.Length * 2);
        float TextHeight = 0f;

        foreach (string Line in UnformattedTextLines)
        {
            // Single line.
            TextHeight += Font.LineSpacing;
            if (TextHeight > MaxSize.Y) break;

            float LineLength = Font.MeasureString(Line).X;
            if (LineLength <= MaxSize.X)
            {
                FormattedTextLines.Add(Line);
                continue;
            }

            // Multi-line.
            string[] Lines = FormatLineBySize(Line);
            int LinesAdded = 0;
            for (int i = 0; i < Lines.Length; i++, LinesAdded++)
            {
                TextHeight += Font.LineSpacing;
                if (TextHeight > MaxSize.Y) break;

                FormattedTextLines.Add(Lines[i]);
            }

            if (LinesAdded < Lines.Length)
            {
                break;
            }
        }

        return FormattedTextLines;
    }

    private string[] FormatLineBySize(string line)
    {
        if (line == string.Empty)
        {
            return new string[] { string.Empty };
        }

        StringBuilder FormattedLine = new(line);

        int LineStartIndex = 0;
        int? PreviousWSIndex = null;
        float LineLength = 0f;

        // Format line.
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] != ' ') continue;

            LineLength = Font.MeasureString(line[LineStartIndex..(i + 1)]).X;

            if ((LineLength > MaxSize.X) && (PreviousWSIndex.HasValue))
            {
                FormattedLine[PreviousWSIndex.Value] = '\n';
                LineStartIndex = PreviousWSIndex.Value + 1;
            }

            PreviousWSIndex = i;
        }

        // Final format if needed.
        LineLength = Font.MeasureString(line[LineStartIndex..]).X;

        if ((LineLength > MaxSize.X) && (PreviousWSIndex.HasValue))
        {
            FormattedLine[PreviousWSIndex.Value] = '\n';
        }

        return FormattedLine.ToString().Split('\n');
    }

    private void SetShadowColorToDefault()
    {
        _shadowColor = new(
            (int)(CombinedMask.R * 0.4f),
            (int)(CombinedMask.G * 0.4f),
            (int)(CombinedMask.B * 0.4f),
            (CombinedMask.A));
    }

    private void UpdateVectorOrigin()
    {
        Vector2 TextSize = RealPixelSize;

        _vectorOrigin.X = ((int)_origin % 3) switch
        {
            0 => 0f,
            1 => TextSize.X / 2f,
            2 => TextSize.X
        };

        _vectorOrigin.Y = ((int)_origin / 3) switch
        {
            0 => 0f,
            1 => TextSize.Y / 2f,
            2 => TextSize.Y
        };
    }


    // Inherited methods.
    public override void Draw()
    {
        if (!IsVisible) return;

        if (IsShadowEnabled)
        {
            GameFrameManager.SpriteBatch.DrawString(Font,
            FormattedText,
            Display.ToRealPosition(Position.Vector +
            new Vector2(Font.LineSpacing, Font.LineSpacing) * ShadowOffset * Scale),
            _shadowColor,
            Rotation,
            VectorOrigin,
            Display.ToRealScale(Scale),
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
        }

        GameFrameManager.SpriteBatch.DrawString(Font,
            FormattedText,
            Display.ToRealPosition(Position),
            CombinedMask,
            Rotation,
            VectorOrigin,
            Display.ToRealScale(Scale),
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }
}
