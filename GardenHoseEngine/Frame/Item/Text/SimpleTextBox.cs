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
    public DynamicFont Font
    {
        get => _font;
        set
        {
            _font = value ?? throw new ArgumentNullException(nameof(value));
            FormatText();
        }
    }

    public Vector2 MaxSize { get; set; } = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

    public int MaxCharacters { get; set; } = int.MaxValue;

    public Vector2 RealPixelSize { get; private set; }


    // Protected fields.
    protected string RealText;
    protected string FormattedText;


    // Private fields.
    private DynamicFont _font;


    // Constructors.
    public SimpleTextBox(ITimeUpdater updater, IVirtualConverter converter, IDrawer drawer, DynamicFont font, string text) 
        : base(updater, converter, drawer)
    {
        _font = font ?? throw new ArgumentNullException(nameof(font));
        Text = text;
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
        }

        FormattedText = string.Join('\n', FormatTextBySize(Text));
        RealPixelSize = Font.MeasureString(FormattedText);
    }

    // Private methods.
    private IEnumerable<string> FormatTextBySize(string text)
    {
        // Exit if text cannot be formatted.
        if (!text.Contains(' ')) return Array.Empty<string>();

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
            TextHeight += Font.SpriteFont.LineSpacing;
            if (TextHeight > MaxSize.Y) break;

            if (Font.MeasureString(Line).X <= MaxSize.X)
            {
                FormattedTextLines.Add(Line);
                continue;
            }

            string[] Lines = FormatLineBySize(Line);
            int LinesAdded = 0;
            for (int i = 0; i < Lines.Length; i++, LinesAdded++)
            {
                TextHeight += Font.SpriteFont.LineSpacing;
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


    // Inherited methods.
    public override void Draw(TimeSpan passedTime, SpriteBatch spriteBatch)
    {
        if (!IsVisible) return;

        spriteBatch.DrawString(Font,
            FormattedText,
            Converter.ToRealPosition(Position),
            CombinedMask,
            Rotation,
            Origin,
            Converter.ToRealScale(Scale),
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }

    public override void SetOrigin(Origin origin)
    {
        Vector2 NewOrigin = Vector2.Zero;
        Vector2 TextSize =Font.MeasureString(FormattedText);

        NewOrigin.X = ((int)origin % 3) switch
        {
            0 => 0f,
            1 => TextSize.X / 2f,
            2 => TextSize.X
        };

        NewOrigin.Y = ((int)origin / 3) switch
        {
            0 => 0f,
            1 => TextSize.Y / 2f,
            2 => TextSize.Y
        };

        SetOrigin(NewOrigin);
    }
}
