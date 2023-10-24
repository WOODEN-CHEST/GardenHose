using GardenHoseEngine.Frame.Item.Basic;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices.Marshalling;

namespace GardenHose.Game.World.Entities;


internal struct RectangleCollisionBound : ICollisionBound
{
    // Fields.
    public CollisionBoundType Type => CollisionBoundType.Rectangle;

    public Vector2 Offset { get; set; }

    public float Rotation { get; set; }



    // Internal fields.
    internal Vector2 Size { get; set; }

    internal Vector2 HalfSize => Size * 0.5f;


    // Constructors.
    public RectangleCollisionBound() : this(Vector2.Zero) { }

    public RectangleCollisionBound(Vector2 size) : this(size, Vector2.Zero) { }

    public RectangleCollisionBound(Vector2 size, Vector2 offset) : this(size, offset, 0f) { }

    public RectangleCollisionBound(Vector2 size, Vector2 offset, float rotation)
    {
        Size = size;
        Offset = offset;
        Rotation = rotation;
    }


    // Methods.
    internal Vector2[] GetVertices(Vector2 position, float rotation)
    {
        Vector2 TopLeft = -HalfSize + Offset;
        Vector2 BottomRight = HalfSize + Offset;
        Vector2 TopRight = new(BottomRight.X, TopLeft.Y);
        Vector2 BottomLeft = new(TopLeft.X, BottomRight.Y);

        if (rotation != 0f)
        {
            Matrix RotationMatrix = Matrix.CreateRotationZ(Rotation + rotation);
            TopLeft = Vector2.Transform(TopLeft, RotationMatrix) + position;
            BottomRight = Vector2.Transform(BottomRight, RotationMatrix) + position;
            TopRight = Vector2.Transform(TopRight, RotationMatrix) + position;
            BottomLeft = Vector2.Transform(BottomLeft, RotationMatrix) + position;
        }
        

        return new Vector2[] { TopLeft, TopRight, BottomRight, BottomLeft };
    }


    // Inherited methods.
    public void Draw(Vector2 position, float rotation, Line line, GameWorld world)
    {
        line.Mask = Color.Red;
        line.Thickness = 3f * world.Zoom;

        Vector2[] Vertices = GetVertices(position, rotation);

        Vertices[0] = Vertices[0] * world.Zoom + world.ObjectVisualOffset;
        Vertices[1] = Vertices[1] * world.Zoom + world.ObjectVisualOffset;
        Vertices[2] = Vertices[2] * world.Zoom + world.ObjectVisualOffset;
        Vertices[3] = Vertices[3] * world.Zoom + world.ObjectVisualOffset;

        line.Set(Vertices[0], Vertices[1]);
        line.Draw();

        line.Set(Vertices[1], Vertices[2]);
        line.Draw();

        line.Set(Vertices[2], Vertices[3]);
        line.Draw();

        line.Set(Vertices[3], Vertices[0]);
        line.Draw();
    }

    public float GetArea()
    {
        return Size.X * Size.Y;
    }
}