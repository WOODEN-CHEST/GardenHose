using GardenHose.Engine.Frame;
using GardenHose.Engine.Frame.UI.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GardenHose.Engine.IO;


public class MouseListener : InputListener
{
    // Private fields.
    private readonly MouseButton _requiredButton;
    private readonly MouseEventArgs _eventArgs = new();
    private readonly EventHandler<MouseEventArgs> _handler;


    // Constructors.
    public MouseListener(object creator, 
        GameFrame parentFrame,
        MouseCondition condition, 
        MouseButton button,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true) : base(creator, parentFrame, requiresFocus)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _requiredButton = button;

        if (button == MouseButton.Any)
        {
            ConditionFunc = condition switch
            {
                MouseCondition.WhileDown => IsAnyButtonDown,
                MouseCondition.WhileUp => AreAllButtonsUp,
                MouseCondition.OnClick => WasAnyButtonClicked,
                MouseCondition.OnRelease => WasAnyButtonReleased,
                _ => throw new ArgumentException(
                $"Invalid {nameof(MouseCondition)} value: \"{condition}\" ({(int)condition})")
            };
            return;
        }

        ConditionFunc = condition switch
        {
            MouseCondition.WhileDown => () => (GetButtonState(UserInput.MouseCur) == ButtonState.Pressed),
            MouseCondition.WhileUp => () => (GetButtonState(UserInput.MouseCur) == ButtonState.Released),

            MouseCondition.OnClick => () => (GetButtonState(UserInput.MouseCur) == ButtonState.Pressed)
            && (GetButtonState(UserInput.MousePrev) == ButtonState.Released),

            MouseCondition.OnRelease => () => {
                if ((GetButtonState(UserInput.MouseCur) == ButtonState.Released)
                    && (GetButtonState(UserInput.MousePrev) == ButtonState.Pressed))
                {
                    return true;
                }
                _eventArgs.StartPosition = UserInput.VirtualMouseCur;
                return false;
            }, 

            _ => throw new ArgumentException(
                $"Invalid {nameof(MouseCondition)} value: \"{condition}\" ({(int)condition})")
        };
    }

    public MouseListener(object creator, 
        GameFrame parentFrame,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true) : base(creator, parentFrame, requiresFocus)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        ConditionFunc = () => UserInput.MouseCur.Position != UserInput.MouseCur.Position;
    }

    public MouseListener(object creator,
        GameFrame parentFrame,
        ScrollDirection scrollDirection,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true) : base(creator, parentFrame, requiresFocus)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        ConditionFunc = scrollDirection switch
        {
            ScrollDirection.Stationary => () => UserInput.MouseCur.ScrollWheelValue == UserInput.MousePrev.ScrollWheelValue,
            ScrollDirection.Any => () => UserInput.MouseCur.ScrollWheelValue != UserInput.MousePrev.ScrollWheelValue,
            ScrollDirection.Up => () => UserInput.MouseCur.ScrollWheelValue > UserInput.MousePrev.ScrollWheelValue,
            ScrollDirection.Down => () => UserInput.MouseCur.ScrollWheelValue < UserInput.MousePrev.ScrollWheelValue,
            _ => throw new ArgumentException(
                $"Invalid {nameof(ScrollDirection)} value: \"{scrollDirection}\" ({(int)scrollDirection})")
        };
    }


    // Private methods.
    /* Conditional functions for methods. */
    private bool IsAnyButtonDown()
    {
        return UserInput.MouseCur.LeftButton == ButtonState.Pressed
            || UserInput.MouseCur.MiddleButton == ButtonState.Pressed
            || UserInput.MouseCur.MiddleButton == ButtonState.Pressed;
    }

    private bool AreAllButtonsUp()
    {
        return UserInput.MouseCur.LeftButton == ButtonState.Released
            && UserInput.MouseCur.MiddleButton == ButtonState.Released
            && UserInput.MouseCur.MiddleButton == ButtonState.Released;
    }

    private bool WasAnyButtonClicked()
    {
        byte PressedCountPrev = 0;
        if (UserInput.MouseCur.LeftButton == ButtonState.Pressed) PressedCountPrev++;
        if (UserInput.MouseCur.MiddleButton == ButtonState.Pressed) PressedCountPrev++;
        if (UserInput.MouseCur.RightButton == ButtonState.Pressed) PressedCountPrev++;

        byte PressedCountCur = 0;
        if (UserInput.MouseCur.LeftButton == ButtonState.Pressed) PressedCountCur++;
        if (UserInput.MouseCur.MiddleButton == ButtonState.Pressed) PressedCountCur++;
        if (UserInput.MouseCur.RightButton == ButtonState.Pressed) PressedCountCur++;

        return PressedCountCur > PressedCountPrev;
    }

    private bool WasAnyButtonReleased()
    {
        byte PressedCountPrev = 0;
        if (UserInput.MouseCur.LeftButton == ButtonState.Pressed) PressedCountPrev++;
        if (UserInput.MouseCur.MiddleButton == ButtonState.Pressed) PressedCountPrev++;
        if (UserInput.MouseCur.RightButton == ButtonState.Pressed) PressedCountPrev++;

        byte PressedCountCur = 0;
        if (UserInput.MouseCur.LeftButton == ButtonState.Pressed) PressedCountCur++;
        if (UserInput.MouseCur.MiddleButton == ButtonState.Pressed) PressedCountCur++;
        if (UserInput.MouseCur.RightButton == ButtonState.Pressed) PressedCountCur++;

        if (PressedCountCur < PressedCountPrev)
        {
            _eventArgs.StartPosition = UserInput.VirtualMouseCur;
            return true;
        }
        return false;
    }

    private ButtonState GetButtonState(in MouseState mouseState)
    {
        return _requiredButton switch
        {
            MouseButton.Left => mouseState.LeftButton,
            MouseButton.Middle => mouseState.MiddleButton,
            MouseButton.Right => mouseState.RightButton,
            _ => mouseState.LeftButton
        };
    }


    // Inherited methods.
    public override void Listen()
    {
        if (ConditionFunc.Invoke()) _handler.Invoke(Creator, _eventArgs);
    }
}