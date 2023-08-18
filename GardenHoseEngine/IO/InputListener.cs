using GardenHoseEngine.Frame;
using System;

namespace GardenHoseEngine.IO;

public abstract class InputListener
{
    // Internal fields.
    internal readonly object Creator;
    internal readonly GameFrame? ParentFrame;
    internal readonly bool RequiresFocus;
    internal abstract Func<bool> ConditionFunc { get; init; }


    // Constructors.
    internal InputListener(object? creator,
        GameFrame? parentFrame,
        bool requiresFocus)
    {
        ParentFrame = parentFrame;
        RequiresFocus = requiresFocus;
        UserInput.AddListener(this);

        if (creator != null) Creator = creator;
        else if (parentFrame != null) Creator = parentFrame;
        else Creator = this;
    }


    // Methods.
    internal abstract void Listen();

    public virtual void StopListening()
    {
        ParentFrame?.InputListeners.Remove(this);
        UserInput.RemoveListener(this);
    }
}