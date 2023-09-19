using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Text;

public class DynamicFont
{
    // Fields.
    public const int FONT_COUNT = 5;
    public const string TINY_FONT_NAME = "tiny";
    public const string SMALL_FONT_NAME = "small";
    public const string MEDIUM_FONT_NAME = "medium";
    public const string LARGE_FONT_NAME = "large";
    public const string HUGE_FONT_NAME = "huge";


    [MemberNotNull(nameof(SpriteFont))]
    public DynamicFontScale Scale
    {
        get => _scale;
        set
        {
            if ((int)value is < 0 or > FONT_COUNT)
            {
                throw new EnumValueException(nameof(value), nameof(DynamicFontScale), 
                    value.ToString(), (int)value);
            }

            _scale = value;
            SpriteFont = _fonts[(int)value];
        }
    }

    public SpriteFont SpriteFont { get; private set; }


    // Private fields.
    private DynamicFontScale _scale =  DynamicFontScale.Medium;
    private SpriteFont[] _fonts;


    // Constructors.
    public DynamicFont(SpriteFont[] fonts)
    {
        if (SpriteFont == null)
        {
            throw new ArgumentNullException(nameof(fonts));
        }
        if (fonts.Length != FONT_COUNT)
        {
            throw new ArgumentException($"A Dynamic Font requires exactly 5 fonts, got {fonts.Length}", nameof(fonts));
        }
        for (int i =0; i <  FONT_COUNT; i++)
        {
            if (SpriteFont == null)
            {
                throw new ArgumentException($"Font at index {i} is null.");
            }
        }

        _fonts = fonts;
        Scale = DynamicFontScale.Medium;
    }

    public DynamicFont(GameFrame user, AssetManager assetManager, string assetRootPath)
    {
        if (assetManager == null)
        {
            throw new ArgumentNullException(nameof(assetManager));
        }

        if (assetRootPath == null)
        {
            throw new ArgumentNullException(nameof(assetRootPath));
        }

        _fonts = new SpriteFont[assetRootPath.Length];

        _fonts[(int)DynamicFontScale.Tiny] = assetManager.GetFont(user, Path.Combine(assetRootPath, TINY_FONT_NAME));
        _fonts[(int)DynamicFontScale.Small] = assetManager.GetFont(user, Path.Combine(assetRootPath, SMALL_FONT_NAME));
        _fonts[(int)DynamicFontScale.Medium] = assetManager.GetFont(user, Path.Combine(assetRootPath, MEDIUM_FONT_NAME));
        _fonts[(int)DynamicFontScale.Large] = assetManager.GetFont(user, Path.Combine(assetRootPath, LARGE_FONT_NAME));
        _fonts[(int)DynamicFontScale.Huge] = assetManager.GetFont(user, Path.Combine(assetRootPath, HUGE_FONT_NAME));

        Scale = DynamicFontScale.Medium;
    }


    // Methods.
    public Vector2 MeasureString(string text) =>  SpriteFont.MeasureString(text);


    // Operators.
    public static implicit operator SpriteFont(DynamicFont dynamicFont) => dynamicFont.SpriteFont;
}