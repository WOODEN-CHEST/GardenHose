using GardenHose.Frames.MainMenu;
using GardenHose.Settings;
using GardenHoseEngine;
using GardenHoseEngine.Animatable;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        _monogameLogoItem = new(GH.Engine.Display, _monogameLogoAnim);
        _monogameLogoItem.Opacity = 0f;
        _monogameLogoItem.Position.Vector = GH.Engine.Display.VirtualSize / 2f;
        _monogameLogoItem.Scale.SetKeyFrames(new KeyFrameBuilder(_logoStartSize)
            .AddKeyFrame(_logoEndSize, LOGO_TIME_SEC));
        _monogameLogoItem.Scale.Start();
        AddItem(_monogameLogoItem);
        TopLayer!.AddDrawableItem(_monogameLogoItem);


        _logoItem = new(GH.Engine.Display, _logoAnim);
        _logoItem.Opacity = 0f;
        _logoItem.Position.Vector = GH.Engine.Display.VirtualSize / 2f;
        _logoItem.Scale.Vector = Vector2.One; ;
        _logoItem.Scale.SetKeyFrames(new KeyFrameBuilder(_logoStartSize)
            .AddKeyFrame(LOGO_TIME_SEC)
            .AddKeyFrame(_logoEndSize, LOGO_TIME_SEC));
        _logoItem.Scale.Start();
        AddItem(_logoItem);
        TopLayer!.AddDrawableItem(_logoItem);
    }

    private void CreateSkipListeners()
    {
        _keyboardListener = KeyboardListenerCreator.AnyKey(GH.Engine.UserInput, this, this,
            KeyCondition.WhileDown, OnKeyboardInputEvent);
        _mouseListener = MouseListenerCreator.AnyButton(GH.Engine.UserInput, this, this, true,
            MouseCondition.WhileDown, OnMouseInputEvent);
        GH.Engine.UserInput.AddListener(_keyboardListener);
        GH.Engine.UserInput.AddListener(_mouseListener);
    }

    private void LoadSettings()
    {
        SettingsManager.DataRootPath = GH.Engine.DataRootPath;
        SettingsManager.ReadSettings();
    }


    // Inherited methods.
    public override void Load(AssetManager assetManager)
    {
        base.Load(assetManager);

        _monogameLogoAnim = new(0f, this, assetManager, Origin.Center, "ui/monogame_logo");
        _logoAnim = new(0f, this, assetManager, Origin.Center, "ui/logo");
    }

    public override void OnStart()
    {
        base.OnStart();

        AddLayer(new Layer("logo layer"));

        CreateLogos();
        CreateSkipListeners();

        GH.Engine.IsFixedTimeStep = false;
        GH.Engine.GraphicsManager.SynchronizeWithVerticalRetrace = false;
        GH.Engine.GraphicsManager.ApplyChanges();

        GH.Engine.FrameManager.LoadNextFrame(new MainMenuFrame("Main Menu"), () => _nextFrameLoaded = true);
    }

    public override void Update(float passedTimeSeconds)
    {
        base.Update(passedTimeSeconds);

        _passedTime += passedTimeSeconds;

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
            GH.Engine.FrameManager.JumpToNextFrame();
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
        if (GH.Engine.UserInput.KeyboardState.Current.IsKeyDown(Keys.F11)
            || GH.Engine.UserInput.KeyboardState.Current.IsKeyDown(Keys.LeftControl))
        {
            return;
        }

        SkipIntro();
    }

    private void OnMouseInputEvent(object? sender, MouseEventArgs args) => SkipIntro();

    private void SkipIntro() => _passedTime = LOGO_TIME_SEC * 3;
}