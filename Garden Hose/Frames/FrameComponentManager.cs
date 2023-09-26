using GardenHoseEngine;
using GardenHoseEngine.Frame;
using System;

namespace GardenHose.Frames;


internal abstract class FrameComponentManager<FrameType> where FrameType : IGameFrame
{
    // Protected fields.
    protected readonly FrameType ParentFrame;


    // Constructors.
    internal FrameComponentManager(FrameType parentFrame)
    {
        ParentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
    }


    // Methods.
    internal virtual void Load(AssetManager assetManager) { }

    internal virtual void OnStart() { }

    internal virtual void OnEnd() { }

    internal virtual void Unload(AssetManager assetManager) { }

    internal virtual void Update(float passedTimeSeconds) { }

}