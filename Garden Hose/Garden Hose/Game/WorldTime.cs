using GardenHoseEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game;

internal class WorldTime : IProgramTime
{
    public float PassedTimeSeconds { get; set; }

    public float TotalTimeSeconds { get; set; }
}