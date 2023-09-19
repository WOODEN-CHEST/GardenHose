using GardenHoseEngine.Frame.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame;

public interface IDrawer
{
    public void AddDrawableItem(IDrawableItem item);

    public void RemoveDrawableItem(IDrawableItem item);

    public void ClearDrawableItems();

    public void OnShaderChange(IDrawableItem item);
}