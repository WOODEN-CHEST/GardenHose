using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GardenHoseEngine.IO;


public static class MouseListenerCreator
{
    // Internal static methods.
    public static IInputListener AnyButton(UserInput userInput,
        object? creator,
        GameFrame? parentFrame,
        bool requiresFocus,
        MouseCondition condition,
        EventHandler<MouseEventArgs> handler)
    {
        return new InputListener<MouseEventArgs>(userInput, creator, parentFrame, requiresFocus, 
            GetPredicateAnyButton(condition), handler);
    }

    public static IInputListener SingleButton(UserInput userInput,
        object? creator,
        GameFrame? parentFrame,
        bool requiresFocus,
        MouseCondition condition,
        EventHandler<MouseEventArgs> handler,
        MouseButton button)
    {
        return new InputListener<MouseEventArgs>(userInput, creator, parentFrame, requiresFocus,
            GetPredicateSingleButton(condition, button), handler);
    }

    public static IInputListener MultiButton(UserInput userInput,
        object? creator,
        GameFrame? parentFrame,
        bool requiresFocus,
        MouseCondition condition,
        EventHandler<MouseEventArgs> handler,
        params MouseButton[] buttons)
    {
        return new InputListener<MouseEventArgs>(userInput, creator, parentFrame, requiresFocus,
            GetPredicateMultiButton(condition, buttons), handler);
    }

    public static IInputListener Scroll(UserInput userInput,
        object?  creator,
        GameFrame? parentFrame,
        bool requiresFocus,
        ScrollDirection scrollDirection,
        EventHandler<MouseEventArgs> handler)
    {
        return new InputListener<MouseEventArgs>(userInput, creator, parentFrame, requiresFocus,
            GetPredicateScroll(scrollDirection), handler);
    }

    public static IInputListener Move(UserInput userInput,
        object? creator,
        GameFrame? parentFrame,
        bool requiresFocus,
        EventHandler<MouseEventArgs> handler)
    {
        return new InputListener<MouseEventArgs>(userInput, creator, parentFrame, requiresFocus,
            GetPreicateMove(), handler);
    }


    // Private static methods.
    private static Predicate<InputListener<MouseEventArgs>> GetPredicateAnyButton(MouseCondition condition)
    {
        return condition switch
        {
            MouseCondition.WhileDown => (self) => self.Input.MouseButtonsPressedCount.Current > 0,

            MouseCondition.WhileUp => (self) => self.Input.MouseButtonsPressedCount.Current == 0,

            MouseCondition.OnClick => (self) => 
                self.Input.MouseButtonsPressedCount.Current > self.Input.MouseButtonsPressedCount.Previous,

            MouseCondition.OnRelease => (self) =>
            {
                if (self.Input.MouseButtonsPressedCount.Current < self.Input.MouseButtonsPressedCount.Previous)
                {
                    self.Args ??= new(self.Input.VirtualMousePosition.Current);
                    return true;
                }

                if ((self.Input.MouseButtonsPressedCount.Current > self.Input.MouseButtonsPressedCount.Previous)
                    && (self.Args == null))
                {
                    self.Args = new(self.Input.VirtualMousePosition.Current);
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
            MouseCondition.WhileDown => (self) => IsButtonPressed(self.Input.MouseState.Current, button),

            MouseCondition.WhileUp => (self) => !IsButtonPressed(self.Input.MouseState.Current, button),

            MouseCondition.OnClick => (self) =>
            {
                // Flag = Button is down.
                bool ButtonWasDown = self.Flag;
                self.Flag = IsButtonPressed(self.Input.MouseState.Current, button);
                return !ButtonWasDown && self.Flag;
            },

            MouseCondition.OnRelease => (self) => 
            {
                // Flag = Button is released.
                bool ButtonWasReleased = self.Flag;
                self.Flag = !IsButtonPressed(self.Input.MouseState.Current, button);

                if (!ButtonWasReleased && self.Flag)
                {
                    self.Args ??= new(self.Input.VirtualMousePosition.Current);
                    return true;
                }

                if (ButtonWasReleased && !self.Flag)
                {
                    self.Args = new(self.Input.VirtualMousePosition.Current);
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
                    if (!IsButtonPressed(self.Input.MouseState.Current, Button))
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
                    if (IsButtonPressed(self.Input.MouseState.Current, Button))
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
                    if (!IsButtonPressed(self.Input.MouseState.Current, Button))
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
                    if (IsButtonPressed(self.Input.MouseState.Current, Button))
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
                self.Input.MouseState.Current.ScrollWheelValue != self.Input.MouseState.Previous.ScrollWheelValue,

            ScrollDirection.Up => (self) =>
                self.Input.MouseState.Current.ScrollWheelValue > self.Input.MouseState.Previous.ScrollWheelValue,

            ScrollDirection.Down => (self) =>
                self.Input.MouseState.Current.ScrollWheelValue < self.Input.MouseState.Previous.ScrollWheelValue,

            _ => throw new EnumValueException(nameof(scrollDirection), nameof(ScrollDirection),
                scrollDirection.ToString(), (int)scrollDirection)
        };
    }

    private static Predicate<InputListener<MouseEventArgs>> GetPreicateMove()
    {
        return (self) => self.Input.MouseState.Current.Position != self.Input.MouseState.Previous.Position;
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