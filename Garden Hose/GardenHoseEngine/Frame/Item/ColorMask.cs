using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item;

public struct ColorMask : IColorMaskable
{
    // Fields.
    public float Brightness
    {
        get => _brightness;
        set
        {
            _brightness = Math.Clamp(value, 0f, 1f);
            UpdateCombinedMask();
        }
    }

    public float Opacity
    {
        get => _opacity;
        set
        {
            _opacity = Math.Clamp(value, 0f, 1f);
            UpdateCombinedMask();
        }
    }

    public Color Mask
    {
        get => _mask;
        set
        {
            _mask = value;
            UpdateCombinedMask();
        }
    }

    public Color CombinedMask => _combinedMask;


    // Constructors.
    public ColorMask() { }


    // private fields.
    private float _brightness = 1f;
    private float _opacity = 1f;
    private Color _combinedMask = Color.White;
    private Color _mask = Color.White;


    // Private methods.
    private void UpdateCombinedMask()
    {
        _combinedMask.R = (byte)(_brightness * _mask.R);
        _combinedMask.G = (byte)(_brightness * _mask.G);
        _combinedMask.B = (byte)(_brightness * _mask.B);
        _combinedMask.A = (byte)(_opacity * byte.MaxValue);
    }
}