using GardenHose.Engine.Frame.UI.Animation;
using GardenHose.Engine.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.Intrinsics.X86;
using System.Text;


namespace GardenHose.Engine.Frame.UI.Item;

public class TextBox : DrawableItem
{
    // Fields.
    public bool IsTypeable
    {
        get => IsTypeable;
        set
        {
            _isTypeable = value;
            if (_isTypeable) MainGame.Instance.Window.TextInput += OnUserType;
            else MainGame.Instance.Window.TextInput -= OnUserType;
        }
    }

    public bool PreventTypingOverflow = false;
    public bool AllowNewlineTyping = true;
    public uint MaxCharacters = uint.MaxValue;

    public string Text
    {
        get { return _realText; }
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            _realText = value;

            _wrappedText = value;
            if (WrapText) FoldText();
        }
    }

    public float MaxPixelsPerLine
    {
        get => _maxPixelsPerLine;
        set => _maxPixelsPerLine = Math.Max(1, value);
    }

    public bool WrapText = true;

    public DynamicFont Font
    {
        get => _font;
        set => _font = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Vector2 TextOrigin = new Vector2();

    public Vector2 TextSizePixels { get => _font.FontAsset.MeasureString(_wrappedText); }


    // Private fields.
    private string _wrappedText;
    private string _realText;

    private float _maxPixelsPerLine;

    private DynamicFont _font;

    private bool _isTypeable;


    // Constructors.
    public TextBox(
        Vector2 position,
        Vector2 scale,
        float rotation,

        DynamicFont font,
        string text,
        Color color,
        float maxPixelsPerLine)
        : base(position, scale, rotation)
    {
        Font = font;
        MaxPixelsPerLine = maxPixelsPerLine;
        Text = text;
        Tint = color;
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
    private void FoldText()
    {
        StringBuilder NewText = new(_wrappedText);
        int StartIndex = 0; // Start of sentence.
        int EndIndex = 0; // End of sentence.
        int SpaceIndex = -1; // Index of last found space or tab. -1 means it doesn't exist.
        char Letter;
        float PartLength;

        for (int Index = 0; (Index < _wrappedText.Length) && (Index <= MaxCharacters); Index++, EndIndex++)
        {
            Letter = _wrappedText[Index];

            if (Letter is ' ' or '\t') SpaceIndex = Index;
            else if (Letter == '\n')
            {
                SpaceIndex = -1;
                StartIndex = Index + 1;
                EndIndex = StartIndex;
                continue;
            }

            PartLength = Font.FontAsset.MeasureString(_wrappedText.Substring(StartIndex, EndIndex - StartIndex)).X;
            if ((PartLength > MaxPixelsPerLine) && (SpaceIndex != -1))
            {
                NewText[SpaceIndex] = '\n';

                StartIndex = SpaceIndex + 1;
                EndIndex = Index;
                SpaceIndex = -1;
            }
        }

        _wrappedText = NewText.ToString();
    }


    // Inherited methods.
    public override void Draw()
    {
        base.Draw();

        if (IsVisible) GameFrame.DrawBatch.DrawString(
            _font.FontAsset,
            _wrappedText,
            RealPosition,
            RealColorMask,
            Rotation,
            TextOrigin,
            RealScale,
            SpriteEffects.None,
            1f);
    }


    // Private methods.
    private void OnUserType(object sender, TextInputEventArgs args)
    {
        if (args.Key == Keys.Back)
        {
            Text = Text.Length == 0 ? String.Empty : _realText.Substring(0, _realText.Length - 1);
        }

        if (_realText.Length >= MaxCharacters) return;

        string Typed = null;
        if (args.Character is >= ' ' and <= '~')
        {
            Typed = args.Character.ToString();
        }
        else if (args.Key == Keys.Enter)
        {
            if (AllowNewlineTyping) Typed = "\n";
        }
        else if (args.Key == Keys.Tab) Typed = "    ";
        else
        {
            Logger.Warning($"Couldn't resolve pressed key for {nameof(TextBox)}" +
                    $"\nKey pressed: \"{args.Key}\", Character: \"{args.Character}\", Font: \"{Font.Name}\"");
            return;
        }

        if (PreventTypingOverflow &&
            (Font.FontAsset.MeasureString(Typed).X 
            + Font.FontAsset.MeasureString(_realText).X
            > _maxPixelsPerLine)) return;

        Text = _realText + Typed;
    }
}
