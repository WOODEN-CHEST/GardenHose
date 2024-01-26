using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GardenHoseEngine.IO;


public static class KeyboardListenerCreator
{
    // Static methods.
    public static IInputListener AnyKey(KeyCondition condition,
        EventHandler handler)
    {
        return new InputListener(false,
            GetPredicateAnyKey(condition), handler);
    }

    public static IInputListener SingleKey(object? creator,
        KeyCondition condition,
        EventHandler handler,
        Keys key)
    {
        return new InputListener(false,
            GetPredicateSingleKey(condition, key), handler);
    }

    public static IInputListener MultiKey(KeyCondition condition,
        EventHandler handler,
        params Keys[] keys)
    {
        return new InputListener(false,
            GetPredicateMultiKey(condition, keys), handler);
    }

    public static IInputListener Shortcut(KeyCondition condition,
        EventHandler handler,
        params Keys[] keys)
    {
        return new InputListener(false,
            GetPredicateShortcut(condition, keys), handler);
    }


    // Private static methods.
    private static Predicate<InputListener> GetPredicateAnyKey(KeyCondition condition)
    {
        return condition switch
        {
            KeyCondition.WhileDown => (self) => UserInput.KeysDownCount.Current > 0,
            KeyCondition.WhileUp => (self) => UserInput.KeysDownCount.Current == 0,
            KeyCondition.OnPress => (self) => UserInput.KeysDownCount.Current > UserInput.KeysDownCount.Previous,
            KeyCondition.OnRelease => (self) => UserInput.KeysDownCount.Current < UserInput.KeysDownCount.Previous,

            _ => throw new EnumValueException(nameof(condition), nameof(KeyCondition),
                condition.ToString(), (int)condition)
        };
    }

    private static Predicate<InputListener> GetPredicateSingleKey(KeyCondition condition, Keys key)
    {
        return condition switch
        {
            KeyCondition.WhileDown => (self) => UserInput.KeyboardState.Current.IsKeyDown(key),
            KeyCondition.WhileUp => (self) => UserInput.KeyboardState.Current.IsKeyUp(key),

            KeyCondition.OnPress => (self) =>
            {
                // Flag = Key is down now.
                bool KeyWasDown = self.Flag;
                self.Flag = UserInput.KeyboardState.Current.IsKeyDown(key);
                return !KeyWasDown && self.Flag;
            },

            KeyCondition.OnRelease => (self) =>
            {
                // Flag = Key is up.
                bool KeyWasUp = self.Flag;
                self.Flag = UserInput.KeyboardState.Current.IsKeyUp(key);
                return !KeyWasUp && self.Flag;
            },

            _ => throw new EnumValueException(nameof(condition), nameof(KeyCondition),
                condition.ToString(), (int)condition)
        };
    }

    private static Predicate<InputListener> GetPredicateMultiKey(KeyCondition condition, Keys[] keys)
    {
        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }
        if (keys.Length < 2)
        {
            throw new ArgumentException("A minimum of 2 keys are required.", nameof(keys));
        }

        return condition switch
        {
            
            KeyCondition.WhileDown => (self) =>
            {
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardState.Current.IsKeyUp(Key)) return false;
                }
                return true;
            },

            KeyCondition.WhileUp => (self) =>
            {
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardState.Current.IsKeyDown(Key)) return false;
                }
                return true;
            },

            KeyCondition.OnPress => (self) =>
            {
                // Flag = All keys were pressed in previous invocation.
                bool KeysDownNow = true;
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardState.Current.IsKeyUp(Key))
                    {
                        KeysDownNow = false;
                        break;
                    }
                }

                return !self.Flag && (self.Flag = KeysDownNow);
            },

            KeyCondition.OnRelease => (self) =>
            {
                // Flag = All keys were up in previous invocation.
                bool KeysUpNow = true;
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardState.Current.IsKeyDown(Key))
                    {
                        KeysUpNow = false;
                        break;
                    }
                }

                return !self.Flag && (self.Flag = KeysUpNow);
            },

            _ => throw new EnumValueException(nameof(condition), nameof(KeyCondition),
                condition.ToString(), (int)condition)
        };
    }

    private static Predicate<InputListener> GetPredicateShortcut(KeyCondition condition, Keys[] keys)
    {
        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }
        if (keys.Length < 2)
        {
            throw new ArgumentException("A minimum of 2 keys are required.", nameof(keys));
        }

        (keys[0], keys[^1]) = (keys[^1], keys[0]);

        return condition switch
        {
            KeyCondition.OnPress => (self) =>
            {
                if (!(UserInput.KeyboardState.Current.IsKeyDown(keys[0]) 
                    && UserInput.KeyboardState.Previous.IsKeyUp(keys[0])))
                {
                    return false;
                }

                for (int i = 1; i < (keys.Length - 1); i++)
                {
                    if (UserInput.KeyboardState.Current.IsKeyUp(keys[i]))
                    {
                        return false;
                    }
                }
                return true;
            },

            KeyCondition.OnRelease => (self) =>
            {
                if (!(UserInput.KeyboardState.Current.IsKeyUp(keys[0]) 
                    && UserInput.KeyboardState.Previous.IsKeyDown(keys[0])))
                {
                    return false;
                }

                for (int i = 1; i < keys.Length - 1; i++)
                {
                    if (UserInput.KeyboardState.Current.IsKeyDown(keys[i]))
                    {
                        return false;
                    }
                }
                return true;
            },

            _ => throw new ArgumentOutOfRangeException(
                $"The {nameof(KeyCondition)} value \"{condition}\" ({(int)condition}) " +
                "is not supported for shortcut keyboard events.")
        };
    }
}