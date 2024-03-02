using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class EngineButtonSwitchEventArgs : EventArgs
{
    internal bool IsOn { get; init; }
    internal ProbeThruster Thruster { get; init; }

    internal EngineButtonSwitchEventArgs(bool isOn, ProbeThruster thruster)
    {
        IsOn = isOn;
        Thruster = thruster;
    }
}