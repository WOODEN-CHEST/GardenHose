using GardenHoseEngine.IO;

namespace GardenHoseEngine.Frame.Item.Buttons;

internal struct ButtonListener
{
    // Private fields
    internal IInputListener? InputListener { get; private init; }
    internal EventHandler Handler { get; private init; }


    // Constructors.
    internal ButtonListener(IInputListener? listener, EventHandler handler)
    {
        InputListener = listener;
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }
}