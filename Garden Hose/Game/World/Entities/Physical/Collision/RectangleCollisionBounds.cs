using GardenHoseEngine.Frame.Item.Basic;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities;


internal class RectangleCollisionBound : CollisionBound
{
    // Internal fields.
    internal Vector2 Size
    {
        get => _size;
        set
        {
            _size = value;
            _halfSize = _size / 2f;
        }
    }

    internal Vector2 HalfSize => _halfSize;


    // Private fields.
    private Vector2 _size;
    private Vector2 _halfSize;


    // Constructors.
    public RectangleCollisionBound() : this(Vector2.Zero) { }

    public RectangleCollisionBound(Vector2 size) : base(CollisionBoundType.Rectangle)
    {
        Size = size;
    }


    // Methods.
    internal Vector2[] GetVertices()
    {
        Vector2 TopLeft = -HalfSize;
        Vector2 BottomRight = HalfSize;
        Vector2 TopRight = new(BottomRight.X, TopLeft.Y);
        Vector2 BottomLeft = new(TopLeft.X, BottomRight.Y);

        Matrix RotationMatrix = Matrix.CreateRotationZ(Rotation);
        TopLeft = Vector2.Transform(TopLeft, RotationMatrix) + Position;
        BottomRight = Vector2.Transform(BottomRight, RotationMatrix) + Position;
        TopRight = Vector2.Transform(TopRight, RotationMatrix) + Position;
        BottomLeft = Vector2.Transform(BottomLeft, RotationMatrix) + Position;

        return new Vector2[] { TopLeft, TopRight, BottomRight, BottomLeft };
    }


    // Inherited methods.
    internal override void Draw(Line line, GameWorld world)
    {
        line.Mask = Color.Red;
        line.Thickness = 3f * world.Zoom;

        Vector2[] Vertices = GetVertices();
        Vector2 ZoomAdjustedSize = Size * world.Zoom;
        Vector2 TopLeft = Vertices[0] * world.Zoom + world.ObjectVisualOffset;
        Vector2 BottomRight = Vertices[2] * world.Zoom + world.ObjectVisualOffset;

        line.Set(TopLeft, ZoomAdjustedSize.X, Rotation);
        line.Draw();

        line.Set(TopLeft, ZoomAdjustedSize.Y, Rotation + (MathF.PI / 2f));
        line.Draw();

        line.Set(BottomRight, ZoomAdjustedSize.X, Rotation + MathF.PI);
        line.Draw();

        line.Set(BottomRight, ZoomAdjustedSize.Y, Rotation - (MathF.PI / 2f));
        line.Draw();
    }
}