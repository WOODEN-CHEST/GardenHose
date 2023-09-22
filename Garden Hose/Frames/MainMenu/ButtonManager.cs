using GardenHose.UI;
using GardenHose.UI.Buttons.Connector;
using GardenHoseEngine;
using GardenHoseEngine.Animatable;
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
    private const float X_LOCATION_BUTTON_IN = 275f;
    private const float X_LOCATION_BUTTON_OUT = -275f;

    private ConnectorButton _play;
    private const float Y_LOCATION_BUTTON_PLAY = 500;
    private ConnectorButton _editor;
    private const float Y_LOCATION_BUTTON_EDITOR = 650;
    private ConnectorButton _options;
    private const float Y_LOCATION_BUTTON_OPTIONS = 800;
    private ConnectorButton _exit;
    private const float Y_LOCATION_BUTTON_EXIT = 950;

    // Constructors.
    internal MainFrameButtonManager(MainMenuFrame parentFrame) : base(parentFrame)
    {

    }


    // Internal methods.
    internal void EnableMainButtons()
    {
        _play.IsClickable = true;
        _editor.IsClickable = true;
        _options.IsClickable = true;
        _exit.IsClickable = true;
    }

    internal void DisableMainButtons()
    {
        _play.IsClickable = false;
        _editor.IsClickable = false;
        _options.IsClickable = false;
        _exit.IsClickable = false;
    }

    internal void BringMainButtonsIn()
    {
        EnableMainButtons();
        AnimateMainButtons(X_LOCATION_BUTTON_OUT, X_LOCATION_BUTTON_IN);
    }

    internal void BringMainButtonsOut()
    {
        AnimateMainButtons(X_LOCATION_BUTTON_IN, X_LOCATION_BUTTON_OUT);
        _exit.Position.AnimationFinished += OnExitButtonBroughtOutEvent;
    }


    // Private methods.
    private void CreateButtons()
    {
        _play = ConnectorButton.CreateNormal(ParentFrame, Direction.Right, Vector2.Zero, 
            new Vector2(0.5f, 0.5f), ParentFrame.LayerManager.UILayer);
        _play.Text = "Play";

        _editor = ConnectorButton.CreateNormal(ParentFrame, Direction.Right, Vector2.Zero, 
            new Vector2(0.5f, 0.5f),ParentFrame.LayerManager.UILayer);
        _editor.Text = "Editor";

        _options = ConnectorButton.CreateNormal(ParentFrame, Direction.Right, Vector2.Zero, 
            new Vector2(0.5f, 0.5f), ParentFrame.LayerManager.UILayer);
        _options.Text = "Options";

        _exit = ConnectorButton.CreateNormal(ParentFrame, Direction.Right, Vector2.Zero, 
            new Vector2(0.5f, 0.5f), ParentFrame.LayerManager.UILayer);
        _exit.ClickHandler += OnExitClickEvent;
        _exit.Text = "Exit";
    }

    private void AnimateMainButtons(float xStart, float xEnd)
    {
        _play.Position.Finish();
        _editor.Position.Finish();
        _options.Position.Finish();
        _exit.Position.Finish();

        _play.IsEnabled = true;
        _editor.IsEnabled = true;
        _options.IsEnabled = true;
        _exit.IsEnabled = true;

        _play.Position.SetKeyFrames(new KeyFrameBuilder(
            new(xStart, Y_LOCATION_BUTTON_PLAY), InterpolationMethod.EaseOut)
            .AddKeyFrame(new(xEnd, Y_LOCATION_BUTTON_PLAY), 0.4d));
        _play.Position.Start();

        _editor.Position.SetKeyFrames(new KeyFrameBuilder(
            new(xStart, Y_LOCATION_BUTTON_EDITOR), InterpolationMethod.EaseOut)
            .AddKeyFrame(0.05d)
            .AddKeyFrame(new(xEnd, Y_LOCATION_BUTTON_EDITOR), 0.4d));
        _editor.Position.Start();

        _options.Position.SetKeyFrames(new KeyFrameBuilder(
            new(xStart, Y_LOCATION_BUTTON_OPTIONS), InterpolationMethod.EaseOut)
            .AddKeyFrame(0.1d)
            .AddKeyFrame(new(xEnd, Y_LOCATION_BUTTON_OPTIONS), 0.4d));
        _options.Position.Start();

        _exit.Position.SetKeyFrames(new KeyFrameBuilder(
            new(xStart, Y_LOCATION_BUTTON_EXIT), InterpolationMethod.EaseOut)
            .AddKeyFrame(0.15d)
            .AddKeyFrame(new(xEnd, Y_LOCATION_BUTTON_EXIT), 0.4d));
        _exit.Position.Start();
    }

    /* Click handlers */
    private void OnExitClickEvent(object? sender, EventArgs args)
    {
        BringMainButtonsOut();
        _exit.Position.AnimationFinished += (sender, args) => GH.Engine.Exit();
        ParentFrame.LayerManager.FadeStep = -4d;
        DisableMainButtons();
    }


    /* Animation handlers. */
    private void OnExitButtonBroughtOutEvent(object? sender, EventArgs args)
    {
        _exit.Position.AnimationFinished -= OnExitButtonBroughtOutEvent;

        _play.IsEnabled = false;
        _editor.IsEnabled = false;
        _options.IsEnabled = false;
        _exit.IsEnabled = false;
    }


    // Inherited methods.
    internal override void Load(AssetManager assetManager)
    {
        ConnectorButton.LoadAllAssets(assetManager, ParentFrame);
    }

    internal override void OnStart()
    {
        CreateButtons();
        BringMainButtonsIn();
    }
}