using GardenHose.Game.AssetManager;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using GardenHoseEngine.IO;
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
        _panelSprite = new(assetManager.GetAnimation("ship_probe_thrusterpanel")!);
        _panelSprite.TargetTextureSize = new Vector2(150, 168);

        Vector2 PanelPosition = new Vector2(10f, Display.VirtualSize.Y - 10f);
        PanelPosition.X += _panelSprite.TextureSize.X * 0.5f;
        PanelPosition.Y -= _panelSprite.TextureSize.Y * 0.5f;
        _panelSprite.Position.Vector = PanelPosition;

        _buttonModeManual = new(assetManager.GetAnimation("ship_probe_thrusterpanelbutton").CreateInstance(), new Vector2(44f, 40f));
        _buttonModeStay = new(assetManager.GetAnimation("ship_probe_thrusterpanelbutton").CreateInstance(), new Vector2(44f, 40f));
        _buttonModeStay.Sprite.ActiveAnimation.FrameIndex = 1;
        _buttonModeFollow = new(assetManager.GetAnimation("ship_probe_thrusterpanelbutton").CreateInstance(), new Vector2(44f, 40f));
        _buttonModeFollow.Sprite.ActiveAnimation.FrameIndex = 2;

        _buttonModeManual.Position = PanelPosition + new Vector2(40.5f, -56f);
        _buttonModeStay.Position = PanelPosition + new Vector2(40.5f, -1.5f);
        _buttonModeFollow.Position = PanelPosition + new Vector2(40.5f, 53f);

        SetInputListeners(_system.Ship.Pilot == Ship.SpaceshipPilot.Player);
        SetAutopilotState(_system.AutopilotState);
    }

    internal void Draw()
    {
        _panelSprite.Draw();
        _buttonModeManual.Draw();
        _buttonModeStay.Draw();
        _buttonModeFollow.Draw();
    }

    internal void SetInputListeners(bool areEnabled)
    {
        if (areEnabled)
        {
            _buttonModeManual?.Button.SetEventHandler(ButtonEvent.RightClick, OnManualAutopilotStatePressEvent);
            _buttonModeStay?.Button.SetEventHandler(ButtonEvent.RightClick, OnStayAutopilotStatePressEvent);
            _buttonModeFollow?.Button.SetEventHandler(ButtonEvent.RightClick, OnFollowAutopilotStatePressEvent);
        }
        else
        {
            _buttonModeManual?.Button.ClearEventHandlers();
            _buttonModeStay?.Button.ClearEventHandlers();
            _buttonModeFollow?.Button.ClearEventHandlers();
        }
    }

    internal void SetAutopilotState(ProbeAutopilotState state)
    {
        _system.AutopilotState = state;

        _buttonModeManual.Sprite.Mask = MODE_COLOR_OFF;
        _buttonModeStay.Sprite.Mask = MODE_COLOR_OFF;
        _buttonModeFollow.Sprite.Mask = MODE_COLOR_OFF;

        switch (state)
        {
            case ProbeAutopilotState.Disabled:
                _buttonModeManual.Sprite.Mask = MODE_COLOR_ON;
                break;

            case ProbeAutopilotState.StayStationary:
                _buttonModeStay.Sprite.Mask = MODE_COLOR_ON;
                break;

            case ProbeAutopilotState.FollowDirection:
                _buttonModeFollow.Sprite.Mask = MODE_COLOR_ON;
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