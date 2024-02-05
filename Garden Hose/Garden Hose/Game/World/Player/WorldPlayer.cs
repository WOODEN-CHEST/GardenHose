using GardenHose.Game.World.Entities.Ship;
using GardenHose.Game.World.Entities.Ship.System;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Player;

internal class WorldPlayer
{
    // Internal fields.
    internal bool IsAlive { get; private set; } = true;

    [MemberNotNull(nameof(_playerShip))]
    internal SpaceshipEntity SpaceShip
    {
        get => _playerShip;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (_playerShip != null)
            {
                _playerShip.Pilot = SpaceshipPilot.AI;
                _playerShip.World!.Game.UILayer.RemoveDrawableItem(_playerShip.ShipSystem);
            }

            _playerShip = value;
            _playerShip.Pilot = SpaceshipPilot.Player;
            _playerShip.World!.Game.UILayer.AddDrawableItem(_playerShip.ShipSystem);
        }
    }

    internal ISpaceshipSystem ShipSystem => SpaceShip.ShipSystem;

    internal PlayerCamera Camera { get; private init; }

    internal GameWorld World { get; private init; }


    // Private fields.
    private SpaceshipEntity _playerShip;


    // Constructors.
    internal WorldPlayer(GameWorld world, SpaceshipEntity playerShip)
    {
        SpaceShip = playerShip;
        World = world ?? throw new ArgumentNullException(nameof(world));
        Camera = new(this); 
    }


    // Internal methods.
    [TickedFunction(false)]
    internal void Tick(GHGameTime time)
    {
        Camera.Tick(time);
    }


    // Private fields.
    private void OnPlayerShipDeleteEvent(object? sender, EventArgs args)
    {
        IsAlive = false;
    }
}