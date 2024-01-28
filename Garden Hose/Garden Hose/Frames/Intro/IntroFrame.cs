using GardenHose.Frames.MainMenu;
using GardenHose.Settings;
using GardenHoseEngine;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
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

    private const float LOGO_DURATION = MathF.PI * 0.75f;
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
        _keyboardListener = KeyboardListenerCreator.AnyKey(KeyCondition.WhileDown, OnUserInputEvent);
        _mouseListener = MouseListenerCreator.AnyButton(true, MouseCondition.WhileDown, OnUserInputEvent);
        _keyboardListener.StartListening();
        _mouseListener.StartListening();
    }

    private void LoadSettings()
    {
        SettingsManager.DataRootPath = GHEngine.DataRootPath;
        SettingsManager.ReadSettings();
    }


    // Inherited methods.
    public override void Load()
    {
        _monogameLogo = new(new SpriteAnimation(0f, this, Origin.Center, "ui/monogame_logo").CreateInstance(), LOGO_START_SCALE);
        _logo = new(new SpriteAnimation(0f, this, Origin.Center, "ui/logo").CreateInstance(), LOGO_START_SCALE);
        GH.LoadGlobalAssets();

        base.Load();
    }

    public override void OnStart()
    {
        base.OnStart();

        AddLayer(new Layer("logo layer"));
        CreateSkipListeners();

        GHEngine.Game.IsFixedTimeStep = false;
        Display.GraphicsManager.SynchronizeWithVerticalRetrace = true;
        Display.GraphicsManager.ApplyChanges();

        GameFrameManager.LoadNextFrame(new MainMenuFrame("Main Menu"), () => _nextFrameLoaded = true);
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);
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
            _logo.Size += _monogameLogo.TextureSize * LOGO_SCALE_INCREASE * time.PassedTimeSeconds;
            _logo.Opacity = MathF.Sin((_passedTime - FIRST_LOGO_TIME) / (SECOND_LOGO_TIME - FIRST_LOGO_TIME) * MathF.PI);
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

        _keyboardListener.StopListening();
        _mouseListener.StopListening();
    }


    // Private methods.
    private void OnUserInputEvent(object? sender, EventArgs args)
    {
        if (UserInput.KeyboardState.Current.IsKeyDown(Keys.F11)
            || UserInput.KeyboardState.Current.IsKeyDown(Keys.LeftControl))
        {
            return;
        }

        SkipIntro();
    }

    private void SkipIntro() => _passedTime = EXIT_TIME;
}