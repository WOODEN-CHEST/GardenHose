using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Ship;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeErrorHandler
{
    // Internal static fields.
    internal static Vector2 PANEL_SIZE { get; } = new(250f, 82.7f); 
    internal static Vector2 GLASS_SIZE { get; } = PANEL_SIZE * 0.95f - new Vector2(0f, 5f);
    internal static Vector2 THRUSTER_INDICATOR_SIZE { get; } = new(24.8f, 15.6f); 
    internal static Vector2 SPIN_SIZE { get; } = new(47.7f);
    internal static Vector2 FUEL_SIZE { get; } = new(36.76f, 48.7f);
    internal static Vector2 OXYGEN_SIZE { get; } = new(51f);
    internal static Vector2 LEAK_SIZE { get; } = new(38.59f, 53.3f);

    internal static Color COLOR_CLEAR { get; } = new(0.4f, 0.4f, 0.4f);
    internal static Color COLOR_ERROR { get; } = new(0.9f, 0.18f, 0.18f);


    // Internal fields.
    internal Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;

            _panel.Position = value;
            _glass.Position = value;

            Vector2 XPadding = new(6f, 0f);
            Vector2 YPadding = new(0f, 6f);
            _leftThruster.Position = value - (GLASS_SIZE * 0.5f) + (THRUSTER_INDICATOR_SIZE * 0.5f) + XPadding + YPadding;
            _mainThruster.Position = _leftThruster.Position + new Vector2(0f, THRUSTER_INDICATOR_SIZE.Y) + YPadding;
            _rightThruster.Position = _mainThruster.Position + new Vector2(0f, THRUSTER_INDICATOR_SIZE.Y) + YPadding;

            _spin.Position = _mainThruster.Position + (THRUSTER_INDICATOR_SIZE * new Vector2(0.5f, 0f))
                + (SPIN_SIZE * new Vector2(0.5f, 0f)) + XPadding;
            _fuel.Position = _spin.Position + (FUEL_SIZE * new Vector2(0.5f, 0f))
                + (SPIN_SIZE * new Vector2(0.5f, 0f)) + XPadding;
            _oxygen.Position = _fuel.Position + (OXYGEN_SIZE * new Vector2(0.5f, 0f))
                + (SPIN_SIZE * new Vector2(0.5f, 0f)) + XPadding;
            _leak.Position = _oxygen.Position + (LEAK_SIZE * new Vector2(0.5f, 0f))
                + (SPIN_SIZE * new Vector2(0.5f, 0f)) + XPadding;
        }
    }

    internal bool IsInSpin { get; private set; } = false;
    internal bool IsFuelLeaked { get; private set; } = false;
    internal bool IsFuelLow { get; private set; } = false;
    internal bool IsOxygenLow { get; private set; } = false;
    internal bool IsMainThrusterDamaged { get; private set; } = false;
    internal bool IsLeftThrusterDamaged { get; private set; } = false;
    internal bool IsRightThrusterDamaged { get; private set; } = false;


    // Private fields.
    private Vector2 _position;

    private SpriteItem _panel;
    private SpriteItem _glass;
    private SpriteItem _leftThruster;
    private SpriteItem _mainThruster;
    private SpriteItem _rightThruster;
    private SpriteItem _spin;
    private SpriteItem _fuel;
    private SpriteItem _oxygen;
    private SpriteItem _leak;



    // Constructors.
    internal ProbeErrorHandler() { }


    // Internal methods.
    internal void Tick(ProbeEntity probe)
    {
        // Set errors.
        IsInSpin = Math.Abs(probe.AngularMotion) > MathF.PI * 1.5f;
        IsFuelLeaked = (probe.MainThrusterPart?.IsLeaking ?? false) || (probe.LeftThrusterPart?.IsLeaking ?? false)
            || (probe.RightThrusterPart?.IsLeaking ?? false);

        IsOxygenLow = probe.Oxygen <= SpaceshipEntity.MAX_OXYGEN / 2f;

        IsMainThrusterDamaged = (probe.MainThrusterPart == null) || (probe.MainThrusterPart.MaterialInstance.CurrentStrength <=
            probe.MainThrusterPart.MaterialInstance.Material.Strength * 0.65f);

        IsLeftThrusterDamaged = (probe.LeftThrusterPart == null) || (probe.LeftThrusterPart.MaterialInstance.CurrentStrength <=
            probe.LeftThrusterPart.MaterialInstance.Material.Strength * 0.65f);

        IsRightThrusterDamaged = (probe.RightThrusterPart == null) || (probe.RightThrusterPart.MaterialInstance.CurrentStrength <=
            probe.RightThrusterPart.MaterialInstance.Material.Strength * 0.65f);

        float MaxFuel = (probe.MainThrusterPart?.MaxFuel ?? 0f) + (probe.LeftThrusterPart?.MaxFuel ?? 0f)
            + (probe.RightThrusterPart?.MaxFuel ?? 0f);
        if (MaxFuel == 0f)
        {
            IsFuelLow = false;
        }
        else
        {
            float Fuel = (probe.MainThrusterPart?.Fuel ?? 0f) + (probe.LeftThrusterPart?.Fuel ?? 0f)
                + (probe.RightThrusterPart?.Fuel ?? 0f);
            IsFuelLow = Fuel / MaxFuel <= 0.2f;
        }


        // Set colors.
        _leftThruster.Mask = IsLeftThrusterDamaged ? COLOR_ERROR : COLOR_CLEAR;
        _mainThruster.Mask = IsMainThrusterDamaged ? COLOR_ERROR : COLOR_CLEAR;
        _rightThruster.Mask = IsRightThrusterDamaged ? COLOR_ERROR : COLOR_CLEAR;
        _spin.Mask = IsInSpin ? COLOR_ERROR : COLOR_CLEAR;
        _fuel.Mask = IsFuelLow ? COLOR_ERROR : COLOR_CLEAR;
        _oxygen.Mask = IsOxygenLow ? COLOR_ERROR : COLOR_CLEAR;
        _leak.Mask = IsFuelLeaked ? COLOR_ERROR : COLOR_CLEAR;
    }

    internal void Load(GHGameAssetManager assetManager)
    {
        _panel = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ErrorPanel).CreateInstance(), PANEL_SIZE);
        _glass = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ErrorGlass).CreateInstance(), GLASS_SIZE);
        _leftThruster = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ErrorLT).CreateInstance(), THRUSTER_INDICATOR_SIZE);
        _mainThruster = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ErrorMT).CreateInstance(), THRUSTER_INDICATOR_SIZE);
        _rightThruster = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ErrorRT).CreateInstance(), THRUSTER_INDICATOR_SIZE);
        _spin = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ErrorSpin).CreateInstance(), SPIN_SIZE);
        _fuel = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ErrorFuel).CreateInstance(), FUEL_SIZE);
        _oxygen = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ErrorOxygen).CreateInstance(), OXYGEN_SIZE);
        _leak = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ErrorLeak).CreateInstance(), LEAK_SIZE);
    }

    internal void Draw(IDrawInfo info)
    {
        _panel.Draw(info);
        _leftThruster.Draw(info);
        _mainThruster.Draw(info);
        _rightThruster.Draw(info);
        _spin.Draw(info);
        _fuel.Draw(info);
        _oxygen.Draw(info);
        _leak.Draw(info);
        _glass.Draw(info);
    }
}