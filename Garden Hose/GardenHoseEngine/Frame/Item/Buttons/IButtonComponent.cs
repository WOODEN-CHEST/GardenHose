using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GardenHoseEngine.Frame.Item.Buttons;

public interface IButtonComponent
{
    public bool IsLocationOverButton(Vector2 locationToTest, Vector2 origin, Vector2 scale);
}