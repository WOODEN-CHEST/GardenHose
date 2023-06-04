using GardenHose.Engine.Frame;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GardenHose.Engine.IO;


public class KeyboardListener : InputListener
{
    // Private fields.
    private bool _passedPrev = false;
    private readonly Keys[] _requiredKeys;
    private readonly EventHandler _handler;


    // Constructors.
    public KeyboardListener(object creator, GameFrame parentFrame,
        KeyCondition condition,
        Keys key,
        EventHandler handler,
        bool requiresFocus = true) : base(creator, parentFrame, requiresFocus)
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

    public KeyboardListener(object creator, GameFrame parentFrame,
        KeyCondition condition,
        EventHandler handler,
        bool requiresFocus = true) : base(creator, parentFrame, requiresFocus)
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

    public KeyboardListener(object creator, GameFrame parentFrame,
        KeyCondition condition,
        EventHandler handler,
        bool requiresFocus,
        bool requiresOrder,
        params Keys[] keys) : base(creator, parentFrame, requiresFocus)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        if (keys.Length < 2) 
            throw new ArgumentException($"Not enough keys to listen to: {keys.Length}. Required: >= 2.");

        _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        if (!requiresOrder)
        {
            ConditionFunc = condition switch
            {
                KeyCondition.WhileDown => () =>
                {
                    foreach (var Key in keys) if (UserInput.KeyboardCur.IsKeyUp(Key)) return false;
                    return true;
                },

                KeyCondition.WhileUp => () =>
                {
                    foreach (var Key in keys) if (UserInput.KeyboardCur.IsKeyDown(Key)) return false;
                    return true;
                },

                KeyCondition.OnPress => () =>
                {
                    bool PassedNow = true;
                    foreach (var Key in keys)
                    {
                        if (UserInput.KeyboardCur.IsKeyUp(Key)) PassedNow = false;
                    }

                    bool Result = PassedNow && !_passedPrev;
                    _passedPrev = PassedNow;
                    return Result;
                },

                KeyCondition.OnRelease => () =>
                {
                    bool PassedNow = true;
                    foreach (var Key in keys)
                    {
                        if (UserInput.KeyboardCur.IsKeyDown(Key)) PassedNow = false;
                    }

                    bool Result = PassedNow && !_passedPrev;
                    _passedPrev = PassedNow;
                    return Result;
                }
                ,

                _ => throw new ArgumentException(
                $"Invalid {nameof(KeyCondition)} value: \"{condition}\" ({(int)condition})")
            };
            return;
        }

        Keys Temp = keys[0];
        keys[0] = keys[^1];
        keys[^1] = Temp;

        ConditionFunc = condition switch
        {
            KeyCondition.OnPress => () =>
            {
                if (UserInput.KeyboardCur.IsKeyDown(keys[0]) && UserInput.KeyboardPrev.IsKeyUp(keys[0]))
                {
                    for (int i = 0; i < keys.Length - 1; i++)
                    {
                        if (UserInput.KeyboardCur.IsKeyUp(keys[i])) return false;
                    }
                    return true;
                }
                return false;
            },

            KeyCondition.OnRelease => () =>
            {
                if (UserInput.KeyboardCur.IsKeyUp(keys[0]) && UserInput.KeyboardPrev.IsKeyDown(keys[0]))
                {
                    for (int i = 0; i < keys.Length - 1; i++)
                    {
                        if (UserInput.KeyboardCur.IsKeyDown(keys[i])) return false;
                    }
                    return true;
                }
                return false;
            },

            _ => throw new ArgumentException(
                $"The {nameof(KeyCondition)} value \"{condition}\" ({(int)condition}) " +
                "is not supported for ordered keyboard click events.")
        };
    }


    // Inherited methods.
    public override void Listen()
    {
        if (ConditionFunc.Invoke()) _handler.Invoke(Creator, EventArgs.Empty);
    }
}