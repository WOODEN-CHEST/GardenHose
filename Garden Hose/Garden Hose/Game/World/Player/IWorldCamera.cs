using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Player;

internal interface IWorldCamera
{
    // Fields.
    public Vector2 CameraCenter { get; set; }
    public Vector2 ObjectVisualOffset { get; }
    public float Zoom { get; set; }
    public float InverseZoom { get; }


    // Methods.
    public Vector2 ToViewportPosition(Vector2 worldPosition);

    public Vector2 ToWorldPosition(Vector2 viewportPosition);
}