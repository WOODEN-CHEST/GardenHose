using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeErrorHandler
{
    // Internal fields.
    internal bool IsInSpin { get; private set; } = false;
    internal bool IsFuelLeaked { get; private set; } = false;
    internal bool IsFuelLow { get; private set; } = false;
    internal bool IsOxygenLow { get; private set; } = false;
    internal bool IsMainThrusterDamaged { get; private set; } = false;
    internal bool IsLeftThrusterDamaged { get; private set; } = false;
    internal bool IsRightThrusterDamaged { get; private set; } = false;


    // Constructors.
    internal ProbeErrorHandler() { }


    // Internal methods.
    internal void Tick(ProbeEntity probe)
    {
        IsInSpin = probe.AngularMotion > MathF.PI * 1.5f;
        IsFuelLeaked = false;
        IsOxygenLow = false;

        IsMainThrusterDamaged = (probe.MainThrusterPart == null) || (probe.MainThrusterPart.MaterialInstance.CurrentStrength <=
            probe.MainThrusterPart.MaterialInstance.Material.Strength * 0.65f);

        IsLeftThrusterDamaged = (probe.LeftThrusterPart == null) || (probe.LeftThrusterPart.MaterialInstance.CurrentStrength <=
            probe.LeftThrusterPart.MaterialInstance.Material.Strength * 0.65f);

        IsRightThrusterDamaged = (probe.RightThrusterPart == null) || (probe.RightThrusterPart.MaterialInstance.CurrentStrength <=
            probe.RightThrusterPart.MaterialInstance.Material.Strength * 0.65f);

        float MaxFuel = (probe.MainThrusterPart?.MaxFuel ?? 0f) + (probe.LeftThrusterPart?.MaxFuel ?? 0f)
            + (probe.RightThrusterPart?.MaxFuel ?? 0f);
        if (MaxFuel == 0f)
        {
            IsFuelLow = false;
        }
        else
        {
            float Fuel = (probe.MainThrusterPart?.Fuel ?? 0f) + (probe.LeftThrusterPart?.Fuel ?? 0f)
                + (probe.RightThrusterPart?.Fuel ?? 0f);
            IsFuelLow = Fuel / MaxFuel <= 0.2f;
        }
    }

    internal void Load(GHGameAssetManager assetManager)
    {

    }

    internal void Draw(IDrawInfo info)
    {

    }
}