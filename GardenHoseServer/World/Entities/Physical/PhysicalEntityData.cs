using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseServer.World.Entities;

public class PhysicalEntityData : EntityData
{
    // Fields.
    internal Vector2 Position { get; set; }

    internal Vector2 Motion { get; set; }

    internal float Rotation { get; set; }

    internal float AngularMotion { get; set; }


    // Constructors.
    internal PhysicalEntityData(PhysicalEntity entity) : base(entity)
    {
        Position = entity.Position;
        Motion = entity.Motion; 
        Rotation = entity.Rotation;
        AngularMotion = entity.AngularMotion;
    }
}