using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Entities.Probe;

internal abstract class ProbeSystemComponent
{
    // Internal fields.
    internal virtual Vector2 Position { get; set; }


    // Internal methods.
    internal abstract void Load(GHGameAssetManager assetManager);

    internal abstract void Tick(GHGameTime time, ProbeEntity probe, bool isComponentPowered);

    internal abstract void Draw(IDrawInfo time);
}