﻿using GardenHose.Game.World.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeMainThrusterPart : PhysicalEntityPart
{
    // Constructors.
    public ProbeMainThrusterPart(ProbeEntity entity) : base(WorldMaterial.Test, entity)
    {
    }


    // Inherited methods.
    protected override void OnBreakPart()
    {
        throw new NotImplementedException();
    }

    protected override void OnPartDamage()
    {
        throw new NotImplementedException();
    }
}