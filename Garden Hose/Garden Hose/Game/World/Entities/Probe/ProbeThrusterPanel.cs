using GardenHose.Game.GameAssetManager;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeThrusterPanel
{
    // Private static fields.
    private static Color MODE_COLOR_OFF = new Color(184, 39, 39);
    private static Color MODE_COLOR_ON = new Color(108, 240, 110);

    // Private fields.
    private ProbeSystem _system;

    private SpriteItem _panelSprite;

    private SpriteButton _buttonModeManual;
    private SpriteButton _buttonModeStay;
    private SpriteButton _buttonModeFollow;


    // Constructors.
    internal ProbeThrusterPanel(ProbeSystem system)
    {
        _system = system;
    }


    // Methods.
    internal void Load(GHGameAssetManager assetManager)
    {
        _panelSprite = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanel).CreateInstance(), new Vector2(150, 168));

        Vector2 PanelPosition = new Vector2(10f, Display.VirtualSize.Y - 10f);
        PanelPosition.X += _panelSprite.TextureSize.X * 0.5f;
        PanelPosition.Y -= _panelSprite.TextureSize.Y * 0.5f;
        _panelSprite.Position = PanelPosition;

        _buttonModeManual = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelButton).CreateInstance(), new Vector2(44f, 40f));
        _buttonModeStay = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelButton).CreateInstance(), new Vector2(44f, 40f));
        _buttonModeStay.ActiveAnimation.FrameIndex = 1;
        _buttonModeFollow = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelButton).CreateInstance(), new Vector2(44f, 40f));
        _buttonModeFollow.ActiveAnimation.FrameIndex = 2;

        _buttonModeManual.Position = PanelPosition + new Vector2(40.5f, -56f);
        _buttonModeStay.Position = PanelPosition + new Vector2(40.5f, -1.5f);
        _buttonModeFollow.Position = PanelPosition + new Vector2(40.5f, 53f);

        SetInputListeners(_system.Ship.Pilot == Ship.SpaceshipPilot.Player);
        SetAutopilotState(_system.AutopilotState);
    }

    internal void Draw(IDrawInfo info)
    {
        _panelSprite.Draw(info);
        _buttonModeManual.Draw(info);
        _buttonModeStay.Draw(info);
        _buttonModeFollow.Draw(info);
    }

    internal void SetInputListeners(bool areEnabled)
    {
        if (areEnabled)
        {
            _buttonModeManual?.SetHandler(ButtonEvent.RightClick, OnManualAutopilotStatePressEvent);
            _buttonModeStay?.SetHandler(ButtonEvent.RightClick, OnStayAutopilotStatePressEvent);
            _buttonModeFollow?.SetHandler(ButtonEvent.RightClick, OnFollowAutopilotStatePressEvent);
        }
        else
        {
            _buttonModeManual?.ClearHandlers();
            _buttonModeStay?.ClearHandlers();
            _buttonModeFollow?.ClearHandlers();
        }
    }

    internal void SetAutopilotState(ProbeAutopilotState state)
    {
        _system.AutopilotState = state;

        _buttonModeManual.Mask = MODE_COLOR_OFF;
        _buttonModeStay.Mask = MODE_COLOR_OFF;
        _buttonModeFollow.Mask = MODE_COLOR_OFF;

        switch (state)
        {
            case ProbeAutopilotState.Disabled:
                _buttonModeManual.Mask = MODE_COLOR_ON;
                break;

            case ProbeAutopilotState.StayStationary:
                _buttonModeStay.Mask = MODE_COLOR_ON;
                break;

            case ProbeAutopilotState.FollowDirection:
                _buttonModeFollow.Mask = MODE_COLOR_ON;
                break;

            default:
                throw new EnumValueException(nameof(state), state);
        }
    }


    // Private methods

    private void OnManualAutopilotStatePressEvent(object? sender, EventArgs args)
    {
        SetAutopilotState(ProbeAutopilotState.Disabled);
    }

    private void OnStayAutopilotStatePressEvent(object? sender, EventArgs args)
    {
        SetAutopilotState(ProbeAutopilotState.StayStationary);
    }

    private void OnFollowAutopilotStatePressEvent(object? sender, EventArgs args)
    {
        SetAutopilotState(ProbeAutopilotState.FollowDirection);
    }
}