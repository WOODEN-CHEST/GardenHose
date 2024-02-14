using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Entities.Planet.Buildings;

internal class Starport : PlanetBuilding
{
    // Internal static fields.
    internal static readonly Vector2 ReceiverThingySize = new(20f, 80f);
    internal static readonly Vector2 GroundSize = new(160f, 30f);


    // Constructors.
    public Starport()
        : base(new ICollisionBound[]
        {
            new RectangleCollisionBound(ReceiverThingySize, new Vector2(-70f, (-ReceiverThingySize.Y) * 0.5f - (GroundSize.Y * 0.5f))),
            new RectangleCollisionBound(ReceiverThingySize, new Vector2(-30f, (-ReceiverThingySize.Y) * 0.5f - (GroundSize.Y * 0.5f))),
            new RectangleCollisionBound(GroundSize, new Vector2(0f, (-GroundSize.Y) * 0.5f))
        }, WorldMaterial.Test, null)
    {

    }


    // Inherited methods.
    internal override void SetPositionAndRotation(Vector2 position, float rotation)
    {
        base.SetPositionAndRotation(position, rotation);
    }
}