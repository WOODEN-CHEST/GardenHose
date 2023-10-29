using GardenHose;
using GardenHose.Game.World.Material;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item.Basic;
using Microsoft.Xna.Framework;
using NAudio.CoreAudioApi;
using System;

namespace GardenHose.Game.World.Entities;


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
    public TestEntity(GameWorld? world) : base(EntityType.Test, world)
    {
        AreCollisionBoundsDrawn = true;
        IsCenterOfMassDrawn = false;
        IsMotionDrawn = true;
        IsBoundingBoxDrawn = true;

        MainPart =
            new(new ICollisionBound[]
            {
                new RectangleCollisionBound(new Vector2(20f, 20f), new Vector2(0f, 0f))
            },
            WorldMaterial.Test, this);

        PhysicalEntityPart SidePart1 = new(new ICollisionBound[]
            { new RectangleCollisionBound(new Vector2(20f, 20f), new Vector2(0f, 0f)) }, WorldMaterial.Test, this);

        PhysicalEntityPart SidePart2 = new(new ICollisionBound[] { new RectangleCollisionBound(new Vector2(20f, 20f)) },
            WorldMaterial.Test, this);

        PhysicalEntityPart SidePart3 = new(new ICollisionBound[] { new RectangleCollisionBound(new Vector2(20f, 20f)) },

            WorldMaterial.Test, this);
        PhysicalEntityPart SidePart4 = new(new ICollisionBound[] { new RectangleCollisionBound(new Vector2(20f, 20f)) },

            WorldMaterial.Test, this);



        MainPart.LinkPart(SidePart1, new Vector2(30f, 0f));
        MainPart.LinkPart(SidePart2, new Vector2(0f, -30f));
        MainPart.LinkPart(SidePart3, new Vector2(0f, -60f));
        MainPart.LinkPart(SidePart4, new Vector2(-30f, 0f));
    }

    public TestEntity() : this(null) { }


    // Inherited methods.
    internal override void Load(GHGameAssetManager assetManager) { }

    internal override void Delete() { }
}