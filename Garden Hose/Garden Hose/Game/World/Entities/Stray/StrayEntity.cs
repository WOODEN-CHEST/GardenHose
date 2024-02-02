using GardenHose.Game.World.Entities.Physical;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Stray;

internal class StrayEntity : PhysicalEntity
{
    public StrayEntity(GameWorld world,
        PhysicalEntityPart partToStray)
        : base(EntityType.Stray, world)
    {
        if (partToStray == null)
        {
            throw new ArgumentNullException(nameof(partToStray));
        }
        if (partToStray.Entity == null)
        {
            throw new ArgumentException("Part to stray must have an entity.", nameof(partToStray));
        }
        if (partToStray.IsMainPart)
        {
            throw new ArgumentException("Part to stray must not be the main part.", nameof(partToStray));
        }

        partToStray.ParentLink?.UnlinkPart();
        MainPart = partToStray;
        Motion = partToStray.Entity.Motion;
        Position = partToStray.Entity.Position;
        Rotation = partToStray.Entity.Rotation;
    }
}