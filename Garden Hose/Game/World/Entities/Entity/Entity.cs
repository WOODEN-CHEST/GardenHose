using GardenHose.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities;

internal abstract class Entity
{
    // Fields.
    internal ulong ID { get; set; }

    internal EntityType EntityType { get; private init; }

    internal GameWorld? World { get; set; }

    internal virtual bool IsPhysical { get; } = false;


    // Constructors.
    internal Entity(EntityType type, GameWorld? world)
    {
        EntityType = type;
        World = world;
        ID = World?.GetID() ?? 0ul;
    }

    internal Entity(EntityType entityType) : this(entityType, null) { }


    // Methods.
    internal abstract void Tick();

    internal abstract void Load(GHGameAssetManager assetManager);

    internal abstract void Delete();


    // Inherited methods.
    public override bool Equals(object? obj)
    {
        if ((obj == null) || obj is not Entity)
        {
            return false;
        }

        return ID == ((Entity)obj).ID;
    }


    // Operators.
    public override int GetHashCode()
    {
        return int.MinValue + (int)ID;
    }
}