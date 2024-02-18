using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Entities.Ship.System;

internal interface ISpaceshipSystem : IDrawableItem
{
    // Fields.
    public bool IsEnabled { get; set; }
    public bool IsPowered { get; set; }
    public SpaceshipEntity Ship { get; }
    public Vector2 TargetNavigationPosition { get; set; }


    // Internal methods.
    [TickedFunction(false)]
    public void Tick();

    public void Load(GHGameAssetManager assetManager);

    public void OnPilotChange(SpaceshipPilot newPilot); 
}