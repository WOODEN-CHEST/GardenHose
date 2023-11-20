using GardenHoseEngine.Frame.Item.Basic;
using Microsoft.Xna.Framework;


namespace GardenHose.Game.World.Entities;


internal interface ICollisionBound
{
    // Internal fields.
    public CollisionBoundType Type { get; }

    public Vector2 Offset { get; set; }

    public float Rotation { get; set; }


    // Methods.
    public void Draw(Vector2 position, float rotation, GameWorld world);

    public float GetArea();
}