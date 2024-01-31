using GardenHoseEngine.Frame;

namespace GardenHose.Game;

internal class GHGameTime
{
    // Internal fields.
    internal const float DEFULT_MIN_PASSED_WORLD_TIME = 1f / 720f;
    internal const float DEFULT_MAX_PASSED_WORLD_TIME = 1f / 20f;

    internal float Speed { get; set; } = 1f;
    internal float MinPassedWorldTime { get; private init; } = DEFULT_MIN_PASSED_WORLD_TIME;
    internal float MaxPassedWorldTime { get; private init; } = DEFULT_MAX_PASSED_WORLD_TIME;
    internal IProgramTime ProgramTime { get; private set; }
    internal WorldTime WorldTime { get; } = new WorldTime();
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
            WorldTime.PassedTimeSeconds = MaxPassedWorldTime;
        }
        else
        {
            IsRunningSlowly = false;
            WorldTime.PassedTimeSeconds = _timeSinceLastUpdateSeconds;
        }
        _timeSinceLastUpdateSeconds = 0f;

        WorldTime.PassedTimeSeconds *= Speed;
        WorldTime.TotalTimeSeconds += WorldTime.PassedTimeSeconds;

        return true;
    }
}