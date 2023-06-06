using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Engine.Frame.UI.Item;

public abstract class ColoredItem : PositionalItem
{
    // Fields.
    public override bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            UpdateShouldRender();
        }
    }

    public float Opacity
    {
        get => _opacity;
        set
        {
            _opacity = Math.Clamp(value, 0f, 1f);
            RealColorMask.A = (byte)(255f * _opacity);
            UpdateShouldRender();
        }
    }

    public float Brightness
    {
        get => _brightness;
        set
        {
            _brightness = Math.Clamp(value, 0f, 1f);
            UpdateColorMaskRGB();
        }
    }

    public Color Tint
    {
        get => _tint;
        set
        {
            _tint = value;
            _tint.A = RealColorMask.A = (byte)(255f * _opacity);
            UpdateColorMaskRGB();
        }
    }


    // Protected fields.
    protected Color RealColorMask = new(255, 255, 255, 255);
    protected bool ShouldRender { get; private set; } = true;


    // Private fields.
    private bool _isVisible = true;
    private float _opacity = 1f;
    private float _brightness = 1f;
    private Color _tint = new(255, 255, 255, 255);


    // Constructors.
    public ColoredItem() { }


    // Private methods.
    private void UpdateShouldRender() => ShouldRender = _isVisible && (RealColorMask.A != 0);

    private void UpdateColorMaskRGB()
    {
        RealColorMask.R = (byte)(_tint.R * _brightness);
        RealColorMask.G = (byte)(_tint.G * _brightness);
        RealColorMask.B = (byte)(_tint.B * _brightness);
    }
}