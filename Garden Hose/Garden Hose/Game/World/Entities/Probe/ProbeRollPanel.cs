using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeRollPanel : ProbeSystemComponent
{
    // Internal static fields.
    internal static Vector2 PANEL_SIZE { get; } = new(150f, 150f);
    internal static Vector2 GLASS_SIZE { get; } = PANEL_SIZE * 0.905f;
    internal static Vector2 DISPLAY_SIZE { get; } = GLASS_SIZE;
    internal static Vector2 INDICATOR_SIZE { get; } = new(40f, 24.96f);
    internal static Vector2 INDICATOR_SHADOW_SIZE { get; } = INDICATOR_SIZE * 1.15f;


    // Internal fields.
    internal float IndicatedRoll {  get; private set; }

    internal override Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;

            _panel.Position = value;
            _display.Position = value;
            _glass.Position = value;
            _indicator.Position = value - new Vector2(0f, _indicator.Size.Y * 0.5f);
            _indicatorShadow.Position = _indicator.Position + new Vector2(_indicator.Size.X, -_indicator.Size.Y) * 0.5f;
        }
    }


    // Private fields.
    private SpriteItem _panel;
    private SpriteItem _glass;
    private SpriteItem _display;
    private SpriteItem _indicator;
    private SpriteItem _indicatorShadow;

    private const float ROLL_UPDATE_SPEED = 5f;


    // Internal methods.
    internal override void Tick(GHGameTime time, ProbeEntity probe, bool isComponentPowered)
    {
        if (isComponentPowered)
        {
            IndicatedRoll += MathF.Asin(MathF.Sin(probe.CommonMath.Roll - IndicatedRoll)) * time.WorldTime.PassedTimeSeconds * ROLL_UPDATE_SPEED;
            _display.Rotation = IndicatedRoll;
        }
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        _panel = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_RollPanel).CreateInstance(), PANEL_SIZE);
        _glass = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_RollGlass).CreateInstance(), GLASS_SIZE);
        _display = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_RollDisplay).CreateInstance(), DISPLAY_SIZE);
        _indicator = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_RollIndicator).CreateInstance(), INDICATOR_SIZE);
        _indicatorShadow = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_RollIndicatorShadow).CreateInstance(),
            INDICATOR_SHADOW_SIZE);
    }

    internal override void Draw(IDrawInfo info)
    {
        _display.Draw(info);
        _indicatorShadow.Draw(info);
        _indicator.Draw(info);
        _panel.Draw(info);
        _glass.Draw(info);
    }
}