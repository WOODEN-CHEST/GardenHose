using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Ship;

internal class ThrusterPart : PhysicalEntityPart
{
    // Internal fields.
    internal bool IsThrusterOn { get; set; }

    internal float TargetThrusterThrottle { get; set; }

    internal float CurrentThrusterThrottle { get; }

    internal float ThrusterThrottleChangeSpeed { get; }

    internal float ThrusterPower { get; }


    // Constructors.
    internal ThrusterPart(WorldMaterial material, PhysicalEntity entity) : base(material, entity) { }
}