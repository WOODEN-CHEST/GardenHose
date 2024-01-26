using GardenHoseEngine.Frame;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace GardenHoseEngine.IO;

internal class InputListener : IInputListener
{
    // Internal fields.
    internal bool IsWindowFocusRequired { get; private init; }
    internal bool Flag { get; set; } = true;
    

    // Private fields.
    private EventHandler _handler;
    private Predicate<InputListener> _predicate { get; init; }


    // Constructors.
    internal InputListener(bool requiresFocus,
        Predicate<InputListener> predicate,
        EventHandler handler)
    {
        IsWindowFocusRequired = requiresFocus;
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    }


    // Inherited methods.
    public void Listen(bool windowFocused)
    {
        if (_predicate.Invoke(this) 
            && ((IsWindowFocusRequired && windowFocused) || !IsWindowFocusRequired))
        {
            _handler.Invoke(this, EventArgs.Empty);
        }
    }

    public void StopListening()
    {
        UserInput.RemoveListener(this);
    }

    public void StartListening()
    {
        UserInput.AddListener(this);
    }
}