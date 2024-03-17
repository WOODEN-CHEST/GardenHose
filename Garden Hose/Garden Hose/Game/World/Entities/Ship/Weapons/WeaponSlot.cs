using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Ship.Weapons;

internal class WeaponSlot
{
    // Internal fields.
    internal WeaponType SupportedType { get; init; }
    internal PhysicalEntityPart TargetPart { get; init; }
    internal Vector2 OffsetFromPart { get; init; }
    internal ShipWeapon? Weapon { get; private set; }



    // Constructors.
    internal WeaponSlot(WeaponType supportedType, PhysicalEntityPart part, Vector2 offsetFromPart)
    {
        SupportedType = supportedType;
        OffsetFromPart = offsetFromPart;
        TargetPart = part;
    }

    
    // Internal methods.
    internal bool SetWeapon(ShipWeapon? weapon)
    {
        if (weapon != null && SupportedType != weapon.Type)
        {
            return false;
        }

        if (Weapon != null)
        {
            TargetPart.UnlinkPart(Weapon);
        }

        Weapon = weapon;

        if (Weapon != null)
        {
            TargetPart.LinkPart(Weapon, OffsetFromPart);
        }

        return true;
    }
}