using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Entities.Test;


internal class TestEntity : PhysicalEntity
{
    // Fields.
    internal override float Rotation
    {
        get => base.Rotation;
        set
        {
            base.Rotation = value;
        }
    }


    // Private fields

    // Constructors
    public TestEntity() : base(EntityType.Test)
    {
        MainPart = new PhysicalEntityPart(new ICollisionBound[] { new RectangleCollisionBound(new Vector2(10f)) }, WorldMaterial.Test, this);
        IsCommonMathCalculated = false;
    }
}