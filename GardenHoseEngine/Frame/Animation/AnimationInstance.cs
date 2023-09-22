using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;

namespace GardenHoseEngine.Frame.Animation;

public sealed class AnimationInstance : GardenHoseEngine.ITimeUpdatable
{
    // Fields.
    public Rectangle? TextureRegion { get; set; } = null;
    
    public bool IsLooped { get; set; } = true;

    
    [MemberNotNull(nameof(_animation))]
    public SpriteAnimation Animation
    {
        get => _animation;
        set
        {
            _animation = value ?? throw new ArgumentNullException(nameof(value));
            UpdateIsAnimating();
        }
    }

    public double FPS
    {
        get => _fps;
        set
        {
            if (!double.IsFinite(value))
            {
                throw new ArgumentException($"Invalid animation speed: {value}", nameof(value));
            }

            _fps = value;
            _secondsPerFrame = 1d / _fps;
            FrameStep = value >= 0f ? 1 : -1;

            UpdateIsAnimating();
        }
    }

    public int FrameIndex
    {
        get => _frameIndex;
        set => _frameIndex = Math.Clamp(value, 0, Animation.MaxFrameIndex);
    }

    public int FrameStep
    {
        get => _frameStep;
        set
        {
            _frameStep = value;
            UpdateIsAnimating();
        }
    } 

    public ITimeUpdater Updater { get; init; }

    public event EventHandler<AnimFinishEventArgs>? AnimationFinished;


    // Private fields.
    private SpriteAnimation _animation;

    private int _frameIndex = 0;
    private int _frameStep = 1;

    private bool _isAnimating = true;
    private double _fps;
    private double _secondsSinceFrameSwitch;
    private double _secondsPerFrame;


    // Constructors.
    internal AnimationInstance(SpriteAnimation animation, ITimeUpdater updater)
    {
        Updater = updater ?? throw new ArgumentNullException(nameof(updater));
        Animation  = animation;
        FPS = Animation.DefaultFPS;
    }


    // Methods.
    public AnimationFrame GetFrame() => _animation.Frames[_frameIndex];

    public void Reset()
    {
        _secondsSinceFrameSwitch = 0;
        FrameIndex = 0;
        FrameStep = 1;
        TextureRegion = null;
    }


    // Private methods.
    private void UpdateIsAnimating()
    {
        _isAnimating = !(_fps == 0f || _frameStep == 0 || _animation.MaxFrameIndex == 0);

        if (_isAnimating)
        {
            Updater.AddUpdateable(this);
        }
        else Updater.RemoveUpdateable(this);
    }


    private void AnimationWrap(int newFrameIndex, FinishLocation finishLocation)
    {
        AnimationFinished?.Invoke(this, new(finishLocation));

        if (IsLooped)
        {
            FrameIndex = newFrameIndex;
        }
        else
        {
            FrameIndex = _frameIndex;
            FrameStep = 0;
            UpdateIsAnimating();
        }
    }

    private void IncrementFrame()
    {
        _frameIndex += FrameStep;

        if (_frameIndex > _animation.MaxFrameIndex)
        {
            AnimationWrap(0, FinishLocation.End);
        }
        else if (_frameIndex < 0)
        {
            AnimationWrap(Animation.MaxFrameIndex, FinishLocation.End); ;
        }
    }


    // Inherited methods.
    public void Update(TimeSpan passedTime)
    {
        _secondsSinceFrameSwitch += passedTime.TotalSeconds;
        if (_secondsSinceFrameSwitch > _secondsPerFrame)
        {
            _secondsSinceFrameSwitch -= _secondsPerFrame;
            IncrementFrame();
        }
    }
}