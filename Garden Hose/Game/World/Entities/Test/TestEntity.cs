using GardenHose;
using GardenHoseEngine.Frame.Item.Basic;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities;


internal class TestEntity : DrawablePhysicalEntity
{
    // Fields.
    internal override Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            CollisionBounds[0].Position = value;
        }
    }

    internal override float Rotation
    {
        get => base.Rotation;
        set
        {
            base.Rotation = value;
            CollisionBounds[0].Rotation = value;
        }
    }


    // Private fields

    private Vector2? _collisionPoint;

    // Constructors
    public TestEntity(GameWorld? world) : base(EntityType.Test, world,
        new CollisionBound[] { new RectangleCollisionBound(new Vector2(20f, 20f)) } )
    {
        DrawCollisionBox = true;
    }

    public TestEntity() : this(null) { }

    // Inherited methods.
    public override void Draw()
    {
        base.Draw();

        VisualLine.Thickness = 5f * World!.Zoom;
        VisualLine.Mask = Color.Green;
        VisualLine.Set(Position * World!.Zoom + World.ObjectVisualOffset,
            (Position + Motion / 2f) * World.Zoom + World.ObjectVisualOffset);
        VisualLine.Draw();

        if (_collisionPoint == null) return;

        VisualLine.Mask = Color.Blue;
        VisualLine.Length = 5f * World!.Zoom;

        VisualLine.Position.Vector = (_collisionPoint.Value * World!.Zoom) + World.ObjectVisualOffset;
        VisualLine.Draw();
    }

    internal override void Load(GHGameAssetManager assetManager) { }

    internal override void Delete() { }


    //protected override void OnCollision(Vector2 collisionPoint, Vector2 surface, Vector2 surfaceNormal)
    //{
    //    base.OnCollision(collisionPoint, surface, surfaceNormal);

    //    _collisionPoint = collisionPoint;
    //}
}