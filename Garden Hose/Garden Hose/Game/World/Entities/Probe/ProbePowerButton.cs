using GardenHose.Game.GameAssetManager;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbePowerButton : ProbeSystemComponent
{
    // Internal static fields.
    internal static Vector2 PANEL_SIZE { get; } = new(123f, 44f);
    internal static Vector2 BUTTON_SIZE { get; } = new(119f, 40f);
    internal static Vector2 BUTTON_SHADOW_SIZE { get; } = new(119.5f, 41f);
    internal static Color ON_COLOR { get; } = new(0.4f, 1f, 0.4f);
    internal static Color STARTING_UP_COLOR { get; } = new(1f, 0.737f, 0.122f);
    internal static Color OFF_COLOR { get; } = new(0.9f, 0.9f, 0.9f);


    // Internal fields.
    internal override Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            _panel.Position = value;
            _button.Position = value;
            _buttonShadow.Position = value + (BUTTON_SIZE * new Vector2(0.5f, -0.5f));
        }
    }

    internal ProbeSystemState SystemState
    {
        get => _powerState;
        set
        {
            if (value == _powerState)
            {
                return;
            }

            _powerState = value;
            UpdateColor();
        }
    }

    internal event EventHandler? PowerSwitch;


    // Private fields.
    private ProbeSystemState _powerState;

    private SpriteButton _panel;
    private SpriteItem _button;
    private SpriteItem _buttonShadow;


    // Constructors.
    internal ProbePowerButton(ProbeSystemState initialState)
    {
        _powerState = initialState;
    }


    // Inherited methods.
    internal override void Draw(IDrawInfo time)
    {
        _panel.Draw(time);
        _buttonShadow.Draw(time);
        _button.Draw(time); 
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        _panel = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_PowerPanel).CreateInstance(), PANEL_SIZE);
        _button = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_PowerButton).CreateInstance(), BUTTON_SIZE);
        _buttonShadow = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_PowerButtonShadow).CreateInstance(), BUTTON_SHADOW_SIZE);
        UpdateColor();

        _panel.SetHandler(ButtonEvent.RightClick, (sender, args) =>
        {
            PowerSwitch?.Invoke(this, EventArgs.Empty);
        });
    }

    internal override void Tick(GHGameTime time, ProbeEntity probe, bool isComponentPowered)
    {
        if (probe.Pilot == Ship.SpaceshipPilot.Player)
        {
            _panel.Update(time.WorldTime);
        }
    }


    // Private methods.
    private void UpdateColor()
    {
        _button.Mask = SystemState switch
        {
            ProbeSystemState.Off => OFF_COLOR,
            ProbeSystemState.StartingUp => STARTING_UP_COLOR,
            ProbeSystemState.On => ON_COLOR,
            _ => throw new EnumValueException(nameof(SystemState), SystemState)
        };
    }
}