using GardenHose.Game.GameAssetManager;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Entities.Planet.Buildings;

internal struct ResidentalBuildingDefinition
{
    internal int Population { get; init; }
    internal Vector2 Size { get; init; }
    internal GHGameAnimationName AnimationName { get; init; }
}