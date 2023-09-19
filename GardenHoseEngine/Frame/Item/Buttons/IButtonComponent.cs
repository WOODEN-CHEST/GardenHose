using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Buttons;

public interface IButtonComponent
{
    public Vector2 Offset { get; set; }

    public Vector2 Size { get; set; }

    public bool IsLocationOverButton(Vector2 locationToTest, Vector2 origin);
}