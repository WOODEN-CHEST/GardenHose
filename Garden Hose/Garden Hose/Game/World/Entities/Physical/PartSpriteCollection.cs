using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Material;
using GardenHose.Game.World.Player;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Physical;

internal class PartSpriteCollection
{
    // Internal fields.
    internal PartSprite UndamagedSprite { get; init; }
    internal PartSprite SlightlyDamagedSprite { get; init; }
    internal PartSprite DamagedSprite { get; init; }
    internal PartSprite HeavilyDamagedSprite { get; init; }


    // Private fields.
    private PartSprite _activeSprite;


    // Constructors.
    internal PartSpriteCollection(PartSprite undamagedSprite, PartSprite slightlyDamagedSprite, PartSprite damagedSprite, PartSprite heavilyDamagedSprite)
    {
        UndamagedSprite = undamagedSprite ?? throw new ArgumentNullException(nameof(undamagedSprite));
        SlightlyDamagedSprite = slightlyDamagedSprite ?? throw new ArgumentNullException(nameof(slightlyDamagedSprite));
        DamagedSprite = damagedSprite ?? throw new ArgumentNullException(nameof(damagedSprite));
        HeavilyDamagedSprite = heavilyDamagedSprite ?? throw new ArgumentNullException(nameof(heavilyDamagedSprite));
        _activeSprite = UndamagedSprite;
    }


    // Internal methods.
    internal void Load(GHGameAssetManager assetManager)
    {
        UndamagedSprite.Load(assetManager);
        SlightlyDamagedSprite.Load(assetManager);
        DamagedSprite.Load(assetManager);
        HeavilyDamagedSprite.Load(assetManager);
    }

    internal void Draw(IDrawInfo info, IWorldCamera camera, PhysicalEntityPart part)
    {
        _activeSprite.Draw(info, camera, part);
    }

    internal void SetActiveSprite(WorldMaterialStage stage)
    {
        _activeSprite = stage switch
        {
            WorldMaterialStage.Undamaged => UndamagedSprite,
            WorldMaterialStage.SlightlyDamaged => SlightlyDamagedSprite,
            WorldMaterialStage.Damaged => DamagedSprite,
            WorldMaterialStage.HeavilyDamaged => HeavilyDamagedSprite,
            _ => throw new EnumValueException(nameof(stage), stage)
        };
    }
}