namespace GardenHoseEngine.IO;

internal class InputListener : IInputListener
{
    // Internal fields.
    internal bool IsWindowFocusRequired { get; private init; }
    

    // Private fields.
    private Predicate<InputListener> _predicate { get; init; }


    // Constructors.
    internal InputListener(bool requiresFocus,
        Predicate<InputListener> predicate)
    {
        IsWindowFocusRequired = requiresFocus;
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    }


    // Inherited methods.
    public bool Listen()
    {
        return _predicate.Invoke(this) && ((IsWindowFocusRequired && UserInput.IsWindowFocused) || !IsWindowFocusRequired);
    }
}