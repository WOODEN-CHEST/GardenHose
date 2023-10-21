using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Frames.MainMenu;

internal class MainFrameBackgroundManager : FrameComponentManager<MainMenuFrame>
{
    // Private fields.
    private readonly ILayer _bgLayer;

    private SpriteAnimation _logoAnim;
    private SpriteItem _logo;
    private const float LOGO_SPEED = 1 / 7f;
    private const float LOGO_POSITION_Y = 50f;
    private const float LOGO_POSITION_MOVEMENT_Y = 15f;

    private SpriteAnimation _backgroundAnim;
    private SpriteItem _background;
    private SpriteAnimation _planetAnim;
    private SpriteItem _planet;
    private SpriteAnimation _starAnim;
    private SpriteItem[] _stars;

    private Vector2 _focusPosition = Vector2.Zero;
    private const float PLANET_SIZE = 1.1f;
    private readonly Vector2 _maxPlanetOffset = new(30f);

    private float _timeSinceStarUpdate = 0f;
    private const float REQUIRED_STAR_TIME = 1f / 10f;
    private const int MIN_STAR_OPACITY = 800;
    private const int MAX_STAR_OPACITY = 1000;
    private const int MIN_STAR_COLOR_CHANNEl = 800;
    private const int MAX_STAR_COLOR_CHANNEl = 1000;
    private const float MAX_STAR_SIZE = 0.085f;
    private const int ATTEMPTED_STAR_COUNT = 300;


    // Constructors.
    public MainFrameBackgroundManager(MainMenuFrame parentFrame, ILayer bgLayer) : base(parentFrame)
    {
        _bgLayer = bgLayer ?? throw new ArgumentNullException(nameof(bgLayer));
    }


    // Private methods.
    private void FocusOnMouse()
    {
        _focusPosition += (UserInput.VirtualMousePosition.Current - _focusPosition)
            * 0.8f * GameFrameManager.PassedTimeSeconds;

        _planet.Position.Vector = Display.VirtualSize + _maxPlanetOffset - _maxPlanetOffset * (_focusPosition / Display.VirtualSize);
    }

    private void UpdateStars()
    {
        _timeSinceStarUpdate += GameFrameManager.PassedTimeSeconds;
        if (_timeSinceStarUpdate < REQUIRED_STAR_TIME)
        {
            return;
        }

        foreach (SpriteItem Star in _stars)
        {
            Star.Opacity = Random.Shared.Next(MIN_STAR_OPACITY, MAX_STAR_OPACITY) / (float)MAX_STAR_OPACITY;
        }
        _timeSinceStarUpdate -= REQUIRED_STAR_TIME;
    }

    private void CreateStars()
    {
        float EliminationDistance = _planet.TextureSize.X * _planet.Scale.Vector.X * 0.5f;
        Vector2 PlanetCenter = _planet.Position;
        PlanetCenter.X -= _planet.TextureSize.X * 0.5f;
        PlanetCenter.Y -= _planet.TextureSize.Y * 0.15f;

        List<SpriteItem> Stars = new();
        for (int i = 0; i < ATTEMPTED_STAR_COUNT; i++)
        {
            Vector2 Position = new(
                Random.Shared.Next((int)Display.VirtualSize.X),
                Random.Shared.Next((int)Display.VirtualSize.Y));
            if (Vector2.Distance(PlanetCenter, Position) <= EliminationDistance)
            {
                continue;
            }


            SpriteItem Star = new(_starAnim);
            Star.Position.Vector = Position;

            FloatColor Color = new();
            Color.R = Random.Shared.Next(MIN_STAR_COLOR_CHANNEl, MAX_STAR_COLOR_CHANNEl) / (float)MAX_STAR_COLOR_CHANNEl;
            Color.G = Random.Shared.Next(MIN_STAR_COLOR_CHANNEl, MAX_STAR_COLOR_CHANNEl) / (float)MAX_STAR_COLOR_CHANNEl;
            Color.B = Random.Shared.Next(MIN_STAR_COLOR_CHANNEl, MAX_STAR_COLOR_CHANNEl) / (float)MAX_STAR_COLOR_CHANNEl;
            Color.A = 1f;
            Star.Mask = Color;

            Star.Scale.Vector = Vector2.One * MAX_STAR_SIZE * Random.Shared.NextSingle();
            Star.Rotation = Random.Shared.NextSingle() * MathHelper.TwoPi;

            ParentFrame.LayerManager.BackgroundLayer.AddDrawableItem(Star);
            Stars.Add(Star);
        }
        _stars = Stars.ToArray();
        
    }


    // Inherited methods.
    internal override void Load()
    {
        _logoAnim = new(0f, ParentFrame, Origin.TopLeft, "ui/logo_tiny");
        _backgroundAnim = new(0f, ParentFrame, Origin.TopLeft, "ui/title/title_background");
        _planetAnim = new(0f, ParentFrame, Origin.BottomRight, "ui/title/planet");
        _starAnim = new(0f, ParentFrame, Origin.Center, "ui/title/star");
    }

    internal override void OnStart()
    {
        _background = new(_backgroundAnim);
        ParentFrame.LayerManager.BackgroundLayer.AddDrawableItem(_background);

        _planet = new(_planetAnim);
        _planet.Position.Vector = Display.VirtualSize;
        _planet.Scale.Vector *= PLANET_SIZE;
        CreateStars();
        ParentFrame.LayerManager.BackgroundLayer.AddDrawableItem(_planet);

        _logo = new(_logoAnim);
        _logo.Position.Vector = new(50f, 50f);
        ParentFrame.LayerManager.BackgroundLayer.AddDrawableItem(_logo);
    }

    internal override void Update()
    {
        Vector2 LogoPosition = _logo.Position;
        LogoPosition.Y = LOGO_POSITION_Y + MathF.Sin(GameFrameManager.TotalTimeSeconds * LOGO_SPEED) * LOGO_POSITION_MOVEMENT_Y;
        _logo.Position.Vector = LogoPosition;

        FocusOnMouse();
        UpdateStars();
    }
}