using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Material;

internal class WorldMaterial
{
    // Static fields.
    internal static WorldMaterial Test { get; } = new(0.1f, 5f, 20f, 20f, 500f, 700f, 0.5f, 0.45f);


    // Fields.
    internal float Density { get; set; }

    internal float Strength { get; set; }

    internal float HeatCapacity { get; set; }

    internal float HeatTransferRate { get; set; }

    internal float MeltingPoint { get; set; }

    internal float BoilingPoint { get; set; }

    internal float Bounciness { get; set; }

    internal float FrictionCoefficient { get; set; }



    // Constructors.
    protected WorldMaterial(float density, 
        float strength, 
        float heatCapacity,
        float heatTransferRate,
        float meltingPoint,
        float boilingPoint,
        float bounciness,
        float frictionCoefficient)
    {
        Density = density;
        Strength = strength;
        HeatCapacity = heatCapacity;
        HeatTransferRate = heatTransferRate;
        MeltingPoint = meltingPoint;
        BoilingPoint = boilingPoint;
        Bounciness = bounciness;
        FrictionCoefficient = frictionCoefficient;
    }


    // Methods.
    internal WorldMaterialInstance CreateInstance() => new WorldMaterialInstance(this);
}