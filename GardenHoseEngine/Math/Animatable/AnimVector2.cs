using GardenHoseEngine.Frame.Animation;
using Microsoft.Xna.Framework;


namespace GardenHoseEngine;

public class AnimVector2 : ITimeUpdateable
{
    // Fields.
    public Vector2 Vector;
    public bool IsLooped { get; set; } = false;
    public bool IsAnimating { get; private set; }

    public double Speed
    {
        get => _speed;
        set
        {
            if (!double.IsFinite(value) || value is 0d or -0d)
            {
                throw new ArgumentException($"Invalid animation speed: {value}");
            }

            _speed = Math.Clamp(value, MIN_ANIMATION_SPEED, MAX_ANIMATION_SPEED);
        }
    }

    public double Time
    {
        get => _time;
        set
        {
            ThrowIfAnimNotSet("change time for the");

            _time = Math.Clamp(value, MIN_TIME, Duration);

            for (_curIndex = MIN_INDEX; _curIndex < _keyframes!.Length; _curIndex++)
            {
                if (IsIndexSyncedWithTime()) break;
            }
        }
    }

    public AnimVector2Keyframe[]? Keyframes
    {
        get => _keyframes!.ToArray();
        set
        {
            if (IsAnimating)
            {
                throw new InvalidOperationException("Cannot change animation key-frames mid animation.");
            }

            if (value == null)
            {
                _keyframes = null;
                return;
            }

            if (value.Length < 2)
            {
                throw new ArgumentException($"At least two key-frames are required, got {value.Length}");
            }

            for (int Index = MIN_INDEX; Index < value.Length; Index++)
            {
                if (value[Index].Time <= value[Index - 1].Time)
                {
                    throw new ArgumentException($"Time of keyframe {Index} ({value[Index].Time}) " +
                        $"is lower or equal to the previous frame ({value[Index - 1].Time}).");
                }

                value[Index - 1].TimeToNext = value[Index].Time - value[Index - 1].Time;
            }

            _keyframes = value;
        }
    }

    public ITimeUpdater Updater { get; private set; }

    public event EventHandler<AnimFinishEventArgs>? AnimationFinished;


    // Private fields.
    private const int MIN_INDEX = 1;
    private const double MIN_TIME = 0d;
    private const double MIN_ANIMATION_SPEED = -1e6d;
    private const double MAX_ANIMATION_SPEED = 1e6d;

    private double Duration => _keyframes![^1].Time;
    private int MaxIndex => _keyframes!.Length - 1;

    private double _speed = 1d;
    private AnimVector2Keyframe[]? _keyframes = null;
    private double _time = 0f;
    private int _curIndex = 0;


    // Constructors.
    public AnimVector2(ITimeUpdater updater, Vector2 vector, AnimVector2Keyframe[]? keyframes)
    {
        Vector = vector;
        Keyframes = keyframes;
        Updater = updater ?? throw new ArgumentNullException(nameof(updater));
    }

    public AnimVector2(ITimeUpdater updater, Vector2 vector) : this(updater, vector, null) { }

    public AnimVector2(ITimeUpdater updater) : this(updater, Vector2.Zero, null) { }


    // Methods.
    public void Start()
    {
        ThrowIfAnimNotSet("start");

        if (_speed > 0d)
        {
            SetAnimationData(MIN_INDEX, MIN_TIME, _keyframes![0].Location);
        }
        else
        {
            SetAnimationData(MaxIndex, Duration, _keyframes![^1].Location);
        }

        IsAnimating = true;
        Updater.AddUpdateable(this);
    }

    public void Stop()
    {
        ThrowIfAnimNotSet("stop");
        IsAnimating = false;
        Updater.RemoveUpdateable(this);
    }

    public void Finish()
    {
        Stop();

        if (_speed > 0d)
        {
            SetAnimationData(MaxIndex, Duration, _keyframes![^1].Location);
            AnimationFinished?.Invoke(this, new(FinishLocation.End));
        }
        else
        {
            SetAnimationData(MIN_INDEX, MIN_TIME, _keyframes![0].Location);
            AnimationFinished?.Invoke(this, new(FinishLocation.Start));
        }
    }


    // Private methods.
    private void ThrowIfAnimNotSet(string action)
    {
        if (_keyframes == null)
        {
            throw new InvalidOperationException(
                $"Cannot {action} animation because the animation is not set (null).");
        }
    }

    private void SyncIndexWithTime()
    {
        int frameStep = Math.Sign(_speed);

        do
        {
            _curIndex += frameStep;

            if (_curIndex < MIN_INDEX)
            {
                OnAnimationIndexWrap(FinishLocation.Start, MaxIndex, Duration + _time);
            }
            else if (_curIndex > MaxIndex)
            {
                OnAnimationIndexWrap(FinishLocation.End, MIN_INDEX, _time - Duration);
            }

        } while (!IsIndexSyncedWithTime());
    }

    private bool IsIndexSyncedWithTime() => (_keyframes![_curIndex - 1].Time <= _time) && (_time <= _keyframes[_curIndex].Time);

    private void OnAnimationIndexWrap(FinishLocation finishLoc, int wrapIndex, double wrapTime)
    {
        AnimationFinished?.Invoke(this, new(finishLoc));

        if (IsLooped)
        {
            _curIndex = wrapIndex;
            _time = wrapTime;
        }
        else Stop();
    }

    private void SetAnimationData(int index, double time, Vector2 location)
    {
        _curIndex = index;
        _time = time;
        Vector = location;
    }


    // Inherited methods.
    public void Update(TimeSpan passedTime)
    {
        _time += passedTime.TotalSeconds * Speed;

        if (!IsIndexSyncedWithTime())
        {
            SyncIndexWithTime();
        }

        Vector = GHMath.Interpolate(_keyframes![_curIndex].InterpMethod,
            _keyframes[_curIndex - 1].Location,
            _keyframes[_curIndex].Location,
            (float)((_time - _keyframes[_curIndex - 1].Time) / _keyframes[_curIndex - 1].TimeToNext)
        );
    }


    // Operators.
    public static implicit operator Vector2(AnimVector2 animVector2) => animVector2.Vector;

    public static AnimVector2 operator +(AnimVector2 a, AnimVector2 b)
    {
        a.Vector += b.Vector;
        return a;
    }

    public static AnimVector2 operator -(AnimVector2 a, AnimVector2 b)
    {
        a.Vector -= b.Vector;
        return a;
    }

    public static AnimVector2 operator *(AnimVector2 a, AnimVector2 b)
    {
        a.Vector *= b.Vector;
        return a;
    }

    public static AnimVector2 operator *(AnimVector2 a, float scale)
    {
        a.Vector *= scale;
        return a;
    }

    public static AnimVector2 operator /(AnimVector2 a, AnimVector2 b)
    {
        a.Vector /= b.Vector;
        return a;
    }

    public static AnimVector2 operator /(AnimVector2 a, float denominator)
    {
        a.Vector /= denominator;
        return a;
    }
}