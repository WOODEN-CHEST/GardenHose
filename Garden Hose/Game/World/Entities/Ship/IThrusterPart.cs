using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Ship;

internal interface IThrusterPart
{
    public bool IsThrusterOn { get; set; }

    public float TargetThrusterThrottle { get; set; }

    public float CurrentThrusterThrottle { get; }

    public float ThrusterThrottleChangeSpeed { get; }

    public float ThrusterPower { get; }

    
}