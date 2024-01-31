using GardenHose.Game.World.Player;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Physical.Collision;


internal struct RectangleCollisionBound : ICollisionBound
{
    // Fields.
    public CollisionBoundType Type => CollisionBoundType.Rectangle;
    public Vector2 Offset { get; set; }
    public float Rotation { get; set; }
    public float BoundingRadius => Math.Max(Size.X, Size.Y);


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

    internal Edge[] GetEdges(Vector2 position, float rotation)
    {
        Vector2[] Vertices = GetVertices(position, rotation);

        return new Edge[]
        {
            new Edge(Vertices[0], Vertices[1]),
            new Edge(Vertices[1], Vertices[2]),
            new Edge(Vertices[2], Vertices[3]),
            new Edge(Vertices[3], Vertices[0]),
        };
    }


    // Inherited methods.
    public void Draw(Vector2 position, float rotation, IDrawInfo info, IWorldCamera camera)
    {
        Display.SharedLine.Mask = Color.Red;
        Display.SharedLine.Thickness = 3f * camera.Zoom;

        Vector2[] Vertices = GetVertices(position, rotation);

        Vertices[0] = Vertices[0] * camera.Zoom + camera.ObjectVisualOffset;
        Vertices[1] = Vertices[1] * camera.Zoom + camera.ObjectVisualOffset;
        Vertices[2] = Vertices[2] * camera.Zoom + camera.ObjectVisualOffset;
        Vertices[3] = Vertices[3] * camera.Zoom + camera.ObjectVisualOffset;

        Display.SharedLine.Set(Vertices[0], Vertices[1]);
        Display.SharedLine.Draw(info);

        Display.SharedLine.Set(Vertices[1], Vertices[2]);
        Display.SharedLine.Draw(info);

        Display.SharedLine.Set(Vertices[2], Vertices[3]);
        Display.SharedLine.Draw(info);

        Display.SharedLine.Set(Vertices[3], Vertices[0]);
        Display.SharedLine.Draw(info);
    }

    public float GetArea()
    {
        return Size.X * Size.Y;
    }

    public float GetRadius()
    {
        return MathF.Sqrt((Size.X * Size.X) + (Size.Y *  Size.Y));
    }

    public Vector2 GetFinalPosition(Vector2 partPosition, float partRotation)
    {
        throw new NotImplementedException();
    }
}