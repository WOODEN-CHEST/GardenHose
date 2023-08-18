using Microsoft.Xna.Framework;
using System;


namespace GardenHoseEngine;


public static class GHMath
{
    // Static methods.
    /* Vector interpolation. */
    public static Vector2 LinearInterp(Vector2 start, Vector2 end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + ((end - start) * amount);
    }

    public static Vector2 EaseInInterp(Vector2 start, Vector2 end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + ((end - start) * (amount * amount * amount));
    }

    public static Vector2 EaseOutInterp(Vector2 start, Vector2 end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return end - ((end - start) * (1f - amount) * (1f - amount) * (1f - amount));
    }

    public static Vector2 SineInterp(Vector2 start, Vector2 end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + (end - start) * (0.5f + 0.5f * MathF.Sin(amount * MathF.PI - 0.5f * MathF.PI));
    }

    public static Vector2 ChaosInterp(Vector2 start, Vector2 end)
    {
        return start + ((end - start) * Random.Shared.NextSingle());
    }

    public static Vector2 Interpolate(InterpolationMethod method, Vector2 start, Vector2 end, float amount)
    {
        return method switch
        {
            InterpolationMethod.Linear => LinearInterp(start, end, amount),
            InterpolationMethod.EaseIn => EaseInInterp(start, end, amount),
            InterpolationMethod.EaseOut => EaseOutInterp(start, end, amount),
            InterpolationMethod.Sine => SineInterp(start, end, amount),
            InterpolationMethod.Chaos => ChaosInterp(start, end),
            _ => throw new EnumValueException(nameof(method), nameof(InterpolationMethod), method, (int)method)
        };
    }

    /* Float interpolation */
    public static float LinearInterp(float start, float end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + ((end - start) * amount);
    }

    public static float EaseInInterp(float start, float end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + ((end - start) * (amount * amount * amount));
    }

    public static float EaseOutInterp(float start, float end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return end - ((end - start) * (1f - amount) * (1f - amount) * (1f - amount));
    }

    public static float SineInterp(float start, float end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + (end - start) * (0.5f + 0.5f * MathF.Sin(amount * MathF.PI - 0.5f * MathF.PI));
    }

    public static float ChaosInterp(float start, float end)
    {
        return start + ((end - start) * Random.Shared.NextSingle());
    }

    public static float Interpolate(InterpolationMethod method, float start, float end, float amount)
    {
        return method switch
        {
            InterpolationMethod.Linear => LinearInterp(start, end, amount),
            InterpolationMethod.EaseIn => EaseInInterp(start, end, amount),
            InterpolationMethod.EaseOut => EaseOutInterp(start, end, amount),
            InterpolationMethod.Sine => SineInterp(start, end, amount),
            InterpolationMethod.Chaos => ChaosInterp(start, end),
            _ => throw new EnumValueException(nameof(method), nameof(InterpolationMethod), method, (int)method)
        };
    }
}