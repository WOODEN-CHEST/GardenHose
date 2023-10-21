using GardenHose.Game.World.Planet;
using GardenHose.Game.World.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World;

internal class GameWorldSettings
{
    // Fields.
    internal WorldPlanet Planet { get; init; }

    internal Entity[] StartingEntities { get; init; }


    // Constructors.
    internal GameWorldSettings()
    {
        Planet = new TestPlanet();
        StartingEntities = Array.Empty<Entity>();
    }
}