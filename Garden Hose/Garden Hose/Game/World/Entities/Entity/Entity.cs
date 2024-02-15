using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical;
using System;

namespace GardenHose.Game.World.Entities;

internal abstract class Entity
{
    // Internal fields.
    internal ulong ID { get; set; }
    internal EntityType EntityType { get; private init; }
    internal GameWorld? World { get; set; }
    internal virtual bool IsPhysical => false;
    internal virtual bool IsTicked { get; set; } = true;

    internal event EventHandler? EntityDelete;


    // Constructors.
    internal Entity(EntityType type)
    {
        EntityType = type;
        ID = World?.GetID() ?? 0uL;
    }


    // Internal methods.
    internal abstract void Tick(GHGameTime gameTime);

    internal abstract void Load(GHGameAssetManager assetManager);

    internal virtual void Delete()
    {
        EntityDelete?.Invoke(this, EventArgs.Empty);
        World?.RemoveEntity(this);
    }

    internal abstract Entity CreateClone();

    internal virtual object CloneDataToObject(Entity newEntity)
    {
        newEntity.ID = ID;
        newEntity.IsTicked = IsTicked;
        newEntity.World = World;
        newEntity.EntityDelete = EntityDelete;

        return newEntity;
    }


    // Inherited methods.
    public override bool Equals(object? obj)
    {
        if ((obj == null) || obj is not Entity)
        {
            return false;
        }

        return ID == ((Entity)obj).ID;
    }

    public override int GetHashCode()
    {
        return int.MinValue + (int)ID;
    }
}