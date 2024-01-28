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


namespace GardenHose.Frames.MainMenu;


internal class MainFrameBackgroundManager : FrameComponentManager<MainMenuFrame>
{
    // Private static fields.
    private static readonly Vector2 PLANET_SIZE = new(1100f, 1100f);
    private readonly Vector2 PLANET_PADDING = new(-50f, -50f);
    private const float PLANET_OFFSET_SPEED = 0.8f;

    private static readonly Vector2 LOGO_SIZE = new(467f, 236f);
    private readonly Vector2 LOGO_PADDING = new(50f);
    private readonly Vector2 MAX_PLANET_OFFSET = new(50f);
    private const float LOGO_SPEED = 1 / 7f;
    private readonly Vector2 LOGO_MOVEMENT_AMOUNT = new(0f, 15f);


    // Private fields.
    private readonly ILayer _backgroundLayer;
    private GameBackground _background = new(BackgroundImage.Default);
    private SpriteItem _logo;
    private SpriteItem _planet;

    private Vector2 _delayedOffsetFromCenter = Vector2.Zero;
    

    // Constructors.
    public MainFrameBackgroundManager(MainMenuFrame parentFrame, ILayer backgroundLayer) : base(parentFrame)
    {
        _backgroundLayer = backgroundLayer ?? throw new ArgumentNullException(nameof(backgroundLayer));
    }


    // Inherited methods.
    internal override void Load()
    {
        _background.Load(ParentFrame);
        _backgroundLayer.AddDrawableItem(_background);
        _planet = new(new SpriteAnimation(0f, ParentFrame, Origin.BottomRight, "ui/title/planet").CreateInstance(), PLANET_SIZE);
        _backgroundLayer.AddDrawableItem(_planet);
        _logo = new(new SpriteAnimation(0f, ParentFrame, Origin.Center, "ui/logo_tiny").CreateInstance(), );
    }

    internal override void Update(IProgramTime time)
    {
        _logo.Position = LOGO_PADDING + (LOGO_SIZE * 0.5f) + (MathF.Sin(time.TotalTimeSeconds) * LOGO_MOVEMENT_AMOUNT);
        _delayedOffsetFromCenter += ((UserInput.VirtualMousePosition.Current - Display.VirtualSize * 0.5f)
            - _delayedOffsetFromCenter) * PLANET_OFFSET_SPEED * time.PassedTimeSeconds;
        _planet.Position = Display.VirtualSize + PLANET_PADDING - _delayedOffsetFromCenter;
    }
}