using GardenHose.Engine.Frame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GardenHose.Engine.IO;

// All this code just works, somehow. Must not touch it, it may break then.
public static class UserInput
{
    // Static fields.
    public static KeyboardState KeyboardCur{ get => s_keyboardCur; }
    public static KeyboardState KeyboardPrev { get => s_keyboardPrev; }
    public static int KeysDownCur { get; private set; }
    public static int KeysDownPrev { get; private set; }

    public static MouseState MouseCur { get => s_mouseCur; }
    public static MouseState MousePrev { get => s_mouseCur; }
    public static Vector2 VirtualMouseCur { get => s_virtualMouseCur; }
    public static Vector2 VirtualMousePrev { get => s_virtualMousePrev; }


    // Private static fields.
    private static readonly HashSet<InputListener> s_listeners = new();

    private static KeyboardState s_keyboardCur;
    private static KeyboardState s_keyboardPrev;

    private static MouseState s_mouseCur;
    private static MouseState s_mousePrev;
    private static Vector2 s_virtualMouseCur;
    private static Vector2 s_virtualMousePrev;


    // Static methods.
    public static void ListenForInput()
    {
        UpdateKeyboardInfo();
        UpdateMouseInfo();

        foreach (var Listener in s_listeners)
        {
            if (Listener.RequiresFocus && !MainGame.Instance.IsActive) continue;
            Listener.Listen();
        }
    }

    public static void AddListener(InputListener listener) => s_listeners.Add(listener);

    public static void RemoveListener(InputListener listener) => s_listeners.Remove(listener);


    /* Keyboard input. */
    public static KeyboardListener AddAnyKeyListener(object creator,
        GameFrame parentFrame,
        KeyCondition condition,
        EventHandler handler,
        bool requiresFocus = true)
    {
        return new KeyboardListener(creator, parentFrame, condition, handler, requiresFocus);
    }

    public static KeyboardListener AddKeyListener(object creator,
        GameFrame parentFrame,
        KeyCondition condition,
        Keys key,
        EventHandler handler,
        bool requiresFocus = true)
    {
        return new KeyboardListener(creator, parentFrame, condition, key, handler, requiresFocus);
    }

    public static KeyboardListener AddMultiKeyListener(object creator,
        GameFrame parentFrame,
        KeyCondition condition,
        EventHandler handler,
        bool requiresFocus,
        params Keys[] keys)
    {
        return new KeyboardListener(creator, parentFrame, condition, handler, requiresFocus, false, keys);
    }

    public static KeyboardListener AddOrderedMultiKeyListener(object creator,
        GameFrame parentFrame,
        KeyCondition condition,
        EventHandler handler,
        bool requiresFocus,
        params Keys[] keys)
    {
        return new KeyboardListener(creator, parentFrame, condition, handler, requiresFocus, true, keys);
    }


    /* Mouse input */
    public static MouseListener AddMouseClickListener(object creator,
        GameFrame parentFrame,
        MouseCondition condition,
        MouseButton button,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true)
    {
        return new MouseListener(creator, parentFrame, condition, button, handler, requiresFocus);
    }

    public static MouseListener AddMouseScrollListener(object creator,
        GameFrame parentFrame,
        ScrollDirection scrollDirection,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true)
    {
        return new MouseListener(creator, parentFrame, scrollDirection, handler, requiresFocus);
    }

    public static MouseListener AddMouseMoveListener(object creator,
        GameFrame parentFrame,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true)
    {
        return new MouseListener(creator, parentFrame, handler, requiresFocus);
    }


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
        // Update fields.
        s_mousePrev = MouseCur;
        s_mouseCur = Mouse.GetState();

        s_virtualMousePrev = VirtualMouseCur;
        s_virtualMouseCur.X = MouseCur.X;
        s_virtualMouseCur.Y = MouseCur.Y;
        DisplayInfo.RealToVirtualPosition(ref s_virtualMouseCur);
    }
}