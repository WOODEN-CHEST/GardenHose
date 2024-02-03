using GardenHose.Game.GameAssetManager;
using GardenHose.Game.Background;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace GardenHose.Game;

internal class GameBackground : IDrawableItem
{
    // Fields.
    public bool IsVisible { get; set; } = true;
    public Effect? Shader { get; set; } = null;


    // Internal fields.
    internal const string ASSETPATH_STAR_SMALL = "game/stars/small";
    internal const string ASSETPATH_STAR_MEDIUM = "game/stars/medium";
    internal const string ASSETPATH_STAR_BIG = "game/stars/big";
    internal const string ASSETPATH_BACKGROUND_DEFAULT = "game/backgrounds/default";

    internal Color SpaceColor { get; set; } = Color.Black;
    internal int SmallStarCount { get; init; } = 0;
    internal int MediumStarCount { get; init; } = 0;
    internal int BigStarCount { get; init; } = 0;
    internal BackgroundImage Image { get; init; }


    // Private static fields.
    private static readonly Vector2 SMALL_STAR_SIZE = new(11f);
    private static readonly Vector2 MEDIUM_STAR_SIZE = new(24f);
    private static readonly Vector2 LARGE_STAR_SIZE = new(58f);
    private const float MAX_STAR_SCALE = 1f;
    private const float MIN_STAR_SCALE = 0.5f;


    // Private fields.
    private SpriteItem _background;
    private AnimationInstance _smallStarAnim;
    private AnimationInstance _mediumStarAnim;
    private AnimationInstance _bigStarAnim;

    private const float STAR_TWINKLE_TIME = 1 / 10f;
    private float _timeSinceStarTwinkle = 0f;
    private const float STAR_MIN_BRIGHTNESS = 0.8f;
    private const float STAR_MAX_BRIGHTNESS = 1.0f;
    private SpriteItem[] _stars;



    // Constructors.
    public GameBackground(BackgroundImage image)
    {
        Image = image;
    }


    // Internal methods.
    internal void Load(GHGameAssetManager assetManager)
    {
        _smallStarAnim = assetManager.GetAnimation(GHGameAnimationName.Background_Star_Small)!.CreateInstance();
        _mediumStarAnim = assetManager.GetAnimation(GHGameAnimationName.Background_Star_Medium)!.CreateInstance();
        _bigStarAnim = assetManager.GetAnimation(GHGameAnimationName.Background_Star_Big)!.CreateInstance();

        _background = Image switch
        {
            BackgroundImage.Default => new(assetManager.GetAnimation(GHGameAnimationName.Background_Default)!.CreateInstance()),
            _ => throw new EnumValueException(nameof(Image), Image)
        };
    }

    internal void Load(IGameFrame? owner)
    {
        _smallStarAnim = new SpriteAnimation(0f, owner, Origin.Center, ASSETPATH_STAR_SMALL).CreateInstance();
        _mediumStarAnim = new SpriteAnimation(0f, owner, Origin.Center, ASSETPATH_STAR_MEDIUM).CreateInstance();
        _bigStarAnim = new SpriteAnimation(0f, owner, Origin.Center, ASSETPATH_STAR_BIG).CreateInstance();

        _background = Image switch
        {
            BackgroundImage.Default => new(new SpriteAnimation(0f, owner, Origin.TopLeft, 
                ASSETPATH_BACKGROUND_DEFAULT) .CreateInstance(), Display.VirtualSize),
            _ => throw new EnumValueException(nameof(Image), Image)
        };
    }

    internal void CreateBackground()
    {
        _stars = new SpriteItem[SmallStarCount + MediumStarCount + BigStarCount];
        int Index = 0;

        for (int i =0; i < SmallStarCount; i++, Index++)
        {
            SpriteItem Star = new(_smallStarAnim, SMALL_STAR_SIZE * GetRandomScale());
            Star.Position = GetRandomPosition();
            Star.Mask = GetRandomStarColor();
            Star.Rotation = GetRandomRotation();
            _stars[Index] = Star;
        }

        for (int i = 0; i < MediumStarCount; i++, Index++)
        {
            SpriteItem Star = new(_mediumStarAnim, MEDIUM_STAR_SIZE * GetRandomScale());
            Star.Position = GetRandomPosition();
            Star.Mask = GetRandomStarColor();
            Star.Rotation = GetRandomRotation();
            _stars[Index] = Star;
        }

        for (int i = 0; i < BigStarCount; i++, Index++)
        {
            SpriteItem Star = new(_bigStarAnim, LARGE_STAR_SIZE * GetRandomScale());
            Star.Position = GetRandomPosition();
            Star.Mask = GetRandomStarColor();
            Star.Rotation = GetRandomRotation();
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

    private float GetRandomScale()
    {
        return MAX_STAR_SCALE - (MIN_STAR_SCALE * Random.Shared.NextSingle());
    }

    private void TwinkleStars()
    {
        foreach (SpriteItem Star in _stars)
        {
            Star.Brightness = STAR_MIN_BRIGHTNESS - (Random.Shared.NextSingle() * (1f - STAR_MIN_BRIGHTNESS));
        }
    }


    // Inherited methods.
    public void Draw(IDrawInfo info)
    {
        if (!IsVisible) return;

        _timeSinceStarTwinkle += info.Time.PassedTimeSeconds;

        if (_timeSinceStarTwinkle > STAR_TWINKLE_TIME)
        {
            TwinkleStars();
            _timeSinceStarTwinkle = 0f;
        }

        info.SpriteBatch.Draw(
            Display.SinglePixel,
            Vector2.Zero,
            null,
            SpaceColor,
            0f,
            Vector2.Zero,
            Display.WindowSize,
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);

        _background.Draw(info);

        foreach (SpriteItem Star in _stars)
        {
            Star.Draw(info);
        }
    }
}