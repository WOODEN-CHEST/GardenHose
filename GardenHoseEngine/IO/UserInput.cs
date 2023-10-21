using GardenHoseEngine.Collections;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GardenHoseEngine.IO;

// All this code just works, somehow.
public static class UserInput
{
    // Fields.
    public static DeltaValue<KeyboardState> KeyboardState { get; private set; } = new();

    public static DeltaValue<int> KeysDownCount { get; private set; } = new();


    public static DeltaValue<MouseState> MouseState { get; private set; } = new ();

    public static DeltaValue<Vector2> VirtualMousePosition { get; private set; } = new();

    public static DeltaValue<int> MouseButtonsPressedCount { get; private set; } = new();


    public static event EventHandler<TextInputEventArgs> TextInput;

    public static event EventHandler<FileDropEventArgs> FileDrop;


    // Private fields.
    private static readonly DiscreteTimeList<IInputListener> s_listeners = new();


    // Methods.
    public static void AddListener(IInputListener listener) => s_listeners.Add(listener);

    public static void RemoveListener(IInputListener listener) => s_listeners.Remove(listener);


    // Internal methods.
    internal static void ListenForInput(bool isWindowFocused)
    {
        UpdateKeyboardInfo();
        UpdateMouseInfo();

        s_listeners.ApplyChanges();
        foreach (var Listener in s_listeners)
        {
            Listener.Listen(isWindowFocused);
        }
    }


    // Internal fields.
    internal static void OnTextInputEvent(object? sender, TextInputEventArgs args)
    {
        TextInput?.Invoke(null, args);
    }

    internal static void OnFileDropEvent(object? sender, FileDropEventArgs args)
    {
        FileDrop?.Invoke(null, args);
    }


    // Private methods.
    private static void UpdateKeyboardInfo()
    {
        KeyboardState NewState = Keyboard.GetState();
        KeyboardState.Update(NewState);
        KeysDownCount.Update(KeyboardState.Current.GetPressedKeyCount());
    }

    private static void UpdateMouseInfo()
    {
        MouseState.Update(Mouse.GetState());
        VirtualMousePosition.Update(Display.ToVirtualPosition(MouseState.Current.Position.ToVector2()));

        int MouseButtonsPressed = 0;
        if (MouseState.Current.LeftButton == ButtonState.Pressed) MouseButtonsPressed++;
        if (MouseState.Current.MiddleButton == ButtonState.Pressed) MouseButtonsPressed++;
        if (MouseState.Current.RightButton == ButtonState.Pressed) MouseButtonsPressed++;
        MouseButtonsPressedCount.Update(MouseButtonsPressed);
    }
}