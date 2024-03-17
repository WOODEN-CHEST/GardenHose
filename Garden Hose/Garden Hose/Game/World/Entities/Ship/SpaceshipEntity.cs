using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Entities.Ship.Weapons;
using GardenHoseEngine;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

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

    internal WeaponSlot[] WeaponSlots
    {
        get => _weaponSlots.ToArray();
        init
        {
            _weaponSlots = value ?? throw new ArgumentNullException(nameof(value));
        }
    }



    // Private fields.
    private SpaceshipPilot _pilot = SpaceshipPilot.None;
    private float _oxygen = MAX_OXYGEN;

    private WeaponSlot[] _weaponSlots;


    // Constructors.
    internal SpaceshipEntity(EntityType type) : base(type) { }


    // Protected methods.
    protected abstract void AITick(GHGameTime time);

    protected virtual void PlayerTick(GHGameTime time)
    {
        if (UserInput.MouseState.Current.LeftButton == ButtonState.Pressed)
        {
            TriggerAllWeapons(time);
        }
    }



    // Internal methods.
    internal void TriggerAllWeapons(GHGameTime time)
    {
        foreach (WeaponSlot Slot in _weaponSlots)
        {
            Slot.Weapon?.Trigger(time);
        }
    }

    internal void FireAllWeapons(GHGameTime time)
    {
        foreach (WeaponSlot Slot in _weaponSlots)
        {
            if (Slot.TargetPart.Entity == this)
            {
                Slot.Weapon?.Fire(time);
            }
        }
    }

    internal void AimWeapons(Vector2 location)
    {
        foreach (WeaponSlot Slot in _weaponSlots)
        {
            if ((Slot.TargetPart.Entity == this) && (Slot.Weapon != null))
            {
                Slot.Weapon.AimLocation = location;
            }
        }
    }

    internal virtual void Repair() { }


    // Inherited methods.
    internal override void Tick(GHGameTime time)
    {
        base.Tick(time);

        if (Pilot == SpaceshipPilot.Player)
        {
            PlayerTick(time);

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
            AITick(time);
        }

        if (ShipSystem.IsEnabled)
        {
            ShipSystem.Tick(time);
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

    internal override void OnCollision(CollisionEventArgs args)
    {
        base.OnCollision(args);

        foreach (WeaponSlot Slot in _weaponSlots)
        {
            if (Slot.Weapon != args.Case.SelfPart)
            {
                continue;
            }

            if (args.Case.SelfPart.Entity != this)
            {
                Slot.SetWeapon(null);
            }
        }
    }
}