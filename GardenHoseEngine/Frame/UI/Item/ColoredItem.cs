using Microsoft.Xna.Framework;
using System;

namespace GardenHoseEngine.Frame.UI.Item;

public abstract class ColoredItem : PositionalItem
{
    // Fields.
    public override bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            ShouldDraw = IsDrawingNeeded();
        }
    }

    public float Opacity
    {
        get => _opacity;
        set
        {
            _opacity = Math.Clamp(value, 0f, 1f);
            RealColorMask.A = (byte)(255f * _opacity);
            ShouldDraw = IsDrawingNeeded();
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
    protected bool ShouldDraw = true;


    // Private fields.
    private bool _isVisible = true;
    private float _opacity = 1f;
    private float _brightness = 1f;
    private Color _tint = new(255, 255, 255, 255);


    // Protected methods.
    protected virtual bool IsDrawingNeeded() => _isVisible && (RealColorMask.A != 0);


    // Private methods.
    private void UpdateColorMaskRGB()
    {
        RealColorMask.R = (byte)(_tint.R * _brightness);
        RealColorMask.G = (byte)(_tint.G * _brightness);
        RealColorMask.B = (byte)(_tint.B * _brightness);
    }
}