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
    internal SoundInstanceState State;
    internal bool IsLooped;
    internal float Volume;
    internal float Pan;
    internal double Speed;
    internal int? LowPassCutoffFrequency;
    internal int? HighPassCutoffFrequency;


    // Constructors.
    public SoundInstanceProperties()
    {
        State = SoundInstanceState.Playing;
        IsLooped = false;
        Volume = SoundInstance.VOLUME_MAX;
        Pan = SoundInstance.PAN_MIDDLE;
        Speed = SoundInstance.SPEED_DEFAULT;
        LowPassCutoffFrequency = null;
        HighPassCutoffFrequency = null;
    }
}