using Microsoft.Xna.Framework;
using System.Collections;

namespace GardenHoseEngine.Frame.Item;

public class KeyframeCollection : IEnumerable<PropertyKeyframe>
{
    // Fields.
    public PropertyKeyframe[] KeyframeArray => _keyframes.ToArray();
    internal float Duration => _keyframes[^1].Time;


    // Private fields.
    private List<PropertyKeyframe> _keyframes = new();


    // Constructors.
    public KeyframeCollection() { }


    // Methods.
    public PropertyKeyframe Get(int index)
    {
        if (_keyframes.Count == 0)
        {
            throw new InvalidOperationException("Cannot get keyframe because no keyframes exist.");
        }

        return _keyframes[Math.Clamp(index, 0, _keyframes.Count - 1)];
    }

    public PropertyKeyframe GetPrevious(float time)
    {
        if (_keyframes.Count == 0)
        {
            throw new InvalidOperationException("Cannot get keyframe because no key-frames exist.");
        }

        for (int i = _keyframes.Count - 1; i >= 0; i--)
        {
            if (_keyframes[i].Time <= time)
            {
                return _keyframes[i];
            }
        }

        return _keyframes[0];
    }

    public PropertyKeyframe GetNext(float time)
    {
        if (_keyframes.Count == 0)
        {
            throw new InvalidOperationException("Cannot get keyframe because no key-frames exist.");
        }

        for (int i = 0; i < _keyframes.Count; i++)
        {
            if (_keyframes[i].Time >= time)
            {
                return _keyframes[i];
            }
        }

        return _keyframes[^1];
    }

    public KeyframeCollection Add(Vector2 position, Vector2 size, float rotation, float timeToReach, InterpolationMethod interpolation)
    {
        if (timeToReach < 0f)
        {
            throw new ArgumentException("Time to reach keyframe must be greater than zero.");
        }
        if (_keyframes.Count == 0)
        {
            timeToReach = 0f;
        }

        _keyframes.Add(new PropertyKeyframe(position, size, rotation, timeToReach + _keyframes[^1].Time, interpolation));
        return this;
    }

    public KeyframeCollection Clear()
    {
        _keyframes.Clear();
        return this;
    }

    public KeyframeCollection Remove(int index)
    {
        _keyframes.RemoveAt(Math.Clamp(index, 0, _keyframes.Count - 1));
        return this;
    }

    public KeyframeCollection Remove(PropertyKeyframe keyframe)
    {
        _keyframes.Remove(keyframe);
        return this;
    }


    // Inherited methods.
    public IEnumerator<PropertyKeyframe> GetEnumerator()
    {
        foreach (PropertyKeyframe keyframe in _keyframes)
        {
            yield return keyframe;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    // Operators.
    public PropertyKeyframe this[int index]
    {
        get => Get(index);
    }
}