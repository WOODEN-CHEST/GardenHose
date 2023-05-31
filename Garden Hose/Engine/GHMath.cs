using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Engine;

public enum InterpolationMethod
{
    Linear = 0,
    EaseIn,
    EaseOut,
    Sine,
    Chaos
}

public static class GHMath
{
    public static Vector2 Interpolate(Vector2 start,
        Vector2 end,
        float amount,
        InterpolationMethod method)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        Vector2 Change = end - start;
        return method switch
        {
            InterpolationMethod.Linear => start + (Change * amount),
            InterpolationMethod.EaseIn => start + (Change * (amount * amount * amount)),
            InterpolationMethod.EaseOut => end - (Change * (1f - amount) * (1f - amount) * (1f - amount)),
            InterpolationMethod.Sine => start + Change * (0.5f + 0.5f * MathF.Sin(amount * MathF.PI - 0.5f * MathF.PI)),
            InterpolationMethod.Chaos => start + (Change * Random.Shared.NextSingle()),
            _ => throw new ArgumentOutOfRangeException($"Invalid interpolation method: {method} {(int)method}")
        };
    }

    public static float Interpolate(float start,
        float end,
        float amount,
        InterpolationMethod method)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        float Change = end - start;
        return method switch
        {
            InterpolationMethod.Linear => start + (Change * amount),
            InterpolationMethod.EaseIn => start + (Change * (amount * amount * amount)),
            InterpolationMethod.EaseOut => end - (Change * (1f - amount) * (1f - amount) * (1f - amount)),
            InterpolationMethod.Sine => start + Change * (0.5f + 0.5f * MathF.Sin(amount * MathF.PI - 0.5f * MathF.PI)),
            InterpolationMethod.Chaos => start + (Change * Random.Shared.NextSingle()),
            _ => throw new ArgumentOutOfRangeException($"Invalid interpolation method: {method} {(int)method}")
        };
    }
}