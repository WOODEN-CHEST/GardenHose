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
    bool IsAlive { get; set; } = true;

    [MemberNotNull(nameof(_playerShip))]
    internal SpaceshipEntity PlayerShip
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

    internal ISpaceshipSystem ShipSystem => PlayerShip.ShipSystem;


    // Private fields.
    private SpaceshipEntity _playerShip;


    // Constructors.
    internal WorldPlayer(SpaceshipEntity playerShip)
    {
        PlayerShip = playerShip;
    }
}