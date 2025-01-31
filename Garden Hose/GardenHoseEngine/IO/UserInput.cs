﻿using GardenHoseEngine.Collections;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GardenHoseEngine.IO;

// All the code for user input just works, somehow. Very bad implementation, but a working one.
public static class UserInput
{
    // Fields.
    public static DeltaValue<KeyboardState> KeyboardState { get; private set; } = new();
    public static DeltaValue<int> KeysDownCount { get; private set; } = new();
    public static DeltaValue<MouseState> MouseState { get; private set; } = new ();
    public static DeltaValue<Vector2> VirtualMousePosition { get; private set; } = new();
    public static DeltaValue<int> MouseButtonsPressedCount { get; private set; } = new();
    public static bool IsWindowFocused { get; private set; }

    public static event EventHandler<TextInputEventArgs>? TextInput;

    public static event EventHandler<FileDropEventArgs>? FileDrop;


    // Internal methods.
    internal static void ListenForInput(bool isWindowFocused)
    {
        UpdateKeyboardInfo();
        UpdateMouseInfo();
        IsWindowFocused = isWindowFocused;
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