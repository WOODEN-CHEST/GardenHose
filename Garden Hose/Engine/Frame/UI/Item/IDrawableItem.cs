using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Engine.Frame.UI.Item;

public interface IDrawableItem
{
    public void Draw();

    public void OnDisplayChange();
}