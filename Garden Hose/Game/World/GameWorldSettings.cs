using GardenHose.Game.World.Entities;
using System;


namespace GardenHose.Game.World;


internal class GameWorldSettings
{
    // Fields.
    internal required WorldPlanetEntity? Planet { get; init; }

    internal required Entity[] StartingEntities { get; init; }

    internal required GameBackground Background { get; init; }


    // Constructors.
    internal GameWorldSettings() { }
}