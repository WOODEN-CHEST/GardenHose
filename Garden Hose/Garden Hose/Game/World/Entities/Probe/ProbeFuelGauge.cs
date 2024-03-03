using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeFuelGauge : ProbeSystemComponent
{
    // Internal static fields.
    internal static Vector2 PANEL_SIZE { get; } = new(126.5f, 107f);
    internal static Vector2 GLASS_SIZE { get; } = new(119f, 101f);
    internal static Vector2 INDICATOR_TOP_SIZE { get; } = new(10.5f, 47f);
    internal static Vector2 INDICATOR_BOTTOM_SIZE { get; } = new(10.5f, 5f);
    internal static Vector2 INDICATOR_SHADOW_TOP_SIZE { get; } = new(13.5f, 44f);
    internal static Vector2 INDICATOR_SHADOW_BOTTOM_SIZE { get; } = new(14.5f, 10.5f);


    // Internal fields.
    internal override Vector2 Position 
    {
        get => base.Position;
        set
        {
            base.Position = value;

            _panel.Position = value;
            _glass.Position = value;
            _indicatorTop.Position = _panel.Position + new Vector2(0f, PANEL_SIZE.X * 0.1f);
            _indicatorBottom.Position = _indicatorTop.Position;
            _indicatorShadowTop.Position = _indicatorTop.Position;
            _indicatorShadowBottom.Position = _indicatorTop.Position;

            _indicatorTop.Rotation = -INDICATOR_MAX_ROTATION + (INDICATOR_MAX_ROTATION * 2f) * IndictedFuel;
        }
    }

    internal float IndictedFuel
    {
        get => _indictedFuel;
        set
        {
            _indictedFuel = Math.Clamp(value, 0.0f, 1.0f);
        }
    }


    // Private fields.
    private SpriteItem _panel;
    private SpriteItem _glass;
    private SpriteItem _indicatorTop;
    private SpriteItem _indicatorBottom;
    private SpriteItem _indicatorShadowTop;
    private SpriteItem _indicatorShadowBottom;

    private float _indictedFuel;

    private const float INDICATOR_MAX_ROTATION = MathF.PI * 0.7f;
    private const float INDICATOR_ROATION_SPEED = 4f;



    // Constructors.
    internal ProbeFuelGauge(float initialFuel)
    {
        IndictedFuel = initialFuel;
    }


    // Inherited methods.
    internal override void Draw(IDrawInfo time)
    {
        _panel.Draw(time);
        _indicatorShadowTop.Draw(time);
        _indicatorShadowBottom.Draw(time);
        _indicatorTop.Draw(time);
        _indicatorBottom.Draw(time);
        _glass.Draw(time);
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        _panel = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_FuelGaugePanel).CreateInstance(), PANEL_SIZE);
        _glass = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_FuelGaugeGlass).CreateInstance(), GLASS_SIZE);

        _indicatorTop = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_FuelGaugeIndicatorTop).CreateInstance(),
            INDICATOR_TOP_SIZE);

        _indicatorBottom = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_FuelGaugeIndicatorBottom).CreateInstance(),
            INDICATOR_BOTTOM_SIZE);

        _indicatorShadowTop = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_FuelGaugeIndicatorShadowTop).CreateInstance(), 
            INDICATOR_SHADOW_TOP_SIZE);

        _indicatorShadowBottom = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_FuelGaugeIndicatorShadowBottom).CreateInstance(),
            INDICATOR_SHADOW_BOTTOM_SIZE);
    }

    internal override void Tick(GHGameTime time, ProbeEntity probe, bool isComponentPowered)
    {
        if (!isComponentPowered)
        {
            return;
        }

        float MaxFuel = (probe.LeftThrusterPart?.MaxFuel ?? 0f) + (probe.MainThrusterPart?.MaxFuel ?? 0f)
            + (probe.RightThrusterPart?.MaxFuel ?? 0f);
        float Fuel = (probe.LeftThrusterPart?.Fuel ?? 0f) + (probe.MainThrusterPart?.Fuel ?? 0f)
            + (probe.RightThrusterPart?.Fuel ?? 0f);

        if (Fuel <= 0)
        {
            IndictedFuel = 0f;
        }
        else
        {
            IndictedFuel = Fuel / MaxFuel;
        }

        _indicatorTop.Rotation += ((-INDICATOR_MAX_ROTATION + (INDICATOR_MAX_ROTATION * 2f) * IndictedFuel) - _indicatorTop.Rotation)
            * time.WorldTime.PassedTimeSeconds * INDICATOR_ROATION_SPEED;
        _indicatorBottom.Rotation = _indicatorTop.Rotation;
        _indicatorShadowBottom.Rotation = _indicatorTop.Rotation;
        _indicatorShadowTop.Rotation = _indicatorTop.Rotation;
    }
}