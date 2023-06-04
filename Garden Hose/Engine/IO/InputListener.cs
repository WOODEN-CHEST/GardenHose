using GardenHose.Engine.Frame;
using System;

namespace GardenHose.Engine.IO;

public abstract class InputListener
{
    // Fields.
    public readonly object Creator;
    public readonly GameFrame ParentFrame;
    public readonly bool RequiresFocus;
    public Func<bool> ConditionFunc { get; protected set; }


    // Constructors.
    public InputListener(object creator, GameFrame parentFrame, bool requiresFocus)
    {
        ParentFrame = parentFrame;
        RequiresFocus = requiresFocus;
        UserInput.AddListener(this);

        if (creator == null)
        {
            if (parentFrame != null) Creator = parentFrame;
            else Creator = this;
        }
        else Creator = creator;
    }


    // Methods.
    public abstract void Listen();

    public virtual void StopListening()
    {
        ParentFrame?.InputListeners.Remove(this);
        UserInput.RemoveListener(this);
    }
}