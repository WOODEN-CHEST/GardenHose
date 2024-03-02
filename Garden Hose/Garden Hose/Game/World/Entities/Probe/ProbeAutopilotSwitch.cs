using GardenHose.Game.GameAssetManager;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeAutopilotSwitch : ProbeSystemComponent
{
    // Internal static fields.
    internal static Vector2 PANEL_SIZE { get; } = new(251f, 52f);
    internal static Vector2 KNOB_SIZE { get; } = new(37f, 39f);


    // Internal fields.
    internal override Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            _panel.Position = value;
            _knob.Position = new Vector2(value.X + _targetKnobOffsetX, value.Y + 20f);
        }
    }

    internal ProbeAutopilotState State
    {
        get => _state;
        set 
        {
            _state = value;
            _targetKnobOffsetX = value switch
            {
                ProbeAutopilotState.Disabled => -90f,
                ProbeAutopilotState.StayStationary => 0f,
                ProbeAutopilotState.FollowDirection => 90f,
                _ => throw new EnumValueException(nameof(value), value)
            };
        }
    }

    internal event EventHandler<ProbeAutopilotState>? AutopilotChange;


    // Private fields.
    private SpriteButton _panel;
    private SpriteItem _knob;

    private ProbeAutopilotState _state;
    private float _targetKnobOffsetX = 0f;
    private const float KNOB_MOVEMENT_SPEED = 2.2f;


    // Constructors.
    internal ProbeAutopilotSwitch(ProbeAutopilotState initialState)
    {
        State = initialState;
    }


    // Inherited methods.
    internal override void Draw(IDrawInfo time)
    {
        _panel.Draw(time);
        _knob.Draw(time);
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        _panel = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_AutopilotPanel).CreateInstance(), PANEL_SIZE);
        _panel.SetHandler(ButtonEvent.RightClick, OnPanelClickEvent);
        _knob = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_AutopilotKnob).CreateInstance(), KNOB_SIZE);
    }

    internal override void Tick(GHGameTime time, ProbeEntity probe, bool isComponentPowered)
    {
        if (probe.Pilot == Ship.SpaceshipPilot.Player)
        {
            _panel.Update(time.WorldTime);
        }

        _knob.Position += new(, 0f) * time.WorldTime.PassedTimeSeconds * KNOB_MOVEMENT_SPEED;
    }


    // Private methods.
    private void OnPanelClickEvent(object? sender, EventArgs args)
    {
        float RelativeXPos = (UserInput.VirtualMousePosition.Current.X - (Position.X - PANEL_SIZE.X * 0.5f)) / PANEL_SIZE.X;

        if (RelativeXPos <= 1f / 3f)
        {
            AutopilotChange?.Invoke(this, ProbeAutopilotState.Disabled);
        }
        else if (RelativeXPos <= 2f / 3f)
        {
            AutopilotChange?.Invoke(this, ProbeAutopilotState.StayStationary);
        }
        else
        {
            AutopilotChange?.Invoke(this, ProbeAutopilotState.FollowDirection);
        }
    }
}