using GardenHoseEngine.Animatable;
using GardenHoseEngine.Frame.Animation;
using Microsoft.Xna.Framework;


namespace GardenHoseEngine;

public class AnimVector2 : ITimeUpdatable
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
            if (_keyframes == null)
            {
                return;
            }

            _time = Math.Clamp(value, MIN_TIME, Duration);

            for (_curIndex = MIN_INDEX; _curIndex < _keyframes!.Length; _curIndex++)
            {
                if (IsIndexSyncedWithTime()) break;
            }
        }
    }

    public AnimVector2Keyframe[]? Keyframes => _keyframes?.ToArray();

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
    public AnimVector2(ITimeUpdater updater, Vector2 vector, KeyFrameBuilder? keyframes)
    {
        Vector = vector;
        if (keyframes != null)
        {
            SetKeyFrames(keyframes);
        }
        Updater = updater ?? throw new ArgumentNullException(nameof(updater));
    }

    public AnimVector2(ITimeUpdater updater, Vector2 vector) : this(updater, vector, null) { }

    public AnimVector2(ITimeUpdater updater) : this(updater, Vector2.Zero, null) { }


    // Methods.
    public void SetKeyFrames(KeyFrameBuilder builder)
    {
        if (IsAnimating)
        {
            throw new InvalidOperationException("Cannot change animation key-frames mid animation.");
        }

        _keyframes = builder.Build();
    }

    public void Start()
    {
        if (_keyframes == null)
        {
            throw new InvalidOperationException("Cannot start the animation because it is not set.");
        }

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
        IsAnimating = false;
        Updater.RemoveUpdateable(this);
    }

    public void Finish()
    {
        if (!IsAnimating)
        {
            return;
        }

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
    private void SyncIndexWithTime()
    {
        int frameStep = Math.Sign(_speed);

        do
        {
            _curIndex += frameStep;

            if ((_curIndex < MIN_INDEX) || (_curIndex > MaxIndex))
            {
                WrapAnimation(frameStep);
            }

        } while (!IsIndexSyncedWithTime());
    }

    private bool IsIndexSyncedWithTime()
    {
        return (_keyframes![_curIndex - 1].Time <= _time) && (_time <= _keyframes[_curIndex].Time);
    }

    private void WrapAnimation(int sign)
    {
        if (sign == 1)
        {
            AnimationFinished?.Invoke(this, new(FinishLocation.End));

            if (IsLooped)
            {
                _curIndex = 0;
                _time = 0d;
            }
            else
            {
                _curIndex = MaxIndex;
                _time = _keyframes[MaxIndex].Time;
            }
        }
        else
        {
            AnimationFinished?.Invoke(this, new(FinishLocation.Start));

            if (IsLooped)
            {
                _curIndex = MaxIndex;
                _time = _keyframes[MaxIndex].Time;
            }
            else
            {
                _curIndex = 0;
                _time = 0d;
            }
        }

        if (!IsLooped)
        {
            Stop();
        }
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