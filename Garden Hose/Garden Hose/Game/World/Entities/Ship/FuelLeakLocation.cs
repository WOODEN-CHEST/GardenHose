using GardenHose.Game.World.Entities.Physical;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Ship;

internal class FuelLeakLocation
{
    // Internal fields.
    internal Vector2 Offset { get; set; }
    internal float TimeSinceLastLeak { get; set; } = 0f;


    // Constructors.
    internal FuelLeakLocation(PhysicalEntityPart part, Vector2 collisionLocation)
    {
        Offset = Vector2.Transform(collisionLocation - part.Position, Matrix.CreateRotationZ(-part.CombinedRotation));
    }


    // Internal methods.
    internal Vector2 GetLocation(ThrusterPart part)
    {
        return part.Position + Vector2.Transform(Offset, Matrix.CreateRotationZ(part.CombinedRotation));
    }
}