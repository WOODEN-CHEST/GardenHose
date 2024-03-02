using GardenHose.Frames.MainMenu;
using GardenHose.Settings;
using GardenHoseEngine;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace GardenHose.Frames.Intro;


internal class IntroFrame : GameFrame
{
    // Private fields.
    private SpriteItem _monogameLogo;
    private SpriteItem _logo;

    private float _passedTime = 0f;
    private bool _nextFrameLoaded = false;

    private IInputListener _keyboardListener;
    private IInputListener _mouseListener;

    private const float LOGO_DURATION = MathF.PI * 1.25f;
    private const float FIRST_LOGO_TIME = 1.0f;
    private const float SECOND_LOGO_TIME = FIRST_LOGO_TIME + LOGO_DURATION;
    private const float EXIT_TIME = SECOND_LOGO_TIME + LOGO_DURATION;

    private const float LOGO_START_SCALE = 0.6f;
    private const float LOGO_SCALE_INCREASE = 0.15f / LOGO_DURATION;


    // Constructors.
    public IntroFrame(string? name) : base(name) { }


    // Private methods.
    private void CreateSkipListeners()
    {
        _keyboardListener = IInputListener.CreateAnyKey(KeyCondition.WhileDown);
        _mouseListener = IInputListener.CreateAnyMouseButton(true, MouseCondition.WhileDown);
    }

    private void LoadSettings()
    {
        SettingsManager.DataRootPath = GHEngine.DataRootPath;
        SettingsManager.ReadSettings();
    }


    // Inherited methods.
    public override void Load()
    {
        _monogameLogo = new(new SpriteAnimation(0f, this, Origin.Center, "ui/monogame_logo").CreateInstance(), LOGO_START_SCALE)
        {
            Position = Display.VirtualSize * 0.5f
        };
        _logo = new(new SpriteAnimation(0f, this, Origin.Center, "ui/logo").CreateInstance(), LOGO_START_SCALE)
        {
            Position = Display.VirtualSize * 0.5f
        };


        GH.LoadGlobalAssets();

        base.Load();
    }

    public override void OnStart()
    {
        base.OnStart();

        AddLayer(new Layer("logo layer"));
        TopLayer!.AddDrawableItem(_monogameLogo);
        TopLayer.AddDrawableItem(_logo);

        CreateSkipListeners();

        GHEngine.Game.IsFixedTimeStep = false;
        Display.GraphicsManager.SynchronizeWithVerticalRetrace = true;
        Display.GraphicsManager.ApplyChanges();

        GameFrameManager.LoadNextFrame(new MainMenuFrame("Main Menu"), () => _nextFrameLoaded = true);
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);

        if (_keyboardListener.Listen() || _mouseListener.Listen()
            && !(UserInput.KeyboardState.Current.IsKeyDown(Keys.F11) || UserInput.KeyboardState.Current.IsKeyDown(Keys.LeftControl)))
        {
            SkipIntro();
        }
        _mouseListener.Listen();

        _passedTime += time.PassedTimeSeconds;

        if (_passedTime <= FIRST_LOGO_TIME)
        {
            _monogameLogo.Opacity = 0f;
            _logo.Opacity = 0f;
        }
        else if (_passedTime < SECOND_LOGO_TIME)
        {
            _monogameLogo.Size += _monogameLogo.TextureSize * LOGO_SCALE_INCREASE * time.PassedTimeSeconds;
            _monogameLogo.Opacity = MathF.Sin((_passedTime - FIRST_LOGO_TIME) / (SECOND_LOGO_TIME - FIRST_LOGO_TIME) * MathF.PI);
            _logo.Opacity = 0f;
        }
        else if (_passedTime < EXIT_TIME)
        {
            _monogameLogo.Opacity = 0f;
            _logo.Size += _logo.TextureSize * LOGO_SCALE_INCREASE * time.PassedTimeSeconds;
            _logo.Opacity = MathF.Sin((_passedTime - SECOND_LOGO_TIME) / (EXIT_TIME - SECOND_LOGO_TIME) * MathF.PI);
        }
        else
        {
            _logo.Opacity = 0f;
            _monogameLogo.Opacity = 0f;

            if (_nextFrameLoaded)
            {
                GameFrameManager.JumpToNextFrame();
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }


    // Private methods.
    private void SkipIntro() => _passedTime = EXIT_TIME;

    public override void Draw(IDrawInfo info, RenderTarget2D layerPixelBuffer, RenderTarget2D framePixelBuffer)
    {
        base.Draw(info, layerPixelBuffer, framePixelBuffer);
    }
}