﻿using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeMeter : ProbeSystemComponent
{
    // Internal static fields.
    internal static Vector2 PANEL_SIZE { get; } = new(43.3f, 250f);
    internal static Vector2 VIEW_SIZE { get; } = PANEL_SIZE - new Vector2(10f, 40f);
    internal static Vector2 INDICATOR_SIZE { get; } = new Vector2(15.58f, 11.872f);
    internal static Vector2 MARKING_SIZE { get; } = new Vector2(10f);
    internal static Vector2 ITEM_PADDING { get; } = new Vector2(0f, -6f);


    // Internal fields.
    internal override Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;

            _meter.Position = value;
            _marking.Position = value + new Vector2(0f, PANEL_SIZE.Y * 0.46f);
        }
    }

    internal float MinValue { get; set; }
    internal float MaxValue { get; set; }
    internal float Value { get; set; }


    // Private fields.
    private Func<ProbeEntity, float> _valueSampler;

    private SpriteItem _meter;
    private SpriteItem _indicator;
    private SpriteItem _marking;

    private readonly GHGameAnimationName _markingName;

    private const float INDICATOR_MOVEMENT_SPEED = 5f;


    // Constructors.
    internal ProbeMeter(float min,
        float max,
        GHGameAnimationName markingName,
        Func<ProbeEntity, float> valueSampler)
    {
        MinValue = min;
        MaxValue = max;
        _markingName = markingName;
        _valueSampler = valueSampler ?? throw new ArgumentNullException(nameof(valueSampler));
    }



    // Internal methods.
    internal override void Tick(GHGameTime time, ProbeEntity probe, bool isComponentPowered)
    {
        if (!isComponentPowered)
        {
            return;
        }

        Value = Value + ((_valueSampler.Invoke(probe) - Value) * time.WorldTime.PassedTimeSeconds * INDICATOR_MOVEMENT_SPEED);

        _indicator.Position = new Vector2(Position.X + (VIEW_SIZE.X * 0.5f), Position.Y
        - (Math.Clamp(Value / MaxValue, 0f, 1f) - 0.5f) * VIEW_SIZE.Y) + ITEM_PADDING;
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        _meter = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_Meter).CreateInstance(), PANEL_SIZE);
        _indicator = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_MeterIndicator).CreateInstance(), INDICATOR_SIZE);
        _marking = new(assetManager.GetAnimation(_markingName).CreateInstance(), MARKING_SIZE);
    }

    internal override void Draw(IDrawInfo info)
    {
        _meter.Draw(info);
        _marking.Draw(info);
        _indicator.Draw(info);
    }
}