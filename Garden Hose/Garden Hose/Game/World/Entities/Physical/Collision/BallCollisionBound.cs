using GardenHose.Game.World.Player;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Basic;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Physical.Collision;

internal struct BallCollisionBound : ICollisionBound
{
    // Internal fields.
    internal float Radius { get; set; }
    public CollisionBoundType Type => CollisionBoundType.Ball;
    public Vector2 Offset { get; set; }
    public float Rotation { get; set; }
    public float BoundingRadius => Radius;


    // Constructors.
    public BallCollisionBound() : this(0f) { }

    public BallCollisionBound(float radius) : this(radius, Vector2.Zero) { }

    public BallCollisionBound(float radius, Vector2 offset)
    {
        Radius = radius;
        Offset = offset;
    }


    // Inherited methods.
    public void Draw(Vector2 position, float rotation, IDrawInfo info, IWorldCamera camera)
    {
        position += Offset;

        Display.SharedLine.Thickness = 5f * camera.Zoom;
        Display.SharedLine.Mask = Color.Red;

        Display.SharedLine.Set(
            camera.ToViewportPosition(position + new Vector2(-Radius, 0f)),
            camera.ToViewportPosition(position + new Vector2(Radius, 0f)));
        Display.SharedLine.Draw(info);

        Display.SharedLine.Set(
            camera.ToViewportPosition(position + new Vector2(0f, -Radius)),
            camera.ToViewportPosition(position + new Vector2(0f, Radius)));
        Display.SharedLine.Draw(info);
    }

    public float GetArea()
    {
        return Radius * Radius * MathF.PI;
    }

    public Vector2 GetFinalPosition(Vector2 partPosition, float partRotation)
    {
        return Vector2.Transform(Offset, Matrix.CreateRotationZ(partRotation)) + partPosition;
    }
}
