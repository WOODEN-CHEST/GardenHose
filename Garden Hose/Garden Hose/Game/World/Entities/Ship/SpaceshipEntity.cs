using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Ship.System;
using GardenHoseEngine;
using GardenHoseEngine.IO;

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


    // Constructors.
    internal SpaceshipEntity(EntityType type) : base(type) { }


    // Protected methods.
    protected abstract void AITick();

    protected abstract void PlayerTick();


    // Inherited methods.
    internal override void Tick(GHGameTime time)
    {
        base.Tick(time);

        if (Pilot == SpaceshipPilot.Player)
        {
            PlayerTick();

            if (World!.Planet != null)
            {
                ShipSystem.TargetNavigationPosition = GHMath.NormalizeOrDefault(
                World!.Player.Camera.ToWorldPosition(UserInput.VirtualMousePosition.Current) - World.Planet.Position) * World.Planet.Radius;
            }
            else
            {
                ShipSystem.TargetNavigationPosition = World!.Player.Camera.ToWorldPosition(UserInput.VirtualMousePosition.Current);
            }
            
        }
        else if (Pilot == SpaceshipPilot.AI)
        {
            AITick();
        }

        if (ShipSystem.IsEnabled)
        {
            ShipSystem.Tick();
        }
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        base.Load(assetManager);
        ShipSystem.Load(assetManager);
    }
}