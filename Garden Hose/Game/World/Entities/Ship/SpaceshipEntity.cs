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
    internal SpaceshipController Controller
    {
        get => _controller;
        set
        {
            if (value == _controller) return;

            _controller = value;
            DisableInputListeners();

            if (_controller == SpaceshipController.Player)
            {
                EnableInputListeners();
                return;
            }
            DisableInputListeners();
        }
    }


    // Protected fields.
    protected readonly List<IInputListener> InputListeners = new();


    // Private fields.
    private SpaceshipController _controller = SpaceshipController.None;


    // Constructors.
    internal SpaceshipEntity(EntityType type, GameWorld? world) : base(type, world) { }


    // Internal methods.



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

    protected abstract void AITick();

    protected abstract void PlayerTick();

    protected abstract void TryToNavigateToLocation(Vector2 location);


    // Inherited methods.
    internal override void Tick()
    {
        if (Controller == SpaceshipController.Player)
        {
            PlayerTick();
            TryToNavigateToLocation(World!.ToViewportPosition(UserInput.MouseState.Current.Position.ToVector2()));
        }
        else if (Controller == SpaceshipController.AI)
        {
            AITick();
        }

        base.Tick();
    }
}