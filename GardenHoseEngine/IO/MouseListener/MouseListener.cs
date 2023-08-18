using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.UI.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GardenHoseEngine.IO;


public sealed class MouseListener : InputListener
{
    // Internal fields.
    internal override Func<bool> ConditionFunc { get; init; }


    // Private fields.
    private MouseEventArgs _eventArgs = new();
    private readonly EventHandler<MouseEventArgs> _handler;


    // Constructors.
    internal MouseListener(object? creator, 
        GameFrame? parentFrame,
        MouseCondition condition, 
        MouseButton button,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true) : base(creator, parentFrame, requiresFocus)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));

        if (button == MouseButton.Any)
        {
            ConditionFunc = condition switch
            {
                MouseCondition.WhileDown => () => UserInput.PressedMouseButtonsCur > 0,
                MouseCondition.WhileUp => () => UserInput.PressedMouseButtonsCur == 0,
                MouseCondition.OnClick => () => UserInput.PressedMouseButtonsCur > UserInput.PressedMouseButtonsPrev,
                MouseCondition.OnRelease => () =>
                {
                    if (UserInput.PressedMouseButtonsCur < UserInput.PressedMouseButtonsPrev)
                    {
                        return true;
                    }

                    if (UserInput.PressedMouseButtonsCur > UserInput.PressedMouseButtonsPrev)
                    {
                        _eventArgs = new(UserInput.VirtualMousePosCur);
                    }
                    return false;
                },
                _ => throw new ArgumentException(
                $"Invalid {nameof(MouseCondition)} value: \"{condition}\" ({(int)condition})")
            };

            return;
        }

        ConditionFunc = condition switch
        {
            MouseCondition.WhileDown => () => IsButtonPressed(UserInput.MouseStateCur, button),
            MouseCondition.WhileUp => () => !IsButtonPressed(UserInput.MouseStateCur, button),

            MouseCondition.OnClick => () =>
            {
                return IsButtonPressed(UserInput.MouseStateCur, button)
                    && !IsButtonPressed(UserInput.MouseStatePrev, button);
            },

            MouseCondition.OnRelease => () => 
            {
                bool PressedNow = IsButtonPressed(UserInput.MouseStateCur, button);
                bool PressedPrev = IsButtonPressed(UserInput.MouseStatePrev, button);

                if (!PressedNow && PressedPrev) return true;

                if (!PressedPrev && PressedNow)
                {
                    _eventArgs = new(UserInput.VirtualMousePosCur);
                }
                return false;
            }
            , 

            _ => throw new ArgumentException(
                $"Invalid {nameof(MouseCondition)} value: \"{condition}\" ({(int)condition})")
        };
    }

    public MouseListener(object? creator, 
        GameFrame? parentFrame,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true) : base(creator, parentFrame, requiresFocus)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        ConditionFunc = () => UserInput.MouseStateCur.Position != UserInput.MouseStatePrev.Position;
    }

    public MouseListener(object? creator,
        GameFrame? parentFrame,
        ScrollDirection scrollDirection,
        EventHandler<MouseEventArgs> handler,
        bool requiresFocus = true) : base(creator, parentFrame, requiresFocus)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        ConditionFunc = scrollDirection switch
        {
            ScrollDirection.Stationary => () => UserInput.MouseStateCur.ScrollWheelValue == UserInput.MouseStatePrev.ScrollWheelValue,
            ScrollDirection.Any => () => UserInput.MouseStateCur.ScrollWheelValue != UserInput.MouseStatePrev.ScrollWheelValue,
            ScrollDirection.Up => () => UserInput.MouseStateCur.ScrollWheelValue > UserInput.MouseStatePrev.ScrollWheelValue,
            ScrollDirection.Down => () => UserInput.MouseStateCur.ScrollWheelValue < UserInput.MouseStatePrev.ScrollWheelValue,
            _ => throw new ArgumentException(
                $"Invalid {nameof(ScrollDirection)} value: \"{scrollDirection}\" ({(int)scrollDirection})")
        };
    }


    // Private static methods.
    private static bool IsButtonPressed(in MouseState mouseState, MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => mouseState.LeftButton == ButtonState.Pressed,
            MouseButton.Middle => mouseState.MiddleButton == ButtonState.Pressed,
            MouseButton.Right => mouseState.RightButton == ButtonState.Pressed,
            _ => throw new ArgumentException($"Invalid button \"{button}\" ({(int)button})")
        };
    }


    // Inherited methods.
    internal override void Listen()
    {
        if (ConditionFunc.Invoke())
        {
            _handler.Invoke(Creator, _eventArgs);
        }
    }
}