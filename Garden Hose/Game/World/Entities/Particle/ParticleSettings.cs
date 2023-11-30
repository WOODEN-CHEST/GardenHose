using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Animation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities;

internal class ParticleSettings
{
    // Internal fields.
    internal WorldMaterial Material { get; set; }

    internal float LifetimeMin { get; set; } = 4f;

    internal float LifetimeMax { get; set; } = 6f;

    internal float ScaleMin { get; set; } = 0.9f;

    internal float ScaleMax { get; set; } = 1.1f;

    internal float ScaleChangePerSecondMin { get; set; } = 0f;

    internal float ScaleChangePerSecondMax { get; set; } = 0f;

    internal float CollisionRadius { get; set; } = 2.5f;

    internal float RotationMin { get; set; } = 0f;

    internal float RotationMax { get; set; } = MathHelper.TwoPi;

    internal float AngularMotionMin { get; set; } = 0f;

    internal float AngularMotionMax { get; set; } = 0f;

    internal float FadeOutTime = 1f;

    internal float FadeInTime = 0f;

    internal FloatColor ColorMaskMin { get; set; } = FloatColor.White;

    internal FloatColor ColorMaskMax { get; set; } = FloatColor.White;

    internal Func<SpriteAnimation> AnimationProvider { get; init; }

    
    // Constructors.
    internal ParticleSettings(WorldMaterial material, Func<SpriteAnimation> animationProvider)
    {
        Material = material ?? throw new ArgumentNullException(nameof(material));
        AnimationProvider = animationProvider ?? throw new ArgumentNullException(nameof(animationProvider));
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