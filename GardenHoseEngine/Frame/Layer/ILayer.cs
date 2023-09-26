using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame;

public interface ILayer : IColorMaskable, IDrawableItem
{
    // Properties.
    string Name { get; }

    public int DrawableItemCount { get; }


    // Methods.
    public void AddDrawableItem(IDrawableItem item);

    public void AddDrawableItem(IDrawableItem item, int index);

    public void RemoveDrawableItem(IDrawableItem item);

    public void ClearDrawableItems();

    
}