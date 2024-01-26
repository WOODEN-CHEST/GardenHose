using GardenHoseEngine.IO;

namespace GardenHoseEngine.Frame.Item.Buttons;

internal class ButtonHandler
{
    // Private fields
    private IInputListener? _inputListerer;
    private EventHandler _handler;

    // Constructors.
    internal ButtonHandler(IInputListener? listener, EventHandler handler)
    {
        _inputListerer = listener;
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _inputListerer?.StartListening();
    }


    // Internal methods.
    internal void StopListening()
    {
        _inputListerer?.StopListening();
    }

    internal void InvokeEvent(SpriteButton button)
    {
        _handler.Invoke(button, EventArgs.Empty);
    }
}