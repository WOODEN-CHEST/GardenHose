using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Animation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Particle;

internal class ParticleSettings
{
    // Internal fields.
    internal WorldMaterial Material { get; init; }

    internal float LifetimeMin { get; init; } = 4f;

    internal float LifetimeMax { get; init; } = 6f;

    internal float ScaleMin { get; init; } = 0.9f;

    internal float ScaleMax { get; init; } = 1.1f;

    internal float ScaleChangePerSecondMin { get; init; } = 0f;

    internal float ScaleChangePerSecondMax { get; init; } = 0f;

    internal float CollisionRadius { get; init; } = 2.5f;

    internal float RotationMin { get; init; } = 0f;

    internal float RotationMax { get; init; } = MathHelper.TwoPi;

    internal float AngularMotionMin { get; init; } = 0f;

    internal float AngularMotionMax { get; init; } = 0f;

    internal float FadeOutTime { get; init; } = 1f;

    internal float FadeInTime { get; init;} = 0f;

    internal FloatColor ColorMaskMin { get; init; } = FloatColor.White;

    internal FloatColor ColorMaskMax { get; init; } = FloatColor.White;

    internal string AnimationName { get; init; }

    
    // Constructors.
    internal ParticleSettings(WorldMaterial material, string animationName)
    {
        Material = material ?? throw new ArgumentNullException(nameof(material));
        AnimationName = animationName ?? throw new ArgumentNullException(nameof(animationName));
    }


    // Internal fields.
    internal Color GetColor()
    {
        return FloatColor.InterpolateRGBA(ColorMaskMin, ColorMaskMax, Random.Shared.NextSingle());
    }

    internal float GetLifetime()
    {
        return GHMath.LinearInterp(LifetimeMin, LifetimeMax, Random.Shared.NextSingle());
    }

    internal float GetScale()
    {
        return GHMath.LinearInterp(ScaleMin, ScaleMax, Random.Shared.NextSingle());
    }

    internal float GetRotation()
    {
        return GHMath.LinearInterp(RotationMin, RotationMax, Random.Shared.NextSingle());
    }

    internal float GetAngularMotion()
    {
        return GHMath.LinearInterp(AngularMotionMin, AngularMotionMax, Random.Shared.NextSingle());
    }

    internal float GetScaleChangePerSecond()
    {
        return GHMath.LinearInterp(ScaleChangePerSecondMin, ScaleChangePerSecondMax, Random.Shared.NextSingle());
    }
}