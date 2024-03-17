using GardenHoseEngine.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Audio;

internal struct SoundInstanceProperties
{
    // Fields.
    internal SoundInstanceState State { get; set; }
    internal bool IsLooped { get; set; }
    internal float Volume { get; set; }
    internal float Pan { get; set; }
    internal double Speed { get; set; }
    internal int? LowPassCutoffFrequency { get; set; }
    internal int? HighPassCutoffFrequency { get; set; }


    // Constructors.
    public SoundInstanceProperties()
    {
        State = SoundInstanceState.Stopped;
        IsLooped = false;
        Volume = SoundInstance.VOLUME_MAX;
        Pan = SoundInstance.PAN_MIDDLE;
        Speed = SoundInstance.SPEED_DEFAULT;
        LowPassCutoffFrequency = null;
        HighPassCutoffFrequency = null;
    }
}