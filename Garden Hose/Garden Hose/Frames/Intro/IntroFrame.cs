using GardenHose.Frames.MainMenu;
using GardenHose.Settings;
using GardenHoseEngine;
using GardenHoseEngine.Animatable;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GardenHose.Frames.Intro;

internal class IntroFrame : GameFrame
{
    // Private fields.
    private SpriteAnimation _monogameLogoAnim;
    private SpriteItem _monogameLogoItem;
    private SpriteAnimation _logoAnim;
    private SpriteItem _logoItem;

    private float _passedTime = -0.5f;
    private bool _nextFrameLoaded = false;

    private IInputListener _keyboardListener;
    private IInputListener _mouseListener;

    private const float LOGO_TIME_SEC = 5f;
    private const float LOGO_TIME_SIN_MULTIPLIER = MathF.PI / LOGO_TIME_SEC;
    private readonly Vector2 _logoStartSize = new(0.6f, 0.6f);
    private readonly Vector2 _logoEndSize = new(0.75f, 0.75f);


    // Constructors.
    public IntroFrame(string? name) : base(name) { }


    // Private methods.
    private void CreateLogos()
    {
        _monogameLogoItem = new(_monogameLogoAnim);
        _monogameLogoItem.Opacity = 0f;
        _monogameLogoItem.Position.Vector = Display.VirtualSize / 2f;
        _monogameLogoItem.Scale.SetKeyFrames(new KeyframeCollection(_logoStartSize)
            .AddKeyFrame(_logoEndSize, LOGO_TIME_SEC));
        _monogameLogoItem.Scale.Start();
        AddItem(_monogameLogoItem);
        TopLayer!.AddDrawableItem(_monogameLogoItem);


        _logoItem = new(_logoAnim);
        _logoItem.Opacity = 0f;
        _logoItem.Position.Vector = Display.VirtualSize / 2f;
        _logoItem.Scale.Vector = Vector2.One; ;
        _logoItem.Scale.SetKeyFrames(new KeyframeCollection(_logoStartSize)
            .AddKeyFrame(LOGO_TIME_SEC)
            .AddKeyFrame(_logoEndSize, LOGO_TIME_SEC));
        _logoItem.Scale.Start();
        AddItem(_logoItem);
        TopLayer!.AddDrawableItem(_logoItem);
    }

    private void CreateSkipListeners()
    {
        _keyboardListener = KeyboardListenerCreator.AnyKey(this, KeyCondition.WhileDown, OnKeyboardInputEvent);
        _mouseListener = MouseListenerCreator.AnyButton(this, true, MouseCondition.WhileDown, OnMouseInputEvent);
        UserInput.AddListener(_keyboardListener);
        UserInput.AddListener(_mouseListener);
    }

    private void LoadSettings()
    {
        SettingsManager.DataRootPath = GHEngine.DataRootPath;
        SettingsManager.ReadSettings();
    }


    // Inherited methods.
    public override void Load()
    {
        base.Load();

        _monogameLogoAnim = new(0f, this, Origin.Center, "ui/monogame_logo");
        _logoAnim = new(0f, this, Origin.Center, "ui/logo");
    }

    public override void OnStart()
    {
        base.OnStart();

        AddLayer(new Layer("logo layer"));

        CreateLogos();
        CreateSkipListeners();

        GHEngine.Game.IsFixedTimeStep = false;
        Display.GraphicsManager.SynchronizeWithVerticalRetrace = true;
        Display.GraphicsManager.ApplyChanges();

        GameFrameManager.LoadNextFrame(new MainMenuFrame("Main Menu"), () => _nextFrameLoaded = true);
    }

    public override void Update()
    {
        base.Update();

        _passedTime += GameFrameManager.PassedTimeSeconds;

        if (_passedTime <= LOGO_TIME_SEC)
        {
            _monogameLogoItem.Opacity = (float)Math.Sin(_passedTime * LOGO_TIME_SIN_MULTIPLIER);
            return;
        }
        else if (_passedTime <= LOGO_TIME_SEC * 2)
        {
            _monogameLogoItem.Opacity = 0f;
            _logoItem.Opacity = (float)Math.Sin((_passedTime - LOGO_TIME_SEC) * LOGO_TIME_SIN_MULTIPLIER);
            return;
        }

        _logoItem.Opacity = 0f;
        _monogameLogoItem.Opacity = 0f;
        if (_nextFrameLoaded && (_passedTime >= LOGO_TIME_SEC * 2))
        {
            GameFrameManager.JumpToNextFrame();
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        _keyboardListener.StopListening();
        _mouseListener.StopListening();
    }


    // Private methods.
    private void OnKeyboardInputEvent(object? sender, EventArgs args)
    {
        if (UserInput.KeyboardState.Current.IsKeyDown(Keys.F11)
            || UserInput.KeyboardState.Current.IsKeyDown(Keys.LeftControl))
        {
            return;
        }

        SkipIntro();
    }

    private void OnMouseInputEvent(object? sender, MouseEventArgs args) => SkipIntro();

    private void SkipIntro() => _passedTime = LOGO_TIME_SEC * 3;
}