using Microsoft.Xna.Framework.Input;


namespace GardenHoseEngine.IO;


public static class MouseListenerCreator
{
    // Internal static methods.
    public static IInputListener AnyButton(bool requiresFocus,
        MouseCondition condition,
        EventHandler handler)
    {
        return new InputListener(requiresFocus, 
            GetPredicateAnyButton(condition), handler);
    }

    public static IInputListener SingleButton(bool requiresFocus,
        MouseCondition condition,
        EventHandler handler,
        MouseButton button)
    {
        return new InputListener(requiresFocus,
            GetPredicateSingleButton(condition, button), handler);
    }

    public static IInputListener MultiButton(bool requiresFocus,
        MouseCondition condition,
        EventHandler handler,
        params MouseButton[] buttons)
    {
        return new InputListener(requiresFocus,
            GetPredicateMultiButton(condition, buttons), handler);
    }

    public static IInputListener Scroll(bool requiresFocus,
        ScrollDirection scrollDirection,
        EventHandler handler)
    {
        return new InputListener(requiresFocus,
            GetPredicateScroll(scrollDirection), handler);
    }

    public static IInputListener Move(bool requiresFocus,
        EventHandler handler)
    {
        return new InputListener(requiresFocus,
            GetPreicateMove(), handler);
    }


    // Private static methods.
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
            },

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

                return !ButtonWasReleased && self.Flag;
            },

            _ => throw new EnumValueException(nameof(condition), nameof(MouseCondition),
                condition.ToString(), (int)condition)
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

            _ => throw new EnumValueException(nameof(button), nameof(MouseButton),
                button.ToString(), (int)button)
        };
    }
}