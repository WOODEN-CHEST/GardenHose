﻿using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Particle;
using GardenHoseEngine.Audio;


namespace GardenHose.Game.World.Material;


internal class WorldMaterial
{
    // Static fields.
    internal static WorldMaterial Test { get; } = new()
    {
        Density = 0.1f,
        Strength = 80_000f,
        Resistance = 3000f,
        HeatCapacity = 10_000f,
        HeatTransferRate = 0.8f,
        MeltingPoint = 400f,
        BoilingPoint = 800f,
        Friction = 0.85f,
        Bounciness = 0.4f,
        Magnetism = 0f,
        Conductivity = 0f,
        Attraction = 0f
    };

    internal static WorldMaterial PlanetTest { get; } = new()
    {
        Density = 0.1f,
        Strength = 80_000f,
        Resistance = 3000f,
        HeatCapacity = 10_000f,
        HeatTransferRate = 0.8f,
        MeltingPoint = 400f,
        BoilingPoint = 800f,
        Friction = 0.85f,
        Bounciness = 0.4f,
        Magnetism = 0f,
        Conductivity = 0f,
        Attraction = 50f
    };

    internal static WorldMaterial Void { get; } = new() // Random values go!
    {
        Density = 0.000005f,
        Strength = 10f,
        Resistance = 10f,
        HeatCapacity = 1_000_009f,
        HeatTransferRate = 0.06f,
        MeltingPoint = 0.002f,
        BoilingPoint = 0.002375f,
        Friction = 0.009f,
        Bounciness = 0.35f,
        Magnetism = 0f,
        Conductivity = 0f,
        Attraction = 0f
    };


    // Fields.
    /* Properties. */
    internal required float Density { get; set; }

    internal required float Strength { get; set; }

    internal required float Resistance { get; set; }

    internal required float HeatCapacity { get; set; }

    internal required float HeatTransferRate { get; set; }

    internal required float MeltingPoint { get; set; }

    internal required float BoilingPoint { get; set; }

    internal required float Friction { get; set; }

    internal required float Bounciness { get; set; }

    internal required float Magnetism { get; set; }

    internal required float Conductivity { get; set; }

    internal required float Attraction { get; set; }


    /* Sound. */
    internal Sound? TapSound { get; private set; }

    internal Sound? LightHitSound { get; private set; }

    internal Sound? HitSound { get; private set; }

    internal Sound? HeavyHitSound { get; private set; }

    internal Sound? SlightlyDamagedSound { get; private set; }

    internal Sound? DamagedSound { get; private set; }

    internal Sound? HeavilyDamagedSound { get; private set; }

    internal Sound? DestroyedSound { get; private set; }


    internal string? TapSoundName { get; init; }

    internal string? LightHitSoundName { get; init; }

    internal string? HitSoundName { get; init; }

    internal string? HeavyHitSoundName { get; init; }

    internal string? SlightDamageSoundName { get; init; }

    internal string? DamagedSoundName { get; init; }

    internal string? HeavilyDamagedSoundName { get; init; }

    internal string? DestroyedSoundName { get; init; }


    /* Particles. */
    internal ParticleSettings? DamageParticles { get; set; }


    // Constructors.
    protected WorldMaterial() { }


    // Methods.
    internal WorldMaterialInstance CreateInstance() => new WorldMaterialInstance(this);

    internal void Load(GHGameAssetManager assetManager)
    {
        if (TapSoundName != null)
        {
            TapSound = assetManager.GetSound(TapSoundName);
        }
        if (LightHitSoundName != null)
        {
            LightHitSound = assetManager.GetSound(LightHitSoundName);
        }
        if (HitSoundName != null)
        {
            HitSound = assetManager.GetSound(HitSoundName);
        }
        if (HeavyHitSoundName != null)
        {
            HeavyHitSound = assetManager.GetSound(HeavyHitSoundName);
        }

        if (SlightDamageSoundName != null)
        {
            SlightlyDamagedSound = assetManager.GetSound(SlightDamageSoundName);
        }
        if (DamagedSoundName != null)
        {
            DamagedSound = assetManager.GetSound(DamagedSoundName);
        }
        if (HeavilyDamagedSoundName  != null)
        {
            HeavyHitSound = assetManager.GetSound(HeavilyDamagedSoundName);
        }
        if (DestroyedSoundName != null)
        {
            DestroyedSound = assetManager.GetSound(DestroyedSoundName);
        }

        if (DamageParticles != null)
        {
            assetManager.GetAnimation(DamageParticles.AnimationName);
        }
    }
}