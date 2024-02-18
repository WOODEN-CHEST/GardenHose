using GardenHose.Game.World.Entities.Physical;
using System;

namespace GardenHose.Game.World.Entities.Planet.Buildings;

internal class BuildingPlaceholderEntity : PhysicalEntity
{
    // Constructors.
    public BuildingPlaceholderEntity(PlanetBuilding building) : base(EntityType.BuildingPlaceholder)
    {
        MainPart = building;

        CollisionHandler.IsCollisionEnabled = false;
        CollisionHandler.IsCollisionReactionEnabled = false;
        IsTicked = false;
    }

    internal override Entity CreateClone()
    {
        throw new NotSupportedException("Copying of placeholder entities is not supported and should never happen.");
    }


    // Inherited methods.
    internal override void Tick(GHGameTime time)
    {
        throw new InvalidOperationException("A building placeholder entity was ticked. This should be impossible " +
            "because the placeholder entity is temporary and only exists while the game is paused. Crashing game to avoid further issues." +
            $"\nEntity info: {this}");
    }
}