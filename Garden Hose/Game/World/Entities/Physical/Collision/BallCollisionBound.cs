using GardenHoseEngine.Frame.Item.Basic;
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


    }

    public float GetArea()
    {
        return Radius * Radius * MathF.PI;
    }
}
