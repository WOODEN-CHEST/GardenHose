using GardenHose.Game.World.Entities.Physical;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Stray;

internal class StrayEntity : PhysicalEntity
{
    public StrayEntity(GameWorld? world, 
        PhysicalEntityPart strayPart, 
        Vector2 entityPosition, 
        Vector2 entityMotion, 
        float entityRotation)
        : base(EntityType.Stray, world)
    {
        Motion = entityMotion;
        MainPart = strayPart ?? throw new ArgumentNullException(nameof(strayPart));
        SetPositionAndRotation(entityPosition, entityRotation);
    }
}