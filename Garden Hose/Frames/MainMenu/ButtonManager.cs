using GardenHose.Frames.InGame;
using GardenHose.UI;
using GardenHose.UI.Buttons.Connector;
using GardenHoseEngine;
using GardenHoseEngine.Animatable;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Frames.MainMenu;

internal class MainFrameButtonManager : FrameComponentManager<MainMenuFrame>
{
    // Private fields.
    private readonly ILayer _uiLayer;

    private const float X_LOCATION_BUTTON_IN = 275f;
    private const float X_LOCATION_BUTTON_OUT = -275f;

    private ConnectorRectangleButton _play;
    private const float Y_LOCATION_BUTTON_PLAY = 500f;
    private ConnectorRectangleButton _editor;
    private const float Y_LOCATION_BUTTON_EDITOR = 650f;
    private ConnectorRectangleButton _options;
    private const float Y_LOCATION_BUTTON_OPTIONS = 800f;
    private ConnectorRectangleButton _exit;
    private const float Y_LOCATION_BUTTON_EXIT = 950f;

    // Constructors.
    internal MainFrameButtonManager(MainMenuFrame parentFrame, ILayer uiLayer) : base(parentFrame)
    {
        _uiLayer = uiLayer ?? throw new ArgumentNullException(nameof(uiLayer));
    }


    // Internal methods.
    internal void SetMainButtonAbility(bool ability)
    {
        _play.IsEnabled = ability;
        _play.IsVisible = ability;
        _editor.IsEnabled = ability;
        _editor.IsVisible = ability;
        _options.IsEnabled = ability;
        _options.IsVisible = ability;
        _exit.IsEnabled = ability;
        _exit.IsVisible = ability;

    }
    
    internal void SetMainButtonClickability(bool clickability)
    {
        _play.IsClickable = false;
        _editor.IsClickable = false;
        _options.IsClickable = false;
        _exit.IsClickable = false;
    }

    internal void BringMainButtonsIn()
    {
        SetMainButtonAbility(true);
        AnimateMainButtons(X_LOCATION_BUTTON_OUT, X_LOCATION_BUTTON_IN);
        _exit.Position.AnimationFinished -= OnExitButtonBroughtOutEvent;
    }

    internal void BringMainButtonsOut()
    {
        AnimateMainButtons(X_LOCATION_BUTTON_IN, X_LOCATION_BUTTON_OUT);
        _exit.Position.AnimationFinished += OnExitButtonBroughtOutEvent;
    }


    // Private methods.
    private void CreateButtons()
    {
        _play = ConnectorElement.CreateNormalButton(Direction.Right, Vector2.Zero, new Vector2(0.5f, 0.5f));
        _play.Text = "Play";
        _uiLayer.AddDrawableItem(_play);
        ParentFrame.AddItem(_play);
        _play.ClickHandler += OnPlayClickEvent;

        _editor = ConnectorElement.CreateNormalButton(Direction.Right, Vector2.Zero, new Vector2(0.5f, 0.5f));
        _editor.Text = "Editor";
        _uiLayer.AddDrawableItem(_editor);
        ParentFrame.AddItem(_editor);

        _options = ConnectorElement.CreateNormalButton(Direction.Right, Vector2.Zero, new Vector2(0.5f, 0.5f));
        _options.Text = "Options";
        _uiLayer.AddDrawableItem(_options);
        ParentFrame.AddItem(_options);

        _exit = ConnectorElement.CreateNormalButton(Direction.Right, Vector2.Zero, new Vector2(0.5f, 0.5f));
        _exit.ClickHandler += OnExitClickEvent;
        _exit.Text = "Exit";
        _uiLayer.AddDrawableItem(_exit);
        ParentFrame.AddItem(_exit);

        ConnectorSlider MySlider = new(Direction.Left, 0f, 1f, 0f);
        MySlider.Position.Vector = new(1000f, 500f); 
        _uiLayer.AddDrawableItem(MySlider);
        ParentFrame.AddItem(MySlider);
    }

    private void AnimateMainButtons(float xStart, float xEnd)
    {
        _play.Position.Stop();
        _editor.Position.Stop();
        _options.Position.Stop();
        _exit.Position.Stop();

        _play.Position.SetKeyFrames(new KeyFrameBuilder(
            new(xStart, Y_LOCATION_BUTTON_PLAY), InterpolationMethod.EaseOut)
            .AddKeyFrame(new(xEnd, Y_LOCATION_BUTTON_PLAY), 0.4f));
        _play.Position.Start();

        _editor.Position.SetKeyFrames(new KeyFrameBuilder(
            new(xStart, Y_LOCATION_BUTTON_EDITOR), InterpolationMethod.EaseOut)
            .AddKeyFrame(0.05f)
            .AddKeyFrame(new(xEnd, Y_LOCATION_BUTTON_EDITOR), 0.4f));
        _editor.Position.Start();

        _options.Position.SetKeyFrames(new KeyFrameBuilder(
            new(xStart, Y_LOCATION_BUTTON_OPTIONS), InterpolationMethod.EaseOut)
            .AddKeyFrame(0.1f)
            .AddKeyFrame(new(xEnd, Y_LOCATION_BUTTON_OPTIONS), 0.4f));
        _options.Position.Start();

        _exit.Position.SetKeyFrames(new KeyFrameBuilder(
            new(xStart, Y_LOCATION_BUTTON_EXIT), InterpolationMethod.EaseOut)
            .AddKeyFrame(0.15f)
            .AddKeyFrame(new(xEnd, Y_LOCATION_BUTTON_EXIT), 0.4f));
        _exit.Position.Start();
    }

    /* Click handlers */
    private void OnExitClickEvent(object? sender, EventArgs args)
    {
        SetMainButtonClickability(false);
        BringMainButtonsOut();
        _exit.Position.AnimationFinished += (sender, args) => GHEngine.Exit();
        ParentFrame.LayerManager.FadeStep = -4f;
    }

    private void OnPlayClickEvent(object? sender, EventArgs args)
    {
        GameFrameManager.LoadNextFrame(new InGameFrame("In-Game"), 
            () => GameFrameManager.JumpToNextFrame());
    }


    /* Animation handlers. */
    private void OnExitButtonBroughtOutEvent(object? sender, EventArgs args)
    {
        _exit.Position.AnimationFinished -= OnExitButtonBroughtOutEvent;
        SetMainButtonAbility(false);
    }


    // Inherited methods.
    internal override void Load()
    {
        ConnectorElement.LoadAllAssets(ParentFrame);
    }

    internal override void OnStart()
    {
        CreateButtons();
        BringMainButtonsIn();
    }
}