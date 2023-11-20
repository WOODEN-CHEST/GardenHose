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
    internal WorldMaterial Material { get; init; }

    internal int CountMin { get; set; }

    internal int CountMax { get; set; }

    internal float Lifetime { get; init; } = 4f;

    internal float RandomLifetimeBonus { get; init; } = 2f;

    internal float Scale { get; init; } = 0.9f;

    internal float RandomScaleBonus { get; init; } = 0.2f;

    internal float Radius { get; init; } = 2.5f;

    internal float Rotation { get; init; } = 0f;

    internal float RandomRotationBonus { get; init; } = MathHelper.TwoPi;

    internal float AngularMotion { get; init; } = 0f;

    internal float RandomAngularMotionBonus { get; init; } = 0f;

    internal FloatColor ColorMask { get; init; } = FloatColor.White;

    internal FloatColor RandomColorMaskBonus { get; init; } = FloatColor.White;

    internal Vector2 Position { get; set; } = Vector2.Zero;

    internal Vector2 Motion { get; set; } = Vector2.UnitX;

    internal float RandomMotionBonus { get; set; } = 0.5f;

    internal float MotionDirectionRandomness { get; set; } = MathF.PI / 6f;

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
        FloatColor ResultColor = ColorMask;
        ResultColor.R += RandomColorMaskBonus.R * Random.Shared.NextSingle();
        ResultColor.G += RandomColorMaskBonus.R * Random.Shared.NextSingle();
        ResultColor.B += RandomColorMaskBonus.R * Random.Shared.NextSingle();
        ResultColor.A += RandomColorMaskBonus.R * Random.Shared.NextSingle();
        return (Color)ResultColor;
    }

    internal float GetLifetime()
    {
        return Lifetime + (Random.Shared.NextSingle() * RandomLifetimeBonus);
    }

    internal float GetScale()
    {
        return Scale + (Random.Shared.NextSingle() * RandomScaleBonus);
    }

    internal float GetRotation()
    {
        return Rotation + (Random.Shared.NextSingle() * RandomRotationBonus);
    }

    internal float GetAngularMotion()
    {
        return AngularMotion + (Random.Shared.NextSingle() * RandomAngularMotionBonus);
    }

    internal Vector2 GetMotion()
    {
        float ResultingLength = Motion.Length() + (RandomMotionBonus * Random.Shared.NextSingle());

        Vector2 MotionUnit = Vector2.Normalize(Motion);
        float RandomRotation = (Random.Shared.NextSingle() * MotionDirectionRandomness) - (MotionDirectionRandomness / 2f);
        MotionUnit = Vector2.Transform(MotionUnit, Matrix.CreateRotationZ(RandomRotation));

        return MotionUnit * ResultingLength;
    }

    internal int GetCount() => Random.Shared.Next(CountMin, CountMax + 1);
}