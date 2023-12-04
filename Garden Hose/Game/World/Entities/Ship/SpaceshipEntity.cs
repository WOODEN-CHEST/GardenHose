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
    internal SpaceshipPilot Controller
    {
        get => _pilot;
        set
        {
            if (value == _pilot) return;

            _pilot = value;
            DisableInputListeners();

            if (_pilot == SpaceshipPilot.Player)
            {
                EnableInputListeners();
                return;
            }
            DisableInputListeners();
        }
    }

    internal abstract ISpaceshipSystem ShipSystem { get; init; }


    // Protected fields.
    protected readonly List<IInputListener> InputListeners = new();


    // Private fields.
    private SpaceshipPilot _pilot = SpaceshipPilot.None;


    // Constructors.
    internal SpaceshipEntity(EntityType type, GameWorld? world) : base(type, world) { }

    // Protected methods.
    protected virtual void EnableInputListeners()
    {
        foreach (IInputListener Listener in InputListeners)
        {
            UserInput.AddListener(Listener);
        }
    }

    protected virtual void DisableInputListeners()
    {
        foreach (IInputListener Listener in InputListeners)
        {
            Listener.StopListening();
        }
    }

    protected abstract void AIParallelTick();

    protected abstract void PlayerParallelTick();

    protected abstract void AISequentialTick();

    protected abstract void PlayerSequentialTick();


    // Inherited methods.
    internal override void ParallelTick()
    {
        base.ParallelTick();

        if (Controller == SpaceshipPilot.Player)
        {
            PlayerParallelTick();
        }
        else if (Controller == SpaceshipPilot.AI)
        {
            AIParallelTick();
        }
    }

    internal override void SequentialTick()
    {
        base.SequentialTick();

        if (Controller == SpaceshipPilot.Player)
        {
            PlayerSequentialTick();
        }
        else if (Controller == SpaceshipPilot.AI)
        {
            AIParallelTick();
        }
    }
}