using GardenHose.Game.World.Entities;
using GardenHose.Game.World.Entities.Planet;
using GardenHose.Game.World.Material;
using System;


namespace GardenHose.Game.World;


internal class GameWorldSettings
{
    // Fields.
    internal required WorldPlanetEntity? Planet { get; init; }

    internal required Entity[] StartingEntities { get; init; }

    internal required GameBackground Background { get; init; }

    internal required WorldMaterial AmbientMaterial { get; init; }


    // Constructors.
    internal GameWorldSettings() { }
}