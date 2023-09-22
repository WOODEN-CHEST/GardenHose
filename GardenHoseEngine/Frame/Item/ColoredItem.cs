using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;

namespace GardenHoseEngine.Frame.Item;

public abstract class ColoredItem : PositionalItem, IColorMaskable
{
    // Fields.
    public override bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            UpdateShouldDraw();
        }
    }

    public virtual float Opacity
    {
        get => _colorMask.Opacity;
        set
        {
            _colorMask.Opacity = value;
            UpdateShouldDraw();
        }
    }

    public virtual float Brightness
    {
        get => _colorMask.Brightness;
        set => _colorMask.Brightness = value;
    }

    public virtual Color Mask
    {
        get => _colorMask.Mask;
        set => _colorMask.Mask = value;
    }

    public Color CombinedMask => _colorMask.CombinedMask;



    // Protected fields.
    protected bool ShouldDraw = true;


    // Private fields.
    private bool _isVisible = true;
    private ColorMask _colorMask = new();


    // Constructors.
    public ColoredItem(ITimeUpdater updater, IVirtualConverter converter, IDrawer? drawer) 
        : base(updater, converter, drawer) { }


    // Protected methods.
    protected virtual void UpdateShouldDraw()
    {
        ShouldDraw = _isVisible && Opacity != 0f;
    }
}