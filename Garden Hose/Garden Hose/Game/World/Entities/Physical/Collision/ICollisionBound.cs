using GardenHose.Game.World.Player;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;


namespace GardenHose.Game.World.Entities.Physical.Collision;


internal interface ICollisionBound
{
    // Internal fields.
    public CollisionBoundType Type { get; }
    public Vector2 Offset { get; set; }
    public float Rotation { get; set; }
    public float BoundingRadius { get; }


    // Methods.
    public void Draw(Vector2 partPosition, float partRotation, IDrawInfo info, IWorldCamera world);

    public float GetArea();

    public Vector2 GetFinalPosition(Vector2 partPosition, float partRotation);
}