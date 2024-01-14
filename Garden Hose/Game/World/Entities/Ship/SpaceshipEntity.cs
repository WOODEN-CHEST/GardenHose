using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Ship.System;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Ship;

internal abstract class SpaceshipEntity : PhysicalEntity
{
    // Fields.
    internal SpaceshipPilot Pilot
    {
        get => _pilot;
        set
        {
            if (value == _pilot) return;

            _pilot = value;
            ShipSystem.OnPilotChange(_pilot);
        }
    }

    internal abstract ISpaceshipSystem ShipSystem { get; init; }


    // Private fields.
    private SpaceshipPilot _pilot = SpaceshipPilot.None;
    private bool _isInputListened = false;


    // Constructors.
    internal SpaceshipEntity(EntityType type, GameWorld? world) : base(type, world) { }


    // Protected methods.
    protected abstract void AIParallelTick();

    protected abstract void PlayerParallelTick();

    protected abstract void AISequentialTick();

    protected abstract void PlayerSequentialTick();


    // Inherited methods.
    internal override void ParallelTick()
    {
        base.ParallelTick();

        if (Pilot == SpaceshipPilot.Player)
        {
            PlayerParallelTick();
        }
        else if (Pilot == SpaceshipPilot.AI)
        {
            AIParallelTick();
        }
    }

    internal override void SequentialTick()
    {
        base.SequentialTick();

        if (Pilot == SpaceshipPilot.Player)
        {
            PlayerSequentialTick();
        }
        else if (Pilot == SpaceshipPilot.AI)
        {
            AIParallelTick();
        }
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        base.Load(assetManager);
        ShipSystem.Load(assetManager);
    }
}