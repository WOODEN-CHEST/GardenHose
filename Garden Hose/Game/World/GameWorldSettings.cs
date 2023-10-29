using GardenHose.Game.World.Entities;
using System;


namespace GardenHose.Game.World;


internal class GameWorldSettings
{
    // Fields.
    internal required WorldPlanet Planet { get; init; }

    internal required Entity[] StartingEntities { get; init; }


    // Constructors.
    internal GameWorldSettings() { }
}