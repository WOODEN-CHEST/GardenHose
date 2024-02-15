using GardenHoseEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Physical;

internal class CommonEntityMath
{
    // Internal static fields.
    internal static readonly Vector2 DEFAULT_NONZERO_VECTOR = -Vector2.UnitY;

    // Internal fields.
    internal PhysicalEntity Entity { get; private init; }

    internal Vector2 DirPlanetToEntity { get; private set; } = DEFAULT_NONZERO_VECTOR;
    internal Vector2 DirPlanetToEntityNormal { get; private set; } = DEFAULT_NONZERO_VECTOR;
    internal Vector2 DirPlanetToEntityNormalNeg { get; private set; } = DEFAULT_NONZERO_VECTOR;
    internal float DistanceToPlanet { get; private set; } = 0f;
    internal float PlanetRelativeXSpeed { get; private set; } = 0f;
    internal float PlanetRelativeYSpeed { get; private set; } = 0f;
    internal float Altitude { get; private set; } = 0f;
    internal float AltitudeOneSecInFuture { get; private set; } = 0f;
    internal float Roll { get; private set; } = 0f;
    internal float RollOneSecInFuture { get; private set; } = 0f;




    // Constructors.
    internal CommonEntityMath(PhysicalEntity entity)
    {
        Entity = entity ?? throw new ArgumentNullException(nameof(entity));
    }


    // Internal methods.
    internal void Calculate()
    {
        if (Entity.World!.Planet != null)
        {
            DirPlanetToEntity = Entity.Position - Entity.World.Planet.Position;
            if (DirPlanetToEntity.X + DirPlanetToEntity.Y is 0f or -0f)
            {
                DirPlanetToEntity = DEFAULT_NONZERO_VECTOR;
            }
            DirPlanetToEntity.Normalize();

            DirPlanetToEntityNormal = GHMath.PerpVectorClockwise(DirPlanetToEntity);
            DirPlanetToEntityNormalNeg = -DirPlanetToEntityNormal;

            DistanceToPlanet = Vector2.Distance(Entity.Position, Entity.World.Planet.Position);

            PlanetRelativeXSpeed = Vector2.Dot(Entity.Motion, DirPlanetToEntityNormal);
            PlanetRelativeYSpeed = Vector2.Dot(Entity.Motion, DirPlanetToEntity);

            Altitude = DistanceToPlanet - Entity.World.Planet.Radius;
            AltitudeOneSecInFuture = Altitude + PlanetRelativeYSpeed;

            float UpAngleAtPosition = MathF.Atan2(Entity.Position.X, -Entity.Position.Y);
            Vector2 Direction = Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(Entity.Rotation - UpAngleAtPosition));
            Roll = MathF.Atan2(Direction.X, -Direction.Y);
            RollOneSecInFuture = Roll + Entity.AngularMotion; // May cause weird values (roll is [-pi;pi],
                                                              // but high angular motion may bring out of this range).
        }
    }

    internal CommonEntityMath CreateClone(PhysicalEntity entity)
    {
        return CopyDataToObject(new CommonEntityMath(entity));
    }

    internal CommonEntityMath CopyDataToObject(CommonEntityMath math)
    {
        math.DirPlanetToEntity = DirPlanetToEntity;
        math.DirPlanetToEntityNormal = DirPlanetToEntityNormal;
        math.DirPlanetToEntityNormalNeg = DirPlanetToEntityNormalNeg;
        math.DistanceToPlanet = DistanceToPlanet;
        math.PlanetRelativeXSpeed = PlanetRelativeXSpeed;
        math.PlanetRelativeYSpeed = PlanetRelativeYSpeed;
        math.Altitude = Altitude;
        math.AltitudeOneSecInFuture = AltitudeOneSecInFuture;
        math.Roll = Roll;
        math.RollOneSecInFuture = RollOneSecInFuture;

        return math;
    }
}