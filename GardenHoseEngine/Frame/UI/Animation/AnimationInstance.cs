using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace GardenHoseEngine.Frame.UI.Animation;

public sealed class AnimationInstance
{
    // Fields.
    public Rectangle? TextureRegion = null;
    public event EventHandler<AnimFinishEventArgs>? AnimationFinished;
    public bool IsLooped = true;

    public SpriteAnimation Animation
    {
        get => _animation!;
        set
        {
            _animation = value ?? throw new ArgumentNullException(nameof(value));
            UpdateIsAnimating();
        }
    }

    public int FrameIndex
    {
        get => _frameIndex;
        set => _frameIndex = Math.Clamp(value, 0, Animation.MaxFrameIndex);
    }

    public float FPS
    {
        get => _fps;
        set
        {
            if (!float.IsFinite(value))
            {
                throw new ArgumentException($"Abnormal animation speed: {value}");
            }

            _fps = value;
            _microSecPerFrame = (ulong)(1_000_000f / Math.Abs(value));
            FrameStep = value >= 0f ? 1 : -1;

            UpdateIsAnimating();
        }
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


    // Private fields.
    private SpriteAnimation _animation;

    private int _frameIndex = 0;
    private int _frameStep = 1;

    private bool _isAnimating = true;
    private float _fps = 60f;
    private ulong _microSecPerFrame = 1_000_000L / 60L;
    private ulong _microSecSinceLastDraw = 0L;


    // Constructors.
    public AnimationInstance(SpriteAnimation animation)
    {
        Animation = animation;
        FPS = Animation.DefaultFPS;
    }


    // Methods.
    public AnimationFrame GetFrame()
    {
        if (_isAnimating) CalculateFrame();
        return _animation.Frames[_frameIndex];
    }

    public void IncrementFrame()
    {
        _frameIndex += FrameStep;

        if (_frameIndex > _animation.MaxFrameIndex)
        {
            AnimationFinished?.Invoke(this, new AnimFinishEventArgs(FinishLocation.End));

            if (IsLooped) _frameIndex = 0;
            else FrameStep = 0;
        }
        else if (_frameIndex < 0)
        {
            AnimationFinished?.Invoke(this, new AnimFinishEventArgs(FinishLocation.Start));

            if (IsLooped) _frameIndex = _animation.MaxFrameIndex;
            else FrameStep = 0;
        }
    }


    // Private methods.
    private void CalculateFrame()
    {
        _microSecSinceLastDraw += (ulong)GameFrame.Time.ElapsedGameTime.TotalMicroseconds;
        if (_microSecSinceLastDraw > _microSecPerFrame)
        {
            _microSecSinceLastDraw = 0;
            IncrementFrame();
        }
    }

    private void UpdateIsAnimating()
    {
        _isAnimating = !(_fps == 0f || _frameStep == 0 || _animation.MaxFrameIndex == 0);
    }

    public void Reset()
    {
        _microSecSinceLastDraw = 0;
        FrameIndex = 0;
        FrameStep = 1;
        TextureRegion = null;
    }
}