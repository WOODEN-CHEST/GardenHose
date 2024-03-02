using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO;

public interface IInputListener
{
    // Static functions.
    public static IInputListener CreateAnyKey(KeyCondition condition)
    {
        return new InputListener(false, GetPredicateAnyKey(condition));
    }

    public static IInputListener CreateSingleKey(KeyCondition condition, Keys key)
    {
        return new InputListener(false, GetPredicateSingleKey(condition, key));
    }

    public static IInputListener CreateMultiKey(KeyCondition condition, params Keys[] keys)
    {
        return new InputListener(false, GetPredicateMultiKey(condition, keys));
    }

    public static IInputListener CreateShortcut(KeyCondition condition, params Keys[] keys)
    {
        return new InputListener(false, GetPredicateShortcut(condition, keys));
    }

    public static IInputListener CreateAnyMouseButton(bool requiresFocus, MouseCondition condition)
    {
        return new InputListener(requiresFocus, GetPredicateAnyButton(condition));
    }

    public static IInputListener CreateSingleMouseButton(bool requiresFocus, MouseCondition condition, MouseButton button)
    {
        return new InputListener(requiresFocus, GetPredicateSingleButton(condition, button));
    }

    public static IInputListener CreateMultiMouseButton(bool requiresFocus, MouseCondition condition, params MouseButton[] buttons)
    {
        return new InputListener(requiresFocus, GetPredicateMultiButton(condition, buttons));
    }

    public static IInputListener CreateMouseScroll(bool requiresFocus, ScrollDirection scrollDirection)
    {
        return new InputListener(requiresFocus, GetPredicateScroll(scrollDirection));
    }

    public static IInputListener CreateMouseMove(bool requiresFocus)
    {
        return new InputListener(requiresFocus, GetPreicateMove());
    }


    // Functions.
    public bool Listen();


    // Private static methods.
    private static Predicate<InputListener> GetPredicateAnyKey(KeyCondition condition)
    {
        return condition switch
        {
            KeyCondition.WhileDown => (self) => UserInput.KeysDownCount.Current > 0,
            KeyCondition.WhileUp => (self) => UserInput.KeysDownCount.Current == 0,
            KeyCondition.OnPress => (self) => UserInput.KeysDownCount.Current > UserInput.KeysDownCount.Previous,
            KeyCondition.OnRelease => (self) => UserInput.KeysDownCount.Current < UserInput.KeysDownCount.Previous,

            _ => throw new EnumValueException(nameof(condition), condition)
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
                return !UserInput.KeyboardState.Previous.IsKeyDown(key) && UserInput.KeyboardState.Current.IsKeyDown(key);
            }
            ,

            KeyCondition.OnRelease => (self) =>
            {
                return UserInput.KeyboardState.Previous.IsKeyDown(key) && !UserInput.KeyboardState.Current.IsKeyDown(key);
            }
            ,

            _ => _ => throw new EnumValueException(nameof(condition), condition)
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

            KeyCondition.OnPress => (self) => // Code duplication.
            {
                bool KeysDownNow = true;
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardState.Current.IsKeyUp(Key))
                    {
                        KeysDownNow = false;
                        break;
                    }
                }

                bool KeysDownPrev = true;
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardState.Previous.IsKeyUp(Key))
                    {
                        KeysDownPrev = false;
                        break;
                    }
                }

                return KeysDownNow & !KeysDownPrev;
            },

            KeyCondition.OnRelease => (self) =>
            {
                bool KeysDownNow = true;
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardState.Current.IsKeyUp(Key))
                    {
                        KeysDownNow = false;
                        break;
                    }
                }

                bool KeysDownPrev = true;
                foreach (var Key in keys)
                {
                    if (UserInput.KeyboardState.Previous.IsKeyUp(Key))
                    {
                        KeysDownPrev = false;
                        break;
                    }
                }

                return !KeysDownNow & KeysDownPrev;
            },

            _ => throw new EnumValueException(nameof(condition), condition)
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
            KeyCondition.OnPress => (self) => // Code duplication.
            {
                if (!(UserInput.KeyboardState.Current.IsKeyDown(keys[0])
                    && UserInput.KeyboardState.Previous.IsKeyUp(keys[0])))
                {
                    return false;
                }

                for (int i = 1; i < keys.Length - 1; i++)
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

    private static Predicate<InputListener> GetPredicateAnyButton(MouseCondition condition)
    {
        return condition switch
        {
            MouseCondition.WhileDown => (self) => UserInput.MouseButtonsPressedCount.Current > 0,

            MouseCondition.WhileUp => (self) => UserInput.MouseButtonsPressedCount.Current == 0,

            MouseCondition.OnClick => (self) =>
                UserInput.MouseButtonsPressedCount.Current > UserInput.MouseButtonsPressedCount.Previous,

            MouseCondition.OnRelease => (self) =>
            {
                return UserInput.MouseButtonsPressedCount.Current < UserInput.MouseButtonsPressedCount.Previous;
            }
            ,

            _ => throw new EnumValueException(nameof(condition), nameof(MouseCondition),
                condition.ToString(), (int)condition)
        };
    }

    private static Predicate<InputListener> GetPredicateSingleButton(
        MouseCondition condition, MouseButton button)
    {
        return condition switch
        {
            MouseCondition.WhileDown => (self) => IsButtonPressed(UserInput.MouseState.Current, button),

            MouseCondition.WhileUp => (self) => !IsButtonPressed(UserInput.MouseState.Current, button),

            MouseCondition.OnClick => (self) =>
            {
                return !IsButtonPressed(UserInput.MouseState.Previous, button) && IsButtonPressed(UserInput.MouseState.Current, button);
            },

            MouseCondition.OnRelease => (self) =>
            {
                return IsButtonPressed(UserInput.MouseState.Previous, button) && !IsButtonPressed(UserInput.MouseState.Current, button);
            },

            _ => throw new EnumValueException(nameof(condition), condition)
        };
    }

    private static Predicate<InputListener> GetPredicateMultiButton(
        MouseCondition condition, params MouseButton[] buttons)
    {
        if (buttons == null)
        {
            throw new ArgumentNullException(nameof(buttons));
        }
        if (buttons.Length < 2)
        {
            throw new ArgumentException("At least two buttons are required.", nameof(buttons));
        }

        return condition switch
        {
            MouseCondition.WhileDown => (self) =>
            {
                foreach (var Button in buttons)
                {
                    if (!IsButtonPressed(UserInput.MouseState.Current, Button))
                    {
                        return false;
                    }
                }
                return true;
            }
            ,

            MouseCondition.WhileUp => (self) =>
            {
                foreach (var Button in buttons)
                {
                    if (IsButtonPressed(UserInput.MouseState.Current, Button))
                    {
                        return false;
                    }
                }
                return true;
            }
            ,

            MouseCondition.OnClick => (self) => // Code duplication.
            {
                bool WereButtonsPressed = true;
                foreach (var Button in buttons)
                {
                    if (!IsButtonPressed(UserInput.MouseState.Previous, Button))
                    {
                        WereButtonsPressed = false;
                        break;
                    }
                }

                bool AreButtonsPressed = true;
                foreach (var Button in buttons)
                {
                    if (!IsButtonPressed(UserInput.MouseState.Current, Button))
                    {
                        AreButtonsPressed = false;
                        break;
                    }
                }

                return !WereButtonsPressed && AreButtonsPressed;
            },

            MouseCondition.OnRelease => (self) =>
            {
                bool WereButtonsPressed = true;
                foreach (var Button in buttons)
                {
                    if (!IsButtonPressed(UserInput.MouseState.Previous, Button))
                    {
                        WereButtonsPressed = false;
                        break;
                    }
                }

                bool AreButtonsPressed = true;
                foreach (var Button in buttons)
                {
                    if (!IsButtonPressed(UserInput.MouseState.Current, Button))
                    {
                        AreButtonsPressed = false;
                        break;
                    }
                }

                return WereButtonsPressed && !AreButtonsPressed;
            },

            _ => throw new EnumValueException(nameof(condition), nameof(MouseCondition),
                condition.ToString(), (int)condition)
        };
    }

    private static Predicate<InputListener> GetPredicateScroll(
        ScrollDirection scrollDirection)
    {
        return scrollDirection switch
        {
            ScrollDirection.Any => (self) =>
                UserInput.MouseState.Current.ScrollWheelValue != UserInput.MouseState.Previous.ScrollWheelValue,

            ScrollDirection.Up => (self) =>
                UserInput.MouseState.Current.ScrollWheelValue > UserInput.MouseState.Previous.ScrollWheelValue,

            ScrollDirection.Down => (self) =>
                UserInput.MouseState.Current.ScrollWheelValue < UserInput.MouseState.Previous.ScrollWheelValue,

            _ => throw new EnumValueException(nameof(scrollDirection), nameof(ScrollDirection),
                scrollDirection.ToString(), (int)scrollDirection)
        };
    }

    private static Predicate<InputListener> GetPreicateMove()
    {
        return (self) => UserInput.MouseState.Current.Position != UserInput.MouseState.Previous.Position;
    }

    private static bool IsButtonPressed(in MouseState mouseState, MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => mouseState.LeftButton == ButtonState.Pressed,
            MouseButton.Middle => mouseState.MiddleButton == ButtonState.Pressed,
            MouseButton.Right => mouseState.RightButton == ButtonState.Pressed,
            _ => throw new EnumValueException(nameof(button), button)
        };
    }
}