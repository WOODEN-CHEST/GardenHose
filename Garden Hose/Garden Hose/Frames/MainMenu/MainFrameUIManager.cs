﻿using GardenHose.Frames.InGame;
using GardenHose.UI.Buttons.Connector;
using GardenHoseEngine;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Frames.MainMenu;

internal class MainFrameUIManager : FrameComponentManager<MainMenuFrame>
{
    // Private fields.
    private readonly Vector2 FIRST_BUTTON_PADDING = new(50f, 0f);
    private readonly Vector2 SEQUENTIAL_BUTTON_PADDING = new(0f, 50f);
    private readonly float FIRST_BUTTON_Y_POSITION = Display.VirtualSize.X * 0.25f;
    private const float MAIN_BUTTON_SCALE = 0.5f;

    private readonly ILayer _uiLayer;

    private ConnectorAssetCollection _connectorAssets = new();
    private ConnectorRectangleButton _play;
    //private ConnectorRectangleButton _editor;
    //private ConnectorRectangleButton _options;
    private ConnectorRectangleButton _exit;


    // Constructors.
    internal MainFrameUIManager(MainMenuFrame parentFrame, ILayer uiLayer) : base(parentFrame)
    {
        _uiLayer = uiLayer ?? throw new ArgumentNullException(nameof(uiLayer));
    }


    // Internal methods.
    internal void SetMainButtonClickability(bool clickability)
    {
        _play.IsClickable = clickability;
        //_editor.IsClickable = clickability;
        //_options.IsClickable = clickability;
        _exit.IsClickable = clickability;
    }


    // Private methods.
    private void CreateButtons()
    {
        _play = new(Direction.Right, RectangleButtonType.Normal, _connectorAssets)
        {
            Text = "Play",
            Scale = MAIN_BUTTON_SCALE,
            Position = new Vector2(ConnectorRectangleButton.NORMAL_BUTTON_SIZE.X * 0.5f * MAIN_BUTTON_SCALE, FIRST_BUTTON_Y_POSITION)
                + FIRST_BUTTON_PADDING
        };
        _play.Click += OnPlayClickEvent;
        _uiLayer.AddDrawableItem(_play);
        ParentFrame.AddItem(_play);

        //_editor = new(Direction.Right, RectangleButtonType.Normal, _connectorAssets)
        //{
        //    Text = "Level Editor",
        //    Scale = MAIN_BUTTON_SCALE,
        //    Position = _play.Position + new Vector2(0f, ConnectorRectangleButton.NORMAL_BUTTON_SIZE.Y * MAIN_BUTTON_SCALE)
        //        + SEQUENTIAL_BUTTON_PADDING
        //};
        //_uiLayer.AddDrawableItem(_editor);
        //ParentFrame.AddItem(_editor);

        //_options = new(Direction.Right, RectangleButtonType.Normal, _connectorAssets)
        //{
        //    Text = "Options",
        //    Scale = MAIN_BUTTON_SCALE,
        //    Position = _editor.Position + new Vector2(0f, ConnectorRectangleButton.NORMAL_BUTTON_SIZE.Y * MAIN_BUTTON_SCALE)
        //        + SEQUENTIAL_BUTTON_PADDING
        //};
        //_uiLayer.AddDrawableItem(_options);
        //ParentFrame.AddItem(_options);

        _exit = new(Direction.Right, RectangleButtonType.Normal, _connectorAssets)
        {
            Text = "Exit",
            Scale = MAIN_BUTTON_SCALE,
            Position = _play.Position + new Vector2(0f, ConnectorRectangleButton.NORMAL_BUTTON_SIZE.Y * MAIN_BUTTON_SCALE)
                + SEQUENTIAL_BUTTON_PADDING
        };
        _exit.Click += OnExitClickEvent;
        _uiLayer.AddDrawableItem(_exit);
        ParentFrame.AddItem(_exit);
    }


    /* Click handlers */
    private void OnExitClickEvent(object? sender, EventArgs args)
    {
        SetMainButtonClickability(false);
        GHEngine.Exit();
    }

    private void OnPlayClickEvent(object? sender, EventArgs args)
    {
        SetMainButtonClickability(false);
        GameFrameManager.LoadNextFrame(new InGameFrame("In-Game"), 
            () => GameFrameManager.JumpToNextFrame());
    }


    // Inherited methods.
    internal override void Load()
    {
        _connectorAssets.LoadAllAssets(ParentFrame);
    }

    internal override void OnStart()
    {
        CreateButtons();
    }

    internal override void OnEnd()
    {

    }
}