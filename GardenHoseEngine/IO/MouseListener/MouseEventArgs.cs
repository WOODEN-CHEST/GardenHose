using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;


namespace GardenHoseEngine.IO;

public class MouseEventArgs : EventArgs
{
    // Constructors.
    public MouseEventArgs() => StartPosition = Vector2.Zero;

    public MouseEventArgs(Vector2 startPosition) => StartPosition = startPosition;


    // Fields.
    public readonly Vector2 StartPosition;
}