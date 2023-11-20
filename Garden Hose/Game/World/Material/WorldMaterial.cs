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
        HeatCapacity = 10f,
        HeatTransferRate = 10f,
        MeltingPoint = 400f,
        BoilingPoint = 500f,
        Friction = 0.85f,
        Bounciness = 0.4f,
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
    internal Sound? TapSound { get; set; }

    internal Sound? LightHitSound { get; set; }

    internal Sound? HitSound { get; set; }

    internal Sound? HeavyHitSound { get; set; }

    internal Sound? SlightlyDamagedSound { get; set; }

    internal Sound? DamagedSound { get; set; }

    internal Sound? HeavilyDamagedSound { get; set; }

    internal Sound? DestroyedSound { get; set; }



    // Constructors.
    protected WorldMaterial() { }


    // Methods.
    internal WorldMaterialInstance CreateInstance() => new WorldMaterialInstance(this);
}