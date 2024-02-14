using GardenHose.Game.World.Entities;
using GardenHose.Game.World.Entities.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }


    // Private methods.
    private void CloneEntitiesIntoCollection(ReadOnlySpan<Entity> entitySpan, Entity[] targetCollection)
    {
        targetCollection = new Entity[entitySpan.Length];

        for (int i = 0; i < entitySpan.Length; i++)
        {
            targetCollection[i] = (Entity)entitySpan[i].Clone();
        }
    }
}