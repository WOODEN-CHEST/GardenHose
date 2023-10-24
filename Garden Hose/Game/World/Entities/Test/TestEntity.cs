using GardenHose;
using GardenHose.Game.World.Material;
using GardenHoseEngine.Frame.Item.Basic;
using Microsoft.Xna.Framework;
using NAudio.CoreAudioApi;
using System;

namespace GardenHose.Game.World.Entities;


internal class TestEntity : DrawablePhysicalEntity
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
    public TestEntity(GameWorld? world) : base(EntityType.Test, world, CreateParts())
    {
        DrawCollisionBox = true;
    }

    public TestEntity() : this(null) { }


    // Private static methods.
    private static PhysicalEntityPart CreateParts()
    {
        PhysicalEntityPart MainPart =
            new(new ICollisionBound[] 
            {
                new RectangleCollisionBound(new Vector2(30f, 30f), new Vector2(0f, 15f), MathF.PI / 4f) ,
                new RectangleCollisionBound(new Vector2(20f, 20f), new Vector2(0f, -10f)) 
            },
            WorldMaterial.Test);

        //PhysicalEntityPart SidePart1 = new(new ICollisionBound[] 
        //    { new RectangleCollisionBound(new Vector2(20f, 20f), new Vector2(0f, 0f)) }, WorldMaterial.Test);
        //PhysicalEntityPart SidePart2 = new(new ICollisionBound[] { new RectangleCollisionBound(new Vector2(20f, 20f)) },
        //    WorldMaterial.Test);
        //PhysicalEntityPart SidePart3 = new(new ICollisionBound[] { new RectangleCollisionBound(new Vector2(20f, 20f)) },
        //    WorldMaterial.Test);
        //PhysicalEntityPart SidePart4 = new(new ICollisionBound[] { new RectangleCollisionBound(new Vector2(20f, 20f)) },
        //    WorldMaterial.Test);

        //MainPart.LinkedParts = new PartLink[] 
        //{ 
        //    new PartLink(MainPart, SidePart1, new Vector2(0f, 30f), true),
        //    new PartLink(MainPart, SidePart2, new Vector2(0f, -30f), true),
        //    new PartLink(MainPart, SidePart3, new Vector2(30f, 0f), true),
        //    new PartLink(MainPart, SidePart4, new Vector2(-30f, 0f), true) 
        //};
        return MainPart;
    }


    // Inherited methods.
    public override void Draw()
    {
        base.Draw();

        VisualLine.Thickness = 5f * World!.Zoom;
        VisualLine.Mask = Color.Green;
        VisualLine.Set(Position * World!.Zoom + World.ObjectVisualOffset,
            (Position + Motion / 2f) * World.Zoom + World.ObjectVisualOffset);
        VisualLine.Draw();

        //VisualLine.Mask = Color.Aqua;
        //Vector2 Vertex = ((RectangleCollisionBound)CollisionBounds[0]).GetVertices()[0];

        //VisualLine.Set(Vertex * World.Zoom + World.ObjectVisualOffset,
        //    (Vertex + GetAngularMotionAtPoint(Vertex)) * World.Zoom + World.ObjectVisualOffset);
        //VisualLine.Draw();
    }

    internal override void Load(GHGameAssetManager assetManager) { }

    internal override void Delete() { }


    //protected override void OnCollision(Vector2 collisionPoint, Vector2 surface, Vector2 surfaceNormal)
    //{
    //    base.OnCollision(collisionPoint, surface, surfaceNormal);

    //    _collisionPoint = collisionPoint;
    //}
}