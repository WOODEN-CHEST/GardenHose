using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame;

internal class GenericProgramTime : IProgramTime
{
    public float PassedTimeSeconds { get; set; }

    public float TotalTimeSeconds { get; set; }
}