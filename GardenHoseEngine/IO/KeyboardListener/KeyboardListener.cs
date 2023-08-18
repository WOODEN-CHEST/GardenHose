using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GardenHoseEngine.IO;


internal sealed class KeyboardListener : InputListener
{
    // Internal fields.
    internal override Func<bool> ConditionFunc { get ; init; }


    // Private fields.
    private readonly EventHandler _handler;


    // Constructors.
    internal KeyboardListener(object? creator,
        GameFrame? parentFrame,
        KeyCondition condition,
        EventHandler handler) : base(creator, parentFrame, true)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        ConditionFunc = condition switch
        {
            KeyCondition.WhileDown => () => UserInput.KeysDownCur > 0,
            KeyCondition.WhileUp => () => UserInput.KeysDownCur == 0,
            KeyCondition.OnPress => () => UserInput.KeysDownCur > UserInput.KeysDownPrev,
            KeyCondition.OnRelease => () => UserInput.KeysDownCur < UserInput.KeysDownPrev,

            _ => throw new ArgumentException(
                $"Invalid {nameof(KeyCondition)} value: \"{condition}\" ({(int)condition})")
        };
    }

    internal KeyboardListener(object? creator, 
        GameFrame? parentFrame,
        KeyCondition condition,
        Keys key,
        EventHandler handler) 
        : base(creator, parentFrame, true) 
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        ConditionFunc = condition switch
        {
            KeyCondition.WhileDown => () => UserInput.KeyboardCur.IsKeyDown(key),
            KeyCondition.WhileUp => () => UserInput.KeyboardCur.IsKeyUp(key),

            KeyCondition.OnPress => () => UserInput.KeyboardCur.IsKeyDown(key)
                && UserInput.KeyboardPrev.IsKeyUp(key),

            KeyCondition.OnRelease => () => UserInput.KeyboardCur.IsKeyUp(key)
                && UserInput.KeyboardPrev.IsKeyDown(key),

            _ => throw new ArgumentException(
                $"Invalid {nameof(KeyCondition)} value: \"{condition}\" ({(int)condition})")
        };
    }

    internal KeyboardListener(object? creator, 
        GameFrame?   parentFrame,
        KeyCondition condition,
        EventHandler handler,
        bool isShortcut,
        params Keys[] keys) :
        base(creator, parentFrame, true)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }
        if (keys.Length < 2)
        {
            throw new ArgumentException($"Not enough keys to listen to: {keys.Length}. Requires 2 or more keys.");
        }

        ConditionFunc = isShortcut ? GetFuncShortcut(condition, keys) : GetFuncMulti(condition, keys);
    }


    // Private static methods.
    private static Func<bool> GetFuncMulti(KeyCondition condition, Keys[] keys)
    {
        return condition switch
        {
            KeyCondition.WhileDown => () =>
            {
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardCur.IsKeyUp(Key)) return false;
                }
                return true;
            }
            ,

            KeyCondition.WhileUp => () =>
            {
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardCur.IsKeyDown(Key)) return false;
                }
                return true;
            }
            ,

            KeyCondition.OnPress => () =>
            {
                bool KeysDownPrev = true;
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardPrev.IsKeyUp(Key))
                    {
                        KeysDownPrev = false;
                        break;
                    }
                }

                bool KeysDownNow = true;
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardCur.IsKeyUp(Key))
                    {
                        KeysDownNow = false;
                        break;
                    }
                }

                return !KeysDownPrev && KeysDownNow;
            }
            ,

            KeyCondition.OnRelease => () =>
            {
                bool KeysDownPrev = true;
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardPrev.IsKeyUp(Key))
                    {
                        KeysDownPrev = false;
                        break;
                    }
                }

                bool KeysDownNow = true;
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardCur.IsKeyUp(Key))
                    {
                        KeysDownNow = false;
                        break;
                    }
                }

                return KeysDownPrev && !KeysDownNow;
            }
            ,

            _ => throw new ArgumentException(
            $"Invalid {nameof(KeyCondition)} value: \"{condition}\" ({(int)condition})")
        };
    }

    private static Func<bool> GetFuncShortcut(KeyCondition condition, Keys[] keys)
    {
        (keys[0], keys[^1]) = (keys[^1], keys[0]);

        return condition switch
        {
            KeyCondition.OnPress => () =>
            {
                if (!(UserInput.KeyboardCur.IsKeyDown(keys[0]) && UserInput.KeyboardPrev.IsKeyUp(keys[0])))
                {
                    return false;
                }

                for (int i = 1; i < keys.Length - 1; i++)
                {
                    if (UserInput.KeyboardCur.IsKeyUp(keys[i])) return false;
                }
                return true;
            }
            ,

            KeyCondition.OnRelease => () =>
            {
                if (!(UserInput.KeyboardCur.IsKeyUp(keys[0]) && UserInput.KeyboardPrev.IsKeyDown(keys[0])))
                {
                    return false;
                }

                for (int i = 1; i < keys.Length - 1; i++)
                {
                    if (UserInput.KeyboardCur.IsKeyDown(keys[i])) return false;
                }
                return true;
            }
            ,

            _ => throw new ArgumentException(
                $"The {nameof(KeyCondition)} value \"{condition}\" ({(int)condition}) " +
                "is not supported for shortcut keyboard events.")
        };
    }


    // Inherited methods.
    internal override void Listen()
    {
        if (ConditionFunc.Invoke())
        {
            _handler.Invoke(Creator, EventArgs.Empty);
        }
    }
}