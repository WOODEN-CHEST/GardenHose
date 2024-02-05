using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;


namespace GardenHoseEngine.Frame.Item.Text;


public class FittedText : ColoredItem, ITextItem
{
    // Fields.
    [MemberNotNull(nameof(_text))]
    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? throw new ArgumentNullException(nameof(value));
            UpdateTextProperties();
        }
    }

    [MemberNotNull(nameof(_font))]
    public SpriteFont Font
    {
        get => _font;
        set
        {
            _font = value ?? throw new ArgumentNullException(nameof(_font));
            UpdateTextProperties();
        }
    }

    public Origin TextOrigin
    {
        get => _origin;
        set
        {
            _origin = value;
            UpdateTextProperties();
        }
    }

    public float Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            UpdateTextProperties();
        }
    }

    public SpriteEffects Effects { get; set; }

    public Vector2 FittingSizePixels
    {
        get => _fittingSizePixels;
        set
        {
            _fittingSizePixels = value;
            UpdateTextProperties();
        }
    }

    public Vector2 PixelSize => _font.MeasureString(Text);
    public bool IsShadowEnabled {  get; set; } = false;
    public Color ShadowColor { get; set; } = Color.Black;

    public Vector2 ShadowOffset
    {
        get => _shadowOffset;
        set
        {
            _shadowOffset = value;
            UpdateTextProperties();
        }
    }


    // Private fields.
    private string _text;
    private SpriteFont _font;

    private Origin _origin;
    private Vector2 _originLocation;

    private Vector2 _fittingSizePixels = new Vector2(float.MaxValue, float.MaxValue);
    private float _scale = 1f;
    private float _finalTextSize = 1f;

    private Vector2 _shadowOffset = new(0.15f, 0.15f);
    private Vector2 _finalShadowOffset = new(0.15f, 0.15f);


    // Constructors.
    public FittedText(string text, SpriteFont font)
    {
        _font = font ?? throw new ArgumentNullException(nameof(font));
        Text = text;
    }


    // Private methods.
    private void UpdateTextProperties()
    {
        Vector2 TextSizePixels = PixelSize;
        _originLocation = TextOrigin switch
        {
            Origin.TopLeft => Vector2.Zero,
            Origin.TopMiddle => new Vector2(TextSizePixels.X * 0.5f, 0f),
            Origin.TopRight => new Vector2(TextSizePixels.X, 0f),
            Origin.CenterLeft => new Vector2(0f, TextSizePixels.Y * 0.5f),
            Origin.Center => new Vector2(TextSizePixels.X * 0.5f, TextSizePixels.Y * 0.5f),
            Origin.CenterRight => new Vector2(TextSizePixels.X, TextSizePixels.Y * 0.5f),
            Origin.BottomLeft => new Vector2(0f, TextSizePixels.Y),
            Origin.BottomMiddle => new Vector2(TextSizePixels.X * 0.5f, TextSizePixels.Y),
            Origin.BottomRight => new Vector2(TextSizePixels.X, TextSizePixels.Y),
            _ => throw new EnumValueException(nameof(TextOrigin), TextOrigin),
        };

        _finalTextSize = Math.Min(Scale, Math.Min(FittingSizePixels.X / TextSizePixels.X, FittingSizePixels.Y / TextSizePixels.Y));

        _finalShadowOffset = _shadowOffset * _finalTextSize * Font.Spacing;
    }

    // Inherited methods.
    public override void Draw(IDrawInfo info)
    {
        if (!IsDrawingNeeded) return;

        if (IsShadowEnabled)
        {
            info.SpriteBatch.DrawString(Font,
            Text,
            Display.ToRealPosition(Position + (Font.LineSpacing * ShadowOffset)),
            ShadowColor,
            Rotation,
            _originLocation + _finalShadowOffset,
            Display.ToRealScale(new Vector2(_finalTextSize)),
            Effects,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
        }

        info.SpriteBatch.DrawString(Font,
            Text,
            Display.ToRealPosition(Position),
            CombinedMask,
            Rotation,
            _originLocation,
            Display.ToRealScale(new Vector2(_finalTextSize)),
            Effects,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }
}