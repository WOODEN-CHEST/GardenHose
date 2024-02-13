using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;

namespace GardenHose.Game.World.Entities.Planet.Buildings;

internal abstract class PlanetBuilding : PhysicalEntityPart
{
    // Internal static fields.
    internal const int MIN_LEVEL = 1;
    internal const int MAX_LEVEL = 3;


    // Internal fields.
    internal virtual int Level {  get; set; } = MIN_LEVEL;
    internal virtual string Name { get; set; } = "Default Building Name";
    internal virtual string Description { get; set; } = "Default Building Description";



    // Constructors.
    internal PlanetBuilding(ICollisionBound[]? collisionBounds, WorldMaterial material, PhysicalEntity? entity)
        : base(collisionBounds, material, entity) { }
}