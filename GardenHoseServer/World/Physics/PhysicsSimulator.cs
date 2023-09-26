using GardenHoseServer.World.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseServer.World.Physics;

internal class PhysicsEngine
{
    // Internal fields.
    internal readonly Vector2 PlanetPosition = Vector2.Zero;


    // Constructors.
    internal PhysicsEngine() { }



    // Internal methods.
        // Planet.
    internal void Simulate(PhysicalEntity entity, float passedTimeSeconds)
    {
        PlanetSimulate(entity, passedTimeSeconds);

        entity.Rotation += entity.AngularMotion;
        entity.Position += entity.Motion;
    }


    // Private methods.
    private void PlanetSimulate(PhysicalEntity entity, float passedTimeSeconds)
    {
        float AttractionStrength = (entity.World.Planet.Radius / Vector2.Distance(entity.Position, PlanetPosition))
            * entity.World.Planet.Attraction;
        AttractionStrength = float.IsFinite(AttractionStrength) ? AttractionStrength : 0f;

        Vector2 Motion = -Vector2.Normalize((entity.Position - PlanetPosition));
        Motion.X *= AttractionStrength * passedTimeSeconds;
        Motion.Y *= AttractionStrength * passedTimeSeconds;

        entity.Motion += Motion;
    }
}