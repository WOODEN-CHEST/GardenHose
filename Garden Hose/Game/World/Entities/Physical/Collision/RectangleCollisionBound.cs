using GardenHoseEngine.Frame.Item.Basic;
using GardenHoseEngine.Screen;
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
        Vector2 TopLeft = -HalfSize;
        Vector2 BottomRight = HalfSize;
        Vector2 TopRight = new(BottomRight.X, TopLeft.Y);
        Vector2 BottomLeft = new(TopLeft.X, BottomRight.Y);

        if (Rotation != 0f)
        {
            Matrix RotationMatrix = Matrix.CreateRotationZ(Rotation);
            TopLeft = Vector2.Transform(TopLeft, RotationMatrix);
            BottomRight = Vector2.Transform(BottomRight, RotationMatrix);
            TopRight = Vector2.Transform(TopRight, RotationMatrix);
            BottomLeft = Vector2.Transform(BottomLeft, RotationMatrix);
        }

        TopLeft += Offset;
        BottomRight += Offset;
        TopRight += Offset;
        BottomLeft += Offset;

        if (rotation != 0f)
        {
            Matrix RotationMatrix = Matrix.CreateRotationZ(rotation);
            TopLeft = Vector2.Transform(TopLeft, RotationMatrix);
            BottomRight = Vector2.Transform(BottomRight, RotationMatrix);
            TopRight = Vector2.Transform(TopRight, RotationMatrix);
            BottomLeft = Vector2.Transform(BottomLeft, RotationMatrix);
        }

        TopLeft += position;
        BottomRight += position;
        TopRight += position;
        BottomLeft += position;

        return new Vector2[] { TopLeft, TopRight, BottomRight, BottomLeft };
    }


    // Inherited methods.
    public void Draw(Vector2 position, float rotation, GameWorld world)
    {
        Display.SharedLine.Mask = Color.Red;
        Display.SharedLine.Thickness = 3f * world.Zoom;

        Vector2[] Vertices = GetVertices(position, rotation);

        Vertices[0] = Vertices[0] * world.Zoom + world.ObjectVisualOffset;
        Vertices[1] = Vertices[1] * world.Zoom + world.ObjectVisualOffset;
        Vertices[2] = Vertices[2] * world.Zoom + world.ObjectVisualOffset;
        Vertices[3] = Vertices[3] * world.Zoom + world.ObjectVisualOffset;

        Display.SharedLine.Set(Vertices[0], Vertices[1]);
        Display.SharedLine.Draw();

        Display.SharedLine.Set(Vertices[1], Vertices[2]);
        Display.SharedLine.Draw();

        Display.SharedLine.Set(Vertices[2], Vertices[3]);
        Display.SharedLine.Draw();

        Display.SharedLine.Set(Vertices[3], Vertices[0]);
        Display.SharedLine.Draw();
    }

    public float GetArea()
    {
        return Size.X * Size.Y;
    }
}