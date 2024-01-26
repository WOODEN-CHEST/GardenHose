using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame;

public interface IProgramTime
{
    // Fields.
    public float PassedTimeSeconds { get; }
    public float TotalTimeSeconds { get; }
}