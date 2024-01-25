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
    internal ProgramTime RealProgramTime { get; private set; } // The actual program time.
    internal ProgramTime FakeProgramTime { get; private init; } = new(); // Disguised as program time but stores world time.
    internal float PassedWorldTimeSeconds { get; set; } = 0f;
    internal float TotalWorldTimeSeconds { get; set; } = 0f;
    internal bool IsRunningSlowly { get; private set; } = false;


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
        RealProgramTime = programTime;
        FakeProgramTime.TotalTimeSeconds = 

        TotalWorldTimeSeconds += RealProgramTime.PassedTimeSeconds;
        _timeSinceLastUpdateSeconds += RealProgramTime.PassedTimeSeconds;

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

        FakeProgramTime.TotalTimeSeconds = TotalWorldTimeSeconds;
        FakeProgramTime.PassedTimeSeconds = PassedWorldTimeSeconds;

        return true;
    }
}