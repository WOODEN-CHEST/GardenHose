using GardenHose.Game.World.Entities;
using System;


namespace GardenHose.Game.World;


internal class WorldState
{
    // Internal fields.
    internal GHGameTime GameTime { get; init; }
    internal Entity[] LivingEntities { get; init; }
    internal Entity[] EntitiesCreated { get; init; }
    internal Entity[] EntitiesRemoved { get; init; }


    // Constructors.
    internal WorldState(GameWorld world)
    {
        GameTime = (GHGameTime)world.Game.GameTime.Clone();

        LivingEntities = new Entity[world.LivingEntities.Length];
        CloneEntitiesIntoCollection(world.LivingEntities, LivingEntities);
        EntitiesCreated = new Entity[world.EntitiesCreated.Length];
        CloneEntitiesIntoCollection(world.EntitiesCreated, EntitiesCreated);
        EntitiesRemoved = new Entity[world.EntitiesRemoved.Length];
        CloneEntitiesIntoCollection(world.EntitiesRemoved, EntitiesRemoved);
    }


    // Private methods.
    private void CloneEntitiesIntoCollection(ReadOnlySpan<Entity> entitySpan, Entity[] targetCollection)
    {
        for (int i = 0; i < entitySpan.Length; i++)
        {
            targetCollection[i] = entitySpan[i].CreateClone();
        }
    }
}