﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Audio;

internal class SoundInstanceSampler : ISampleProvider
{
    public WaveFormat WaveFormat => throw new NotImplementedException();

    public int Read(float[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}