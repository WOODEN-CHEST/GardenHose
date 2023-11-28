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
        set => _temperature = Math.Max(MIN_TEMPERATURE, value);
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

    internal float Charge { get; set; } = (Random.Shared.NextSingle() * 2f - 1f) * 100f;


    // Private float.
    private float _temperature;
    private float _currentStrength;
    private float _stateTransformationValue = 0f; // 0f = solid, 1f = liquid, 2f = gas.
    private const float STATE_SOLID = 0f;
    private const float STATE_LIQUID = 1f;
    private const float STATE_GAS = 2f;


    // Constructors.
    internal WorldMaterialInstance(WorldMaterial material)
    {
        Material = material ?? throw new ArgumentNullException(nameof(material));
        _temperature = DEFAULT_TEMPERATURE;
        CurrentStrength = Material.Strength;
    }


    // Internal methods.
    internal void HeatByTouch(WorldMaterialInstance otherMaterial, float time)
    {
        float AvgTransferRate = (Material.HeatTransferRate + otherMaterial.Material.HeatTransferRate) * 0.5f;
        float TemperatureDifference = otherMaterial.Temperature - Temperature;
        Temperature += TemperatureDifference * AvgTransferRate * time;
    }

    internal void HeatByCollision(float force)
    {
        const float ARBITRARY_REDUCTION_VALUE = 0.01f;
        Temperature += force / Material.HeatCapacity * ARBITRARY_REDUCTION_VALUE * Material.HeatTransferRate;
    }

    internal void Update(float time)
    {
        float TemperatureOffset = GetTemperatureOffsetFromLimit();
        if (TemperatureOffset < 0f)
        {
            ReduceState(TemperatureOffset);
        }
        else
        {
            IncreaseState(TemperatureOffset);
        }
    }


    // Private methods.
    private float GetTemperatureOffsetFromLimit()
    {
        float MaxTemperature = _stateTransformationValue switch
        {
            >= STATE_GAS => float.PositiveInfinity,
            >= STATE_LIQUID => Material.BoilingPoint,
            _ => Material.MeltingPoint
        };

        return Temperature - MaxTemperature;
    }

    private void ReduceState(float temperatureUnder)
    {
        if (_stateTransformationValue == STATE_SOLID)
        {
            return;
        }

        
    }

    private void IncreaseState(float temperatureOver)
    {
        const float ARBITRARY_REDUCTION_VALUE = 0.00212f;
        float TotalPhaseChangeTempAmount = (Material.HeatCapacity * ARBITRARY_REDUCTION_VALUE);

        // Handle melting.
        if (_stateTransformationValue < STATE_LIQUID)
        {
            Temperature = Material.BoilingPoint;
            float TempAmountToMelt = TotalPhaseChangeTempAmount * (1f - _stateTransformationValue);

            if (temperatureOver > TempAmountToMelt)
            {
                _stateTransformationValue = STATE_LIQUID;
                temperatureOver -= TempAmountToMelt;
            }
            else
            {
                _stateTransformationValue += temperatureOver / TempAmountToMelt;
                return;
            }
        }
        if (temperatureOver < 0f) return;

        // Handle heating up to boiling temperature.
        Temperature += temperatureOver;
        temperatureOver = Temperature - Material.BoilingPoint;
        if (temperatureOver <= 0f)
        {
            return;
        }

        // Handle boiling.
        if (_stateTransformationValue >= STATE_GAS)
        {
            return;
        }

        Temperature = Material.BoilingPoint;
        float TempAmountToBoil = TotalPhaseChangeTempAmount * (1f - _stateTransformationValue);

        float _stateChangeAmount = (temperatureOver / TempAmountToBoil);
        _stateTransformationValue = Math.Min(STATE_GAS, _stateTransformationValue + _stateChangeAmount);

        float DamageDone = Material.Strength * _stateChangeAmount;
        CurrentStrength -= DamageDone;
    }
}