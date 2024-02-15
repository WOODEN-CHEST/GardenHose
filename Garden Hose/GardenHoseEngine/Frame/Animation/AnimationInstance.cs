using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;

namespace GardenHoseEngine.Frame.Animation;

public sealed class AnimationInstance : ICloneable
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

    public float FPS
    {
        get => _fps;
        set
        {
            _fps = value;
            _secondsPerFrame = 1f / _fps;
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

    public event EventHandler<AnimFinishEventArgs>? AnimationFinished;


    // Private fields.
    private SpriteAnimation _animation;

    private int _frameIndex = 0;
    private int _frameStep = 1;

    private bool _isAnimating = true;
    private float _fps;
    private float _secondsSinceFrameSwitch;
    private float _secondsPerFrame;


    // Constructors.
    internal AnimationInstance(SpriteAnimation animation)
    {
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
        _isAnimating = (_fps != 0f) && (_frameStep != 0) && (_animation.MaxFrameIndex != 0);
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
            _frameIndex = Math.Clamp(_frameIndex, 0, Animation.MaxFrameIndex);
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
            AnimationWrap(Animation.MaxFrameIndex, FinishLocation.End);
        }
    }


    // Inherited methods.
    public void Update(IProgramTime time)
    {
        if (!_isAnimating) return;

        _secondsSinceFrameSwitch += time.PassedTimeSeconds;
        if (_secondsSinceFrameSwitch > _secondsPerFrame)
        {
            _secondsSinceFrameSwitch -= _secondsPerFrame;
            IncrementFrame();
        }
    }

    public object Clone()
    {
        return new AnimationInstance(_animation)
        {
            IsLooped = IsLooped,
            TextureRegion = TextureRegion,
            FPS = FPS,
            FrameIndex = FrameIndex,
            FrameStep = FrameStep,
            AnimationFinished = AnimationFinished,
            _isAnimating = _isAnimating,
            _secondsSinceFrameSwitch = _secondsSinceFrameSwitch,
            _secondsPerFrame = _secondsPerFrame
        };
    }
}