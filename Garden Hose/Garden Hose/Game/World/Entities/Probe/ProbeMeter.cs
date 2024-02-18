﻿using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeMeter
{
    // Internal static fields.
    internal static Vector2 PANEL_SIZE { get; } = new(43.3f, 250f);
    internal static Vector2 VIEW_SIZE { get; } = PANEL_SIZE - new Vector2(10f, 40f);
    internal static Vector2 DIGIT_SIZE { get; } = new Vector2(13.8f, VIEW_SIZE.Y);
    internal static Vector2 INDICATOR_SIZE { get; } = new Vector2(7.79f, 5.936f);
    internal static Vector2 MARKING_SIZE { get; } = new Vector2(9.3f);
    internal static Vector2 ITEM_PADDING { get; } = new Vector2(0f, -6f);


    // Internal fields.
    internal Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;

            _meter.Position = value;
            _digits.Position = value + ITEM_PADDING;
            _marking.Position = value + new Vector2(0f, PANEL_SIZE.Y * 0.4f);
            UpdateIndicatorPosition();
        }
    }

    internal float MinValue { get; set; }
    internal float MaxValue { get; set; }
    internal float Value { get; set; }


    // Private fields.
    private Vector2 _position;

    private Func<ProbeEntity, float> _valueSampler;

    private SpriteItem _meter;
    private SpriteItem _indicator;
    private SpriteItem _marking;
    private SpriteItem _digits;

    private readonly GHGameAnimationName _markingName;
    private readonly GHGameAnimationName _digitName;


    // Constructors.
    internal ProbeMeter(float min,
        float max,
        GHGameAnimationName markingName,
        GHGameAnimationName digitName,
        Func<ProbeEntity, float> valueSampler)
    {
        MinValue = min;
        MaxValue = max;
        _markingName = markingName;
        _digitName = digitName;
        _valueSampler = valueSampler ?? throw new ArgumentNullException(nameof(valueSampler));
    }



    // Internal methods.
    internal void Tick(ProbeEntity probe)
    {
        Value = _valueSampler.Invoke(probe);
        UpdateIndicatorPosition();
    }

    internal void Load(GHGameAssetManager assetManager)
    {
        _meter = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_Meter).CreateInstance(), PANEL_SIZE);
        _indicator = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_MeterIndicator).CreateInstance(), INDICATOR_SIZE);
        _marking = new(assetManager.GetAnimation(_markingName).CreateInstance(), MARKING_SIZE);
        _digits = new(assetManager.GetAnimation(_digitName).CreateInstance(), DIGIT_SIZE);
    }

    internal void Draw(IDrawInfo info)
    {
        _meter.Draw(info);
        _marking.Draw(info);
        _digits.Draw(info);
        _indicator.Draw(info);
    }

    // Private methods.
    private void UpdateIndicatorPosition()
    {
        _indicator.Position = new Vector2(Position.X + (VIEW_SIZE.X * 0.5f), Position.Y
            - (Math.Clamp(Value / MaxValue, 0f, 1f) - 0.5f) * VIEW_SIZE.Y) + ITEM_PADDING;
    }
}