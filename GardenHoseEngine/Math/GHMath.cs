// Ignore Spelling: Interp

using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace GardenHoseEngine;


public static class GHMath
{
    // Static methods.
    /* Vector interpolation. */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 LinearInterp(Vector2 start, Vector2 end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + ((end - start) * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 EaseInInterp(Vector2 start, Vector2 end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + ((end - start) * (amount * amount * amount));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 EaseOutInterp(Vector2 start, Vector2 end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return end - ((end - start) * (1f - amount) * (1f - amount) * (1f - amount));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 SineInterp(Vector2 start, Vector2 end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + (end - start) * (0.5f + 0.5f * MathF.Sin(amount * MathF.PI - 0.5f * MathF.PI));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float LinearInterp(float start, float end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + ((end - start) * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float EaseInInterp(float start, float end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + ((end - start) * (amount * amount * amount));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float EaseOutInterp(float start, float end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return end - ((end - start) * (1f - amount) * (1f - amount) * (1f - amount));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SineInterp(float start, float end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return start + (end - start) * (0.5f + 0.5f * MathF.Sin(amount * MathF.PI - 0.5f * MathF.PI));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 PerpVectorCounterClockwise(Vector2 vector)
    {
        return new Vector2(vector.Y, -vector.X);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 PerpVectorClockwise(Vector2 vector)
    {
        return new Vector2(-vector.Y, vector.X);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AngleBetweenVectors(Vector2 vector1, Vector2 vector2)
    {
        return (Vector2.Dot(vector1, vector2)) / (vector1.Length() * vector2.Length());
    }
}