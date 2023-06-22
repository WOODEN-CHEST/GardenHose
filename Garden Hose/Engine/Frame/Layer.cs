using GardenHose.Engine.Frame.UI.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace GardenHose.Engine.Frame;

public class Layer
{
    // Private static fields.


    // Fields.
    public Effect Shader = null;
    public readonly GameFrame ParentFrame;


    // Private fields.
    private readonly HashSet<IDrawableItem> _drawableItems = new();


    // Constructors.
    public Layer(GameFrame parentFrame)
    {
        ParentFrame = parentFrame;
        ParentFrame.Layers.Add(this);
    }


    // Methods.

    /* For adding or removing content */
    public void AddDrawable(IDrawableItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _drawableItems.Add(item);
    }

    public void RemoveDrawable(IDrawableItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _drawableItems.Remove(item);
    }

    public void ClearDrawables()
    {
        _drawableItems.Clear();
    }


    /* Drawing */
    public void Draw()
    {
        foreach (var Item in _drawableItems) Item.Draw();
        GameFrame.DrawCalls++;
    }

    public void OnDisplayChange()
    {
        foreach (var Item in _drawableItems) Item.OnDisplayChange();
    }
}