using GardenHose.Game;
using GardenHose.Game.Background;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace GardenHose.Frames.MainMenu;


internal class MainFrameBackgroundManager : FrameComponentManager<MainMenuFrame>
{
    // Private fields.
    private readonly ILayer _backgroundLayer;

    private SpriteItem _logo;
    private const float LOGO_SPEED = 1 / 7f;
    private const float LOGO_POSITION_MOVEMENT_Y = 15f;

    private SpriteItem _planet;

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

    private GameBackground _background = new(BackgroundImage.Default); 


    // Constructors.
    public MainFrameBackgroundManager(MainMenuFrame parentFrame, ILayer backgroundLayer) : base(parentFrame)
    {
        _backgroundLayer = backgroundLayer ?? throw new ArgumentNullException(nameof(backgroundLayer));
    }


    // Private methods.
    private void FocusOnMouse()
    {
        _focusPosition += (UserInput.VirtualMousePosition.Current - _focusPosition)
            * 0.8f * GameFrameManager.PassedTimeSeconds;

        _planet.Position.Vector = Display.VirtualSize + _maxPlanetOffset - _maxPlanetOffset * (_focusPosition / Display.VirtualSize);
    }


    // Inherited methods.
    internal override void Load()
    {
        _background.Load(ParentFrame);
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