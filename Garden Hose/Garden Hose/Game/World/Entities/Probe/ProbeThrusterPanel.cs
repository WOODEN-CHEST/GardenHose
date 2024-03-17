using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Ship;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Probe;


internal class ProbeThrusterPanel : ProbeSystemComponent
{
    // Internal static fields.
    internal static Vector2 PANEL_SIZE { get; } = new(209f, 230f);


    // Internal fields.
    internal float ObservedMTValue => _mainIndicator.Value;
    internal float ObservedRTValue => _rightIndicator.Value;
    internal float ObservedLTValue => _leftIndicator.Value;
    internal override Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            _panel.Position = value;

            Vector2 FirstPadding = new(10f, 6f);
            Vector2 SubsequentPadding = new(0, 5f);

            _leftIndicator.Position = value - (PANEL_SIZE * 0.5f) + (ProbeThrusterIndicator.LIGHT_PANEL_SIZE *0.5f)
                + FirstPadding;

            _mainIndicator.Position = _leftIndicator.Position + (ProbeThrusterIndicator.LIGHT_PANEL_SIZE * new Vector2(0f, 1f))
                + SubsequentPadding;

            _rightIndicator.Position = _mainIndicator.Position + (ProbeThrusterIndicator.LIGHT_PANEL_SIZE * new Vector2(0f, 1f))
                + SubsequentPadding;
        }
    }

    internal event EventHandler<EngineButtonSwitchEventArgs>? EngineSwitch;


    // Private fields.
    private readonly ProbeThrusterIndicator _leftIndicator;
    private readonly ProbeThrusterIndicator _mainIndicator;
    private readonly ProbeThrusterIndicator _rightIndicator;
    private SpriteItem _panel;



    // Constructors.
    internal ProbeThrusterPanel(bool leftEngineState, bool mainEngineState, bool rightEngineState)
    {
        _leftIndicator = new(OnEngineSwitchEvent, leftEngineState);
        _mainIndicator = new(OnEngineSwitchEvent, mainEngineState);
        _rightIndicator = new(OnEngineSwitchEvent, rightEngineState);
    }


    // Internal methods.
    internal override void Tick(GHGameTime time, ProbeEntity probe, bool isComponentPowered)
    {
        _leftIndicator.Tick(time, probe, isComponentPowered);
        _mainIndicator.Tick(time, probe, isComponentPowered);
        _rightIndicator.Tick(time, probe, isComponentPowered);

        if (isComponentPowered)
        {
            _leftIndicator.Value = probe.LeftThrusterPart?.CurrentThrusterThrottle ?? 0f;
            _mainIndicator.Value = probe.MainThrusterPart?.CurrentThrusterThrottle ?? 0f;
            _rightIndicator.Value = probe.RightThrusterPart?.CurrentThrusterThrottle ?? 0f;
        }
        else
        {
            _leftIndicator.Value = 0f;
            _mainIndicator.Value = 0f;
            _rightIndicator.Value = 0f;
        }
        
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        _panel = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanel).CreateInstance(), PANEL_SIZE);
        _leftIndicator.Load(assetManager);
        _mainIndicator.Load(assetManager);
        _rightIndicator.Load(assetManager); 
    }

    internal override void Draw(IDrawInfo info)
    {
        _panel.Draw(info);
        _leftIndicator?.Draw(info);
        _mainIndicator?.Draw(info);
        _rightIndicator?.Draw(info);
    }

    internal void SetLeftThrusterPanelState(bool state) => _leftIndicator.SwitchState = state;

    internal void SetMainThrusterPanelState(bool state) => _mainIndicator.SwitchState = state;

    internal void SetRightThrusterPanelState(bool state) => _rightIndicator.SwitchState = state;


    // Private methods.
    private void OnEngineSwitchEvent(object? sender, bool switchValue)
    {
        if  (sender == _leftIndicator)
        {
            EngineSwitch?.Invoke(this, new(switchValue, ProbeThruster.LeftThruster));
        }
        else if (sender == _mainIndicator)
        {
            EngineSwitch?.Invoke(this, new(switchValue, ProbeThruster.MainThruster));
        }
        else
        {
            EngineSwitch?.Invoke(this, new(switchValue, ProbeThruster.RightThruster));
        }
    }
}