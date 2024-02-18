using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Ship.System;
using GardenHoseEngine;
using GardenHoseEngine.IO;
using System;

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

    internal abstract ISpaceshipSystem ShipSystem { get; set; }
    internal virtual float Oxygen
    {
        get => _oxygen;
        set
        {
            _oxygen = Math.Clamp(value, MIN_OXYGEN, MAX_OXYGEN);
        }
    }

    internal const float MIN_OXYGEN = 0f;
    internal const float MAX_OXYGEN = 1f;


    // Private fields.
    private SpaceshipPilot _pilot = SpaceshipPilot.None;
    private float _oxygen = MAX_OXYGEN;


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

    internal override Entity CloneDataToObject(Entity newEntity)
    {
        base.CloneDataToObject(newEntity);

        SpaceshipEntity Spaceship = (SpaceshipEntity)newEntity;
        Spaceship._pilot = Pilot;

        return newEntity;
    }
}