using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GardenHoseEngine.IO;


public static class MouseListenerCreator
{
    // Internal static methods.
    public static IInputListener AnyButton(object? creator,
        bool requiresFocus,
        MouseCondition condition,
        EventHandler<MouseEventArgs> handler)
    {
        return new InputListener<MouseEventArgs>(creator, requiresFocus, 
            GetPredicateAnyButton(condition), handler);
    }

    public static IInputListener SingleButton(object? creator,
        bool requiresFocus,
        MouseCondition condition,
        EventHandler<MouseEventArgs> handler,
        MouseButton button)
    {
        return new InputListener<MouseEventArgs>(creator, requiresFocus,
            GetPredicateSingleButton(condition, button), handler);
    }

    public static IInputListener MultiButton(object? creator,
        bool requiresFocus,
        MouseCondition condition,
        EventHandler<MouseEventArgs> handler,
        params MouseButton[] buttons)
    {
        return new InputListener<MouseEventArgs>(creator, requiresFocus,
            GetPredicateMultiButton(condition, buttons), handler);
    }

    public static IInputListener Scroll(object?  creator,
        bool requiresFocus,
        ScrollDirection scrollDirection,
        EventHandler<MouseEventArgs> handler)
    {
        return new InputListener<MouseEventArgs>(creator, requiresFocus,
            GetPredicateScroll(scrollDirection), handler);
    }

    public static IInputListener Move(object? creator,
        bool requiresFocus,
        EventHandler<MouseEventArgs> handler)
    {
        return new InputListener<MouseEventArgs>(creator, requiresFocus,
            GetPreicateMove(), handler);
    }


    // Private static methods.
    private static Predicate<InputListener<MouseEventArgs>> GetPredicateAnyButton(MouseCondition condition)
    {
        return condition switch
        {
            MouseCondition.WhileDown => (self) => UserInput.MouseButtonsPressedCount.Current > 0,

            MouseCondition.WhileUp => (self) => UserInput.MouseButtonsPressedCount.Current == 0,

            MouseCondition.OnClick => (self) => 
                UserInput.MouseButtonsPressedCount.Current > UserInput.MouseButtonsPressedCount.Previous,

            MouseCondition.OnRelease => (self) =>
            {
                if (UserInput.MouseButtonsPressedCount.Current < UserInput.MouseButtonsPressedCount.Previous)
                {
                    self.Args ??= new(UserInput.VirtualMousePosition.Current);
                    return true;
                }

                if ((UserInput.MouseButtonsPressedCount.Current > UserInput.MouseButtonsPressedCount.Previous)
                    && (self.Args == null))
                {
                    self.Args = new(UserInput.VirtualMousePosition.Current);
                }
                return false;
            },

            _ => throw new EnumValueException(nameof(condition), nameof(MouseCondition),
                condition.ToString(), (int)condition)
        };
    }

    private static Predicate<InputListener<MouseEventArgs>> GetPredicateSingleButton(
        MouseCondition condition, MouseButton button)
    {
        return condition switch
        {
            MouseCondition.WhileDown => (self) => IsButtonPressed(UserInput.MouseState.Current, button),

            MouseCondition.WhileUp => (self) => !IsButtonPressed(UserInput.MouseState.Current, button),

            MouseCondition.OnClick => (self) =>
            {
                // Flag = Button is down.
                bool ButtonWasDown = self.Flag;
                self.Flag = IsButtonPressed(UserInput.MouseState.Current, button);
                return !ButtonWasDown && self.Flag;
            },

            MouseCondition.OnRelease => (self) => 
            {
                // Flag = Button is released.
                bool ButtonWasReleased = self.Flag;
                self.Flag = !IsButtonPressed(UserInput.MouseState.Current, button);

                if (!ButtonWasReleased && self.Flag)
                {
                    self.Args ??= new(UserInput.VirtualMousePosition.Current);
                    return true;
                }

                if (ButtonWasReleased && !self.Flag)
                {
                    self.Args = new(UserInput.VirtualMousePosition.Current);
                }
                return false;
            },

            _ => throw new EnumValueException(nameof(condition), nameof(MouseCondition),
                condition.ToString(), (int)condition)
        };
    }

    private static Predicate<InputListener<MouseEventArgs>> GetPredicateMultiButton(
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
            },

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
            },

            MouseCondition.OnClick => (self) =>
            {
                // Flag = All buttons are pressed.
                bool WereButtonsPressed = self.Flag;

                self.Flag = true;
                foreach (var Button in buttons)
                {
                    if (!IsButtonPressed(UserInput.MouseState.Current, Button))
                    {
                        self.Flag = false;
                        break;
                    }
                }

                return !WereButtonsPressed && self.Flag;
            },

            MouseCondition.OnRelease => (self) =>
            {
                // Flag = All buttons are released.
                bool WereButtonsReleased = self.Flag;

                self.Flag = true;
                foreach (var Button in buttons)
                {
                    if (IsButtonPressed(UserInput.MouseState.Current, Button))
                    {
                        self.Flag = false;
                        break;
                    }
                }

                return !WereButtonsReleased && self.Flag;
            },

            _ => throw new EnumValueException(nameof(condition), nameof(MouseCondition),
                condition.ToString(), (int)condition)
        };
    }

    private static Predicate<InputListener<MouseEventArgs>> GetPredicateScroll(
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

    private static Predicate<InputListener<MouseEventArgs>> GetPreicateMove()
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

            _ => throw new EnumValueException(nameof(button), nameof(MouseButton),
                button.ToString(), (int)button)
        };
    }
}