using GardenHoseEngine.Frame.Item.Basic;
using Microsoft.Xna.Framework;


namespace GardenHose.Game.World.Entities;


internal abstract class CollisionBound
{
    // Internal fields.
    internal CollisionBoundType Type { get; private init; }

    internal Vector2 Position { get; set; }

    internal float Rotation { get; set; }


    // Constructors.
    internal CollisionBound(CollisionBoundType type)
    {
        Type = type;
    }


    // Methods.
    internal abstract void Draw(Line line, GameWorld world);
}