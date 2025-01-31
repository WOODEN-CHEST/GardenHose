﻿using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.Collections;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Particle;

internal class ParticleSettings
{
    // Internal static fields.


    // Internal fields.
    internal WorldMaterial Material { get; init; }
    internal float LifetimeMin { get; init; } = 4f;
    internal float LifetimeMax { get; init; } = 6f;
    internal Vector2 SizeMin { get; init; } = new Vector2(3f);
    internal Vector2 SizeMax { get; init; } = new Vector2(5f);
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
    internal RandomSequence<GHGameAnimationName> AnimationNames { get; init; }

    
    // Constructors.
    internal ParticleSettings(WorldMaterial material, params GHGameAnimationName[] animationNames)
    {
        Material = material ?? throw new ArgumentNullException(nameof(material));
        AnimationNames = new RandomSequence<GHGameAnimationName>(animationNames);
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

    internal Vector2 GetSize()
    {
        return GHMath.LinearInterp(SizeMin, SizeMax, Random.Shared.NextSingle());
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