using GardenHose.Engine.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.CompilerServices;

namespace GardenHose.Engine.Frame.UI.Animation;

public sealed class AnimationInstance
{
    // Fields.
    public Rectangle? TextureRegion = null;

    public Animation Anim
    {
        get => _anim;
        set
        {
            _anim = value ?? throw new ArgumentNullException(nameof(value));
            SetCalculateFrame();
        }
    }

    public int FrameIndex
    {
        get => _frameIndex;
        set => _frameIndex = Math.Max(0, Math.Min(value, Anim.MaxFrameIndex));
    }

    public float FPS
    {
        get => _fps;
        set
        {
            if (!float.IsFinite(value))
                throw new ArgumentException($"Abnormal animation speed: {value}");

            _fps = value;
            _microSecPerFrame = (ulong)(1_000_000f / (float)Math.Abs(value));
            FrameStep = value >= 0f ? 1 : -1;
        }
    }

    public int FrameStep
    {
        get => _frameStep;
        set
        {
            _frameStep = value;
            SetCalculateFrame();
        }
    }


    // Private fields.
    private Animation _anim;

    private int _frameIndex = 0;
    private int _frameStep = 1;

    private bool _mustCalculateFrame = true;
    private float _fps = 60f;
    private ulong _microSecPerFrame = 1_000_000L / 60L;
    private ulong _microSecSinceLastDraw = 0L;


    // Constructors.
    public AnimationInstance(Animation animation)
    {
        Anim = animation;
        FPS = Anim.DefaultFPS;
    }


    // Methods.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AnimationFrame GetFrame()
    {
        if (_mustCalculateFrame) CalculateFrame();
        return _anim.Frames[_frameIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IncrementFrame()
    {
        _frameIndex += FrameStep;
        if (_frameIndex > _anim.MaxFrameIndex) _frameIndex = 0;
        else if (_frameIndex < 0) _frameIndex = _anim.MaxFrameIndex;
    }


    // Private methods.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CalculateFrame()
    {
        _microSecSinceLastDraw += (ulong)GameFrame.Time.ElapsedGameTime.TotalMicroseconds;
        if (_microSecSinceLastDraw > _microSecPerFrame)
        {
            _microSecSinceLastDraw = 0;
            IncrementFrame();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetCalculateFrame()
    {
        _mustCalculateFrame = !(_fps == 0f || _frameStep == 0 || _anim.MaxFrameIndex == 0);
    }

    public void Reset()
    {
        _microSecSinceLastDraw = 0;
        FrameIndex = 0;
        FrameStep = 1;
        TextureRegion = null;
    }
}