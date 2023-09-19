using GardenHoseEngine.Frame;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GardenHoseEngine.IO;

// All this code just works, somehow. Must not touch it, it may break then.
public class UserInput
{
    // Fields.
    public DeltaValue<KeyboardState> KeyboardState { get; private set; } = new();

    public DeltaValue<int> KeysDownCount { get; private set; } = new();


    public DeltaValue<MouseState> MouseState { get; private set; } = new ();

    public DeltaValue<Vector2> VirtualMousePosition { get; private set; } = new();

    public DeltaValue<int> MouseButtonsPressedCount { get; private set; } = new();


    public event EventHandler<TextInputEventArgs> TextInput;

    public event EventHandler<FileDropEventArgs> FileDrop;


    // Private fields.
    private readonly List<IInputListener> _listeners = new();

    private readonly IVirtualConverter _converter;


    // Constructors.
    public UserInput(IVirtualConverter converter, GameWindow window)
    {
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));

        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        window.TextInput += OnTextInputEvent;
        window.FileDrop += OnFileDropEvent;
    }


    // Methods.
    public void AddListener(IInputListener listener) => _listeners.Add(listener);

    public void RemoveListener(IInputListener listener) => _listeners.Remove(listener);


    // Internal methods.
    internal void ListenForInput(bool isWindowFocused)
    {
        UpdateKeyboardInfo();
        UpdateMouseInfo();

        foreach (var Listener in _listeners)
        {
            Listener.Listen(isWindowFocused);
        }
    } 


    // Private methods.
    private void UpdateKeyboardInfo()
    {
        KeyboardState NewState = Keyboard.GetState();
        KeyboardState.Update(NewState);
        KeysDownCount.Update(KeyboardState.Current.GetPressedKeyCount());
    }

    private void UpdateMouseInfo()
    {
        MouseState.Update(Mouse.GetState());
        VirtualMousePosition.Update(_converter.ToVirtualPosition(MouseState.Current.Position.ToVector2()));

        int MouseButtonsPressed = 0;
        if (MouseState.Current.LeftButton == ButtonState.Pressed) MouseButtonsPressed++;
        if (MouseState.Current.MiddleButton == ButtonState.Pressed) MouseButtonsPressed++;
        if (MouseState.Current.RightButton == ButtonState.Pressed) MouseButtonsPressed++;
        MouseButtonsPressedCount.Update(MouseButtonsPressed);
    }

    private void OnTextInputEvent(object? sender, TextInputEventArgs args)
    {
        TextInput?.Invoke(this, args);
    }

    private void OnFileDropEvent(object? sender, FileDropEventArgs args)
    {
        FileDrop?.Invoke(this, args);
    }
}