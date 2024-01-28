using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Entities.Ship.System;

internal interface ISpaceshipSystem : IDrawableItem
{
    // Fields.
    public bool IsEnabled { get; set; }

    public SpaceshipEntity Ship { get; }


    // Internal methods.
    [TickedFunction(false)]
    public void NavigateToPosition(Vector2 position);

    [TickedFunction(false)]
    public void ParallelTick(bool isPlayerTick);

    [TickedFunction(false)]
    public void SequentialTick(bool isPlayerTick);

    public void Load(GHGameAssetManager assetManager);

    public void OnPilotChange(SpaceshipPilot newPilot); 
}