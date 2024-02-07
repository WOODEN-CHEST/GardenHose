using GardenHose.Game.World.Entities.Physical;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Stray;

internal class StrayEntity : PhysicalEntity
{
    // Constructors.
    protected StrayEntity(PhysicalEntityPart partToStray) : base(EntityType.Stray)
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

        PhysicalEntity OriginalPartEntity = partToStray.Entity;
        Vector2 MotionAtPartPoint = OriginalPartEntity.GetAngularMotionAtPoint(partToStray.Position) + OriginalPartEntity.Motion;
        Vector2 PartPosition = partToStray.Position;

        partToStray.ParentLink?.UnlinkPart();
        MainPart = partToStray;
        Motion = MotionAtPartPoint;
        Position = PartPosition;
        Rotation = OriginalPartEntity.AngularMotion;
    }


    // Static methods.
    internal static StrayEntity MovePartToStrayEntity(PhysicalEntityPart partToStray)
    {
        return new(partToStray);
    }
}