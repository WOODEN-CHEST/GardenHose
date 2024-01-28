using GardenHoseEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game;

internal class GHGameTime
{
    // Internal fields.
    internal const float DEFULT_MIN_PASSED_WORLD_TIME = 1f / 720f;
    internal const float DEFULT_MAX_PASSED_WORLD_TIME = 1f / 20f;

    internal float MinPassedWorldTime { get; private init; } = DEFULT_MIN_PASSED_WORLD_TIME;
    internal float MaxPassedWorldTime { get; private init; } = DEFULT_MAX_PASSED_WORLD_TIME;
    internal IProgramTime ProgramTime { get; private set; }
    internal WorldTime WorldTime { get; } = new WorldTime();
    internal float PassedWorldTimeSeconds { get; set; } = 0f;
    internal float TotalWorldTimeSeconds { get; set; } = 0f;
    internal bool IsRunningSlowly { get; private set; } = false;


    // Private fields.
    private float _timeSinceLastUpdateSeconds = 0f;


    // Constructors.
    internal GHGameTime() { }

    internal GHGameTime(float minTime, float maxTime)
    {
        MinPassedWorldTime = minTime;
        MaxPassedWorldTime = maxTime;
    }


    // Internal methods.
    internal bool Update(IProgramTime programTime)
    {
        ProgramTime = programTime;

        _timeSinceLastUpdateSeconds += ProgramTime.PassedTimeSeconds;

        if (_timeSinceLastUpdateSeconds < MinPassedWorldTime)
        {
            return false;
        }

        if (_timeSinceLastUpdateSeconds > MaxPassedWorldTime)
        {
            IsRunningSlowly = true;
            PassedWorldTimeSeconds = MaxPassedWorldTime;
        }
        else
        {
            IsRunningSlowly = false;
            PassedWorldTimeSeconds = _timeSinceLastUpdateSeconds;
        }
        _timeSinceLastUpdateSeconds = 0f;

        WorldTime.TotalTimeSeconds = TotalWorldTimeSeconds;
        WorldTime.PassedTimeSeconds = PassedWorldTimeSeconds;

        return true;
    }
}