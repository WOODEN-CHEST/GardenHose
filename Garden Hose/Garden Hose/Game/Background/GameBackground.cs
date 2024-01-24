using GardenHose.Game.AssetManager;
using GardenHose.Game.Background;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static System.Formats.Asn1.AsnWriter;


namespace GardenHose.Game;

internal class GameBackground : IDrawableItem, ITimeUpdatable
{
    // Fields.
    public bool IsVisible { get; set; } = true;

    public Effect? Shader { get; set; } = null;


    // Internal fields.
    internal Color SpaceColor { get; set; } = Color.Black;

    internal int SmallStarCount { get; init; } = 0;

    internal int MediumStarCount { get; init; } = 0;

    internal int BigStarCount { get; init; } = 0;

    internal BackgroundImage Image { get; init; }


    // Private fields.
    private SpriteItem _background;
    private AnimationInstance _smallStarAnim;
    private AnimationInstance _mediumStarAnim;
    private AnimationInstance _bigStarAnim;

    private const float STAR_TWINKLE_TIME = 1 / 10f;
    private float _timeSinceStarTwinkle = 0f;
    private const float STAR_MIN_BRIGHTNESS = 0.8f;
    private SpriteItem[] _stars;



    // Constructors.
    public GameBackground(BackgroundImage image)
    {
        Image = image;
    }



    // Internal methods.
    internal void Load(GHGameAssetManager assetManager)
    {
        _smallStarAnim = assetManager.GetAnimation("background_star_small")!.CreateInstance();
        _mediumStarAnim = assetManager.GetAnimation("background_star_medium")!.CreateInstance();
        _bigStarAnim = assetManager.GetAnimation("background_star_big")!.CreateInstance();

        _background = Image switch
        {
            BackgroundImage.Default => new(assetManager.GetAnimation("background_default")!),
            _ => throw new EnumValueException(nameof(Image), nameof(BackgroundImage),
                Image.ToString(), (int)Image)
        };
    }

    internal void CreateBackground()
    {
        _stars = new SpriteItem[SmallStarCount + MediumStarCount + BigStarCount];
        int Index = 0;

        for (int i =0; i < SmallStarCount; i++, Index++)
        {
            SpriteItem Star = new(_smallStarAnim);
            Star.Position.Vector = GetRandomPosition();
            Star.Mask = GetRandomStarColor();
            Star.Rotation = GetRandomRotation();
            Star.Scale.Vector = GetRandomScale();
            _stars[Index] = Star;
        }

        for (int i = 0; i < MediumStarCount; i++, Index++)
        {
            SpriteItem Star = new(_mediumStarAnim);
            Star.Position.Vector = GetRandomPosition();
            Star.Mask = GetRandomStarColor();
            Star.Rotation = GetRandomRotation();
            Star.Scale.Vector = GetRandomScale();
            _stars[Index] = Star;
        }

        for (int i = 0; i < BigStarCount; i++, Index++)
        {
            SpriteItem Star = new(_bigStarAnim);
            Star.Position.Vector = GetRandomPosition();
            Star.Mask = GetRandomStarColor();
            Star.Rotation = GetRandomRotation();
            Star.Scale.Vector = GetRandomScale();
            _stars[Index] = Star;
        }
    }


    // Private methods.
    private Vector2 GetRandomPosition()
    {
        return new Vector2(Random.Shared.Next((int)Display.VirtualSize.X),
            Random.Shared.Next((int)Display.VirtualSize.Y));
    }

    private Color GetRandomStarColor()
    {
        return new FloatColor(0.8f + (Random.Shared.NextSingle() * 0.2f),
            0.8f + (Random.Shared.NextSingle() * 0.2f),
            0.8f + (Random.Shared.NextSingle() * 0.2f), 1f);
    }

    private float GetRandomRotation()
    {
        return Random.Shared.NextSingle() * MathHelper.TwoPi;
    }

    private Vector2 GetRandomScale()
    {
        const float STAR_SCALE = 0.25f;
        const float MAX_STAR_DOWNSCALE = 0.5f;

        return new Vector2(STAR_SCALE - (MAX_STAR_DOWNSCALE * Random.Shared.NextSingle()) );
    }

    private void TwinkleStars()
    {
        foreach (SpriteItem Star in _stars)
        {
            Star.Brightness = STAR_MIN_BRIGHTNESS + (Random.Shared.NextSingle() * (1f - STAR_MIN_BRIGHTNESS));
        }
    }


    // Inherited methods.
    public void Update()
    {
        _timeSinceStarTwinkle += GameFrameManager.PassedTimeSeconds;

        if (_timeSinceStarTwinkle > STAR_TWINKLE_TIME)
        {
            TwinkleStars();
            _timeSinceStarTwinkle = 0f;
        }
        
    }

    public void Draw()
    {
        if (!IsVisible) return;

        GameFrameManager.SpriteBatch.Draw(
            Display.SinglePixel,
            Vector2.Zero,
            null,
            SpaceColor,
            0f,
            Vector2.Zero,
            Display.WindowSize,
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);

        _background.Draw();

        foreach (SpriteItem Star in _stars)
        {
            Star.Draw();
        }
    }
}