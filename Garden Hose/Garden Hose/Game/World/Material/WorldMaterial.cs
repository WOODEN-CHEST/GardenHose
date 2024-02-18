using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Particle;
using GardenHoseEngine.Collections;

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

    internal static WorldMaterial Fuel { get; } = new()
    {
        Density = 0.02f,
        Strength = 200_000f,
        Resistance = 50f,
        HeatCapacity = 800f,
        HeatTransferRate = 12f,
        MeltingPoint = 216f,
        BoilingPoint = 341f,
        Friction = 0.4f,
        Bounciness = 0.15f,
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
    internal RandomSequence<GHGameSoundName>? TapSounds { get; init; }
    internal RandomSequence<GHGameSoundName>? LightHitSounds { get; init; }
    internal RandomSequence<GHGameSoundName>? HitSounds { get; init; }
    internal RandomSequence<GHGameSoundName>? HeavyHitSounds { get; init; }
    internal RandomSequence<GHGameSoundName>? SlightlyDamagedSounds { get; init; }
    internal RandomSequence<GHGameSoundName>? DamagedSounds { get; init; }
    internal RandomSequence<GHGameSoundName>? HeavilyDamagedSounds { get; init; }
    internal RandomSequence<GHGameSoundName>? DestroySounds { get; init; }


    /* Particles. */
    internal ParticleSettings? DamageParticles { get; set; }


    // Constructors.
    protected WorldMaterial() { }


    // Methods.
    internal WorldMaterialInstance CreateInstance() => new WorldMaterialInstance(this);

    internal void Load(GHGameAssetManager assetManager)
    {
        LoadSounds(assetManager, TapSounds?.Items);
        LoadSounds(assetManager, LightHitSounds?.Items);
        LoadSounds(assetManager, HitSounds?.Items);
        LoadSounds(assetManager, HeavyHitSounds?.Items);
        LoadSounds(assetManager, SlightlyDamagedSounds?.Items);
        LoadSounds(assetManager, DamagedSounds?.Items);
        LoadSounds(assetManager, HeavilyDamagedSounds?.Items);
        LoadSounds(assetManager, DestroySounds?.Items);
        LoadSounds(assetManager, TapSounds?.Items);

        if (DamageParticles != null)
        {
            foreach (GHGameAnimationName AnimName in DamageParticles.AnimationNames.Items)
            {
                assetManager.GetAnimation(AnimName);
            }
        }
    }


    // Private methods.
    private void LoadSounds(GHGameAssetManager assetManager, GHGameSoundName[]? sounds)
    {
        if (sounds == null)
        {
            return;
        }

        foreach (GHGameSoundName Sound in sounds)
        {
            assetManager.GetSound(Sound);
        }
    }
}