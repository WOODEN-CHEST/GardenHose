using GardenHoseEngine.Frame.Item.Basic;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities;

internal struct BallCollisionBound : ICollisionBound
{
    // Internal fields.
    internal float Radius { get; set; }

    public CollisionBoundType Type => CollisionBoundType.Ball;

    public Vector2 Offset { get; set; }

    public float Rotation { get; set; }


    // Constructors.
    public BallCollisionBound() : this(0f) { }

    public BallCollisionBound(float radius)
    {
        Radius = radius;
    }


    // Inherited methods.
    public void Draw(Vector2 position, float rotation, GameWorld world)
    {
        position += Offset;

        Display.SharedLine.Thickness = 5f * world!.Zoom;
        Display.SharedLine.Mask = Color.Red;

        Display.SharedLine.Set(
            world.ToViewportPosition(position + new Vector2(-Radius, 0f)),
            world.ToViewportPosition(position + new Vector2(Radius, 0f)));
        Display.SharedLine.Draw();

        Display.SharedLine.Set(
            world.ToViewportPosition(position + new Vector2(0f, -Radius)),
            world.ToViewportPosition(position + new Vector2(0f, Radius)));
        Display.SharedLine.Draw();
    }

    public float GetArea()
    {
        return Radius * Radius * MathF.PI;
    }

    public float GetRadius() => Radius;
}
