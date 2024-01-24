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
    internal const float DEFULT_MIN_PASSED_TIME = 1f / 720f;
    internal const float DEFULT_MAX_PASSED_TIME = 1f / 20f;

    internal float MinPassedTime { get; private init; } = DEFULT_MIN_PASSED_TIME;
    internal float MaxPassedTime { get; private init; } = DEFULT_MAX_PASSED_TIME;

    internal float ProgramPassedTimeSeconds { get; private set; } = 0f;
    internal float ProgramTotalTimeSeconds { get; private set; } = 0f;
    internal float PassedWorldTimeSeconds { get; set; } = 0f;
    internal float TotalWorldTimeSeconds { get; set; } = 0f;

    internal bool IsRunningSlowly { get; private set; } = falsee;


    // Private fields.
    private float _timeSinceLastUpdateSeconds = 0f;


    // Constructors.
    internal GHGameTime() { }

    internal GHGameTime(float minTime, float maxTime)
    {
        MinPassedTime = minTime;
        MaxPassedTime = maxTime;
    }


    // Internal methods.
    internal bool Update(ProgramTime programTime)
    {
        ProgramPassedTimeSeconds = programTime.PassedTimeSeconds;
        ProgramTotalTimeSeconds = programTime.TotalTimeSeconds;

        TotalWorldTimeSeconds += ProgramPassedTimeSeconds;
        _timeSinceLastUpdateSeconds += ProgramPassedTimeSeconds;

        if (_timeSinceLastUpdateSeconds < MinPassedTime)
        {
            return false;
        }

        if (_timeSinceLastUpdateSeconds > MaxPassedTime)
        {
            IsRunningSlowly = true;
            PassedWorldTimeSeconds = MaxPassedTime;
        }
        else
        {
            IsRunningSlowly = false;
            PassedWorldTimeSeconds = _timeSinceLastUpdateSeconds;
        }
        _timeSinceLastUpdateSeconds = 0f;

        return true;
    }
}