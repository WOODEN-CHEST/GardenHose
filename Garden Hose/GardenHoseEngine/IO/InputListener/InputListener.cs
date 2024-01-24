using GardenHoseEngine.Frame;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace GardenHoseEngine.IO;

internal class InputListener<ArgsType> : IInputListener where ArgsType : EventArgs
{
    // Internal fields.
    internal readonly object Creator;
    internal readonly IGameFrame? ParentFrame;
    internal readonly bool IsWindowFocusRequired;
    internal ArgsType Args { get; set; } = default!;
    internal bool Flag { get; set; } = true;
    

    // Private fields.
    private EventHandler<ArgsType> _handler;
    private Predicate<InputListener<ArgsType>> _predicate { get; init; }


    // Constructors.
    internal InputListener(object? creator,
        bool requiresFocus,
        Predicate<InputListener<ArgsType>> predicate,
        EventHandler<ArgsType> handler)
    {
        IsWindowFocusRequired = requiresFocus;

        if (creator != null) Creator = creator;
        else Creator = this;

        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    }


    // Inherited methods.
    public void Listen(bool windowFocused)
    {
        if (_predicate.Invoke(this) 
            && ((IsWindowFocusRequired && windowFocused) || !IsWindowFocusRequired))
        {
            _handler.Invoke(this, Args);
        }
    }

    public void StopListening()
    {
        UserInput.RemoveListener(this);
    }
}