using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Animatable;

public class KeyFrameBuilder
{
    // Private fields.
    private List<AnimVector2Keyframe> _keyframes = new();


    // Constructors.
    public KeyFrameBuilder(Vector2 startPosition)
        : this(startPosition, InterpolationMethod.Linear) { }

    public KeyFrameBuilder(Vector2 startPosition, InterpolationMethod defaultInterpolation)
    {
        _keyframes.Add(new(startPosition, 0d, defaultInterpolation));
    }


    // Methods.
    public KeyFrameBuilder AddKeyFrame(Vector2 position, double timeToReach, InterpolationMethod interpolation)
    {
        if (timeToReach == 0)
        {
            throw new ArgumentException("Time to reach frame is 0");
        }

        _keyframes.Add(new(position, timeToReach + _keyframes[^1].Time, interpolation));

        AnimVector2Keyframe PreviousKeyFrame = _keyframes[^2];
        PreviousKeyFrame.TimeToNext = timeToReach;
        _keyframes[^2] = PreviousKeyFrame;

        return this;
    }

    public KeyFrameBuilder AddKeyFrame(Vector2 position, double timeToReach)
    {
        return AddKeyFrame(position, timeToReach, _keyframes[^1].InterpMethod);
    }

    public KeyFrameBuilder AddKeyFrame(double timeToReach, InterpolationMethod interpolation)
    {
        return AddKeyFrame(_keyframes[^1].Location, timeToReach, interpolation);
    }

    public KeyFrameBuilder AddKeyFrame(double timeToReach)
    {
        return AddKeyFrame(_keyframes[^1].Location, timeToReach, _keyframes[^1].InterpMethod);
    }

    public AnimVector2Keyframe[] Build()
    {
        if (_keyframes.Count < 2 ) 
        {
            throw new InvalidOperationException("Not enough key-frames.");
        }

        return _keyframes.ToArray();
    }
}