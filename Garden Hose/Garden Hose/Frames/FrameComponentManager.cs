using GardenHoseEngine.Frame;
using System;

namespace GardenHose.Frames;


internal abstract class FrameComponentManager<T> where T : IGameFrame
{
    // Protected fields.
    protected T ParentFrame { get; init; }


    // Constructors.
    internal FrameComponentManager(T parentFrame)
    {
        ParentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
    }


    // Methods.
    internal virtual void Load() { }

    internal virtual void OnStart() { }

    internal virtual void OnEnd() { }

    internal virtual void Unload() { }

    internal virtual void Update(IProgramTime time) { }
}