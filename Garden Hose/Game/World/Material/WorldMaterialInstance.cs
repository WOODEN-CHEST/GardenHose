using System;

namespace GardenHose.Game.World.Material;

internal sealed class WorldMaterialInstance
{
    // Fields.
    internal const float MIN_TEMPERATURE = float.Epsilon;

    internal const float DEFAULT_TEMPERATURE = 273.15f;

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

    // Private float.
    private float _temperature;
    private float _currentStrength;


    // Constructors.
    internal WorldMaterialInstance(WorldMaterial material)
    {
        Material = material ?? throw new ArgumentNullException(nameof(material));
        _temperature = DEFAULT_TEMPERATURE;
        CurrentStrength = Material.Strength;
    }
}