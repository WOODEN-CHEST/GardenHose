using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Material;

internal sealed class WorldMaterialInstance : WorldMaterial
{
    // Fields.
    internal const float MIN_TEMPERATURE = float.Epsilon;
    internal const float DEFAULT_TEMPERATURE = 273.15f;

    internal float Temperature
    {
        get => _temperature;
        set => _temperature = Math.Max(MIN_TEMPERATURE, value);
    }

    internal float StrengthLeft { get; set; }



    // Private float.
    private float _temperature;


    // Constructors.
    internal WorldMaterialInstance(WorldMaterial material)
        : base(material.Density, material.Strength, material.HeatCapacity, material.HeatTransferRate, 
            material.MeltingPoint, material.BoilingPoint, material.Bounciness, material.FrictionCoefficient)
    {
        _temperature = DEFAULT_TEMPERATURE;
        StrengthLeft = Strength;
    }
}