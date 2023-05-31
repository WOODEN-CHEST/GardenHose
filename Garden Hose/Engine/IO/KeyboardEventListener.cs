using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace GardenHose.Engine.IO;


public delegate void KeyboardEventHandler();


public enum KeyEventTrigger
{
    WhileDown,
    WhileUp,
    OnPress,
    OnRelease
}


public class KeyboardEventListener : IInputEventListener
{
    // Types.
    public delegate bool ConditionMethod();


    // Static fields.
    public static KeyboardState StateCur = Keyboard.GetState();
    public static KeyboardState StatePrev = Keyboard.GetState();
    public static int KeysDownCur = StateCur.GetPressedKeyCount();
    public static int KeysDownPrev = StatePrev.GetPressedKeyCount();



    // Private static fields.
    private static HashSet<KeyboardEventListener> s_listeners = new();


    // Fields.
    public readonly KeyboardEventHandler Handler;
    public readonly Keys[] RequiredKeys;
    public readonly ConditionMethod Condition;


    // Private fields.
    private bool _keyFlagPrev = true;


    // Constructors.
    private KeyboardEventListener(
        KeyboardEventHandler eventHandler,
        KeyEventTrigger condition,
        params Keys[] keys)
    {
        if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
        else if (keys == null) throw new ArgumentNullException(nameof(keys));

        Handler = eventHandler;
        RequiredKeys = keys;

        if (keys.Length > 0)
        {
            Condition = condition switch
            {
                KeyEventTrigger.WhileDown => AreKeysDown,
                KeyEventTrigger.WhileUp => AreKeysUp,
                KeyEventTrigger.OnPress => WereKeysPressed,
                KeyEventTrigger.OnRelease => WereKeysReleased,
                _ => AreKeysDown
            };
        }
        else
        {
            Condition = condition switch
            {
                KeyEventTrigger.WhileDown => IsAnyKeyDown,
                KeyEventTrigger.WhileUp => AreAllKeysUp,
                KeyEventTrigger.OnPress => WasAnyKeyPressed,
                KeyEventTrigger.OnRelease => WasAnyKeyReleased,
                _ => IsAnyKeyDown
            };
        }

    }


    // Static methods.
    public static void ListenForInput()
    {
        KeysDownPrev = KeysDownCur;
        StatePrev = StateCur;

        StateCur = Keyboard.GetState();
        KeysDownCur = StateCur.GetPressedKeyCount();

        foreach (KeyboardEventListener Listener in s_listeners)
        {
            if (Listener.Condition.Invoke()) Listener.Handler.Invoke();
        }
    }

    public static KeyboardEventListener AddListener(
        KeyboardEventHandler handler,
        KeyEventTrigger condition,
        params Keys[] keys)
    {
        KeyboardEventListener Listener = new(handler, condition, keys);
        s_listeners.Add(Listener);
        return Listener;
    }

    public static void RemoveListener(KeyboardEventListener listener) => s_listeners.Remove(listener);

    public static void ClearListeners() => s_listeners.Clear();


    // Private methods.
    private bool IsAnyKeyDown() => KeysDownCur > 0;
    private bool AreAllKeysUp() => KeysDownCur == 0;
    private bool WasAnyKeyPressed() => KeysDownCur > KeysDownPrev; // May not work with precise timing.
    private bool WasAnyKeyReleased() => KeysDownCur < KeysDownPrev; // Same issue.


    private bool AreKeysDown()
    {
        foreach (Keys key in RequiredKeys) if (StateCur.IsKeyUp(key)) return false;
        return true;
    }

    private bool AreKeysUp()
    {
        foreach (Keys key in RequiredKeys) if (StateCur.IsKeyDown(key)) return false;
        return true;
    }

    private bool WereKeysPressed()
    {
        bool KeysAreDown = true;

        foreach (Keys key in RequiredKeys)
        {
            if (StateCur.IsKeyUp(key))
            {
                KeysAreDown = false;
                break;
            }
        }

        bool Result = !_keyFlagPrev && KeysAreDown;
        _keyFlagPrev = KeysAreDown;
        return Result;
    }

    private bool WereKeysReleased()
    {
        bool KeysAreUp = true;

        foreach (Keys key in RequiredKeys)
        {
            if (StatePrev.IsKeyDown(key))
            {
                KeysAreUp = false;
                break;
            }
        }

        bool Result = !_keyFlagPrev && KeysAreUp;
        _keyFlagPrev = KeysAreUp;
        return Result;
    }


    // Inherited methods.
    public void StopListening() => RemoveListener(this);
}