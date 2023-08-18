using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GardenHoseEngine.IO;

// All this code just works, somehow. Must not touch it, it may break then.
public static class UserInput
{
    // Static fields.
    public static KeyboardState KeyboardCur => s_keyboardCur;
    public static KeyboardState KeyboardPrev => s_keyboardPrev;
    public static int KeysDownCur { get; private set; } = 0;
    public static int KeysDownPrev { get; private set; } = 0;

    public static MouseState MouseStateCur => s_mouseCur; 
    public static MouseState MouseStatePrev => s_mousePrev;
    public static Vector2 VirtualMousePosCur => s_virtualMouseCur;
    public static Vector2 VirtualMousPosPrev => s_virtualMousePrev;
    public static int PressedMouseButtonsCur { get; private set; } = 0;
    public static int PressedMouseButtonsPrev { get; private set; } = 0;


    // Private static fields.
    private static readonly HashSet<InputListener> s_listeners = new();

    private static KeyboardState s_keyboardCur;
    private static KeyboardState s_keyboardPrev;

    private static MouseState s_mouseCur;
    private static MouseState s_mousePrev;
    private static Vector2 s_virtualMouseCur;
    private static Vector2 s_virtualMousePrev;


    // Static methods.
    public static void ListenForInput(Game game)
    {
        UpdateKeyboardInfo();
        UpdateMouseInfo();

        foreach (var Listener in s_listeners)
        {
            if (Listener.RequiresFocus && !game.IsActive) continue;
            Listener.Listen();
        }
    }


    /* Keyboard input. */
    public static InputListener AddAnyKeyListener(object? creator,
        GameFrame? parentFrame,
        KeyCondition condition,
        EventHandler handler)
    {
        return new KeyboardListener(creator, parentFrame, condition, handler);
    }

    public static InputListener AddKeyListener(object? creator,
        GameFrame? parentFrame,
        KeyCondition condition,
        Keys key,
        EventHandler handler)
    {
        return new KeyboardListener(creator, parentFrame, condition, key, handler);
    }

    public static InputListener AddMultiKeyListener(object? creator,
        GameFrame? parentFrame,
        KeyCondition condition,
        EventHandler handler,
        bool isShortcut,
        params Keys[] keys)
    {
        return new KeyboardListener(creator, parentFrame, condition, handler, isShortcut, keys);
    }


    /* Mouse input */
    public static InputListener AddMouseClickListener(object? creator,
        GameFrame? parentFrame,
        MouseCondition condition,
        MouseButton button,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true)
    {
        return new MouseListener(creator, parentFrame, condition, button, handler, requiresFocus);
    }

    public static InputListener AddMouseScrollListener(object? creator,
        GameFrame? parentFrame,
        ScrollDirection scrollDirection,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true)
    {
        return new MouseListener(creator, parentFrame, scrollDirection, handler, requiresFocus);
    }

    public static InputListener AddMouseMoveListener(object? creator,
        GameFrame? parentFrame,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true)
    {
        return new MouseListener(creator, parentFrame, handler, requiresFocus);
    }


    // Internal static methods.
    internal static void AddListener(InputListener listener) => s_listeners.Add(listener);

    internal static void RemoveListener(InputListener listener) => s_listeners.Remove(listener);


    // Private static methods.
    private static void UpdateKeyboardInfo()
    {
        s_keyboardPrev = s_keyboardCur;
        KeysDownPrev = KeysDownCur;
        s_keyboardCur = Keyboard.GetState();
        KeysDownCur = s_keyboardCur.GetPressedKeyCount();
    }

    private static void UpdateMouseInfo()
    {
        // State
        s_mousePrev = MouseStateCur;
        s_mouseCur = Mouse.GetState();

        // Position
        s_virtualMousePrev = VirtualMousePosCur;
        s_virtualMouseCur.X = MouseStateCur.X;
        s_virtualMouseCur.Y = MouseStateCur.Y;
        Display.ToVirtualPosition(s_virtualMouseCur);

        // Pressed buttons.
        PressedMouseButtonsPrev = PressedMouseButtonsCur;
        if (s_mouseCur.LeftButton == ButtonState.Pressed) PressedMouseButtonsCur++;
        if (s_mouseCur.MiddleButton == ButtonState.Pressed) PressedMouseButtonsCur++;
        if (s_mouseCur.RightButton == ButtonState.Pressed) PressedMouseButtonsCur++;
    }
}