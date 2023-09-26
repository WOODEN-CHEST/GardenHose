using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseServer.World.Entities;

internal abstract class Entity
{
    // Fields.
    internal ulong ID { get; private init; }

    internal EntityType EntityType { get; private init; }

    internal GameWorld World { get; private init; }


    // Constructors.
    internal Entity(EntityType type,  GameWorld world)
    {
        EntityType = type;
        World = world ?? throw new ArgumentNullException(nameof(world));
        ID = world.GetID();
    }


    // Methods.
    internal abstract void Tick(float timePassedSeconds);

    internal abstract void Delete();
}