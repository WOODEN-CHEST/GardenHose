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
        }
    }

    [MemberNotNull(nameof(_font))]
    public SpriteFont Font
    {
        get => _font;
        set => _font = value ?? throw new ArgumentNullException(nameof(_font));
    }

    public Origin TextOrigin { get; set; }
    public float Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            UpdateFinalTextSize();
        }
    }
    public SpriteEffects Effects { get; set; }
    public float FittingSizePixels { get; set; }
    public Vector2 PixelSize => _font.MeasureString(Text);
    public bool IsShadowEnabled {  get; set; } = false;
    public Color ShadowColor { get; set; } = Color.Black;

    // Private fields.
    private string _text;
    private SpriteFont _font;
    private Vector2 _originLocation;
    private float _scale;
    private float _finalTextSize;


    // Constructors.
    public FittedText(string text, SpriteFont font)
    {
        Text = text;
        Font = font;
    }


    // Private methods.
    private void UpdateFinalTextSize()
    {
        float TextLengthPixels = PixelSize.X * Scale;
        _finalTextSize = Math.Min(1f, FittingSizePixels / TextLengthPixels);
    }

    private void UpdateOrigin()
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
            Origin.BottomLeft => new Vector2(0f, TextSizePixels.Y ),
            Origin.BottomMiddle => new Vector2(TextSizePixels.X * 0.5f, TextSizePixels.Y),
            Origin.BottomRight => new Vector2(TextSizePixels.X, TextSizePixels.Y),
            _ => throw new EnumValueException(nameof(TextOrigin), TextOrigin),
        };
    }

    // Inherited methods.
    public override void Draw(IDrawInfo info)
    {

        if (IsShadowEnabled)
        {

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