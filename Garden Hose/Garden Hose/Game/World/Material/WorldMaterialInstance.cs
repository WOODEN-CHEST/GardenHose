using GardenHoseEngine.Frame;
using System;

namespace GardenHose.Game.World.Material;

internal sealed class WorldMaterialInstance
{
    // Internal static fields.
    internal const float MIN_TEMPERATURE = float.Epsilon;

    internal const float DEFAULT_TEMPERATURE = 273.15f;


    // Internal fields.
    internal WorldMaterial Material { get; init; }

    internal float Temperature
    {
        get => _temperature;
        set
        {
            _temperature = Math.Max(MIN_TEMPERATURE, value);

            if (_temperature >= Material.BoilingPoint)
            {
                State = WorldMaterialState.Gas;
            }
            else if (_temperature >= Material.MeltingPoint)
            {
                State = WorldMaterialState.Liquid;
            }
            else
            {
                State = WorldMaterialState.Solid;
            }
        }
    }

    internal float CurrentStrength
    {
        get => _currentStrength;
        set
        {
            _currentStrength = Math.Clamp(value, 0f, Material.Strength);

            const float STAGE_COUNT = 4f;
            Stage = (WorldMaterialStage)(STAGE_COUNT - MathF.Ceiling(_currentStrength / (Material.Strength / STAGE_COUNT)));
        }
    }

    internal WorldMaterialStage Stage { get; private set; } = WorldMaterialStage.Undamaged;

    internal WorldMaterialState State { get; private set; }


    // Private float.
    private float _temperature;
    private float _currentStrength;


    // Constructors.
    internal WorldMaterialInstance(WorldMaterial material)
    {
        Material = material ?? throw new ArgumentNullException(nameof(material));
        Temperature = DEFAULT_TEMPERATURE;
        CurrentStrength = Material.Strength;
    }

    internal WorldMaterialInstance(WorldMaterialInstance materialInstance)
    {
        Material = materialInstance.Material;
        Temperature = materialInstance.Temperature;
        CurrentStrength = materialInstance.CurrentStrength;
        State = materialInstance.State;
        Stage = materialInstance.Stage;
    }


    // Internal methods.
    internal void HeatByTouch(WorldMaterialInstance otherMaterial, GHGameTime time)
    {
        const float ARBITRARY_REDUCTION_VALUE = 0.0078f;
        float AvgTransferRate = (Material.HeatTransferRate + otherMaterial.Material.HeatTransferRate) * 0.5f;
        float TemperatureDifference = otherMaterial.Temperature - Temperature;
        Temperature += TemperatureDifference * AvgTransferRate * time.WorldTime.PassedTimeSeconds * ARBITRARY_REDUCTION_VALUE;
    }

    internal void HeatByCollision(float force)
    {
        const float ARBITRARY_REDUCTION_VALUE = 0.294f;
        Temperature += force / Material.HeatCapacity * ARBITRARY_REDUCTION_VALUE * Material.HeatTransferRate;
    }

    internal void Update(GHGameTime time, bool isDamageEnabled)
    {
        if (!isDamageEnabled) return;

        const float MELTING_DAMAGE = 5f;
        const float BOILING_DAMAGE = 3000;
        const float ARBITRARY_REDUCTION_VALUE = 0.00212f;

        if (Temperature >= Material.BoilingPoint)
        {
            CurrentStrength -= BOILING_DAMAGE * time.WorldTime.PassedTimeSeconds * ((Temperature - Material.BoilingPoint) * ARBITRARY_REDUCTION_VALUE);
        }
        else if (Temperature >= Material.MeltingPoint)
        {
            CurrentStrength -= MELTING_DAMAGE * time.WorldTime.PassedTimeSeconds * ((Temperature - Material.MeltingPoint) * ARBITRARY_REDUCTION_VALUE);
        }
    }
}