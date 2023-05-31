using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using GardenHose.Engine.Frame;

namespace GardenHose.Engine.IO;


public delegate void MouseEventHandler(MouseEventInfo eventInfo);


public enum MouseClickCondition
{
    WhileDown,
    WhileUp,
    OnClick,
    OnRelease
}

public enum MouseScrollCondition
{
    OnScrollUp,
    OnScrollDown,
}

public enum MouseMoveCondition
{
    Moving
}

public enum MouseButton
{
    Left,
    Right,
    Middle,
    Any
}

public struct MouseEventInfo
{
    public Point StartLocation;
    public Point EndLocation;
}


public class MouseEventListener : IInputEventListener
{
    // Types.
    public delegate bool ConditionMethod();


    // Static fields.
    public static MouseState StateCur = Mouse.GetState();
    public static MouseState StatePrev = Mouse.GetState();

    public static Vector2 VirtualPositionCur;
    public static Vector2 VirtualPositionPrev;


    // Private static fields.
    private static HashSet<MouseEventListener> s_listeners = new();
    

    // Fields.
    public readonly MouseEventHandler Handler;

    public ConditionMethod Condition;
    public readonly bool WindowMustBeFocused;


    // Private fields.
    private readonly MouseButton _requiredButton = MouseButton.Any;
    private MouseEventInfo _eventInfo = new();


    // Constructors.
    private MouseEventListener(MouseEventHandler handler, bool windowMustBeFocused)
    {
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        WindowMustBeFocused = windowMustBeFocused;
    }

    private MouseEventListener(
        MouseEventHandler handler,
        bool windowMustBeFocused,
        MouseClickCondition condition,
        MouseButton button) : this(handler, windowMustBeFocused)
    {
        if (button == MouseButton.Any)
        {
            Condition = condition switch
            {
                MouseClickCondition.OnClick => WasAnyButtonClicked,
                MouseClickCondition.OnRelease => WasAnyButtonReleased,
                MouseClickCondition.WhileDown => IsAnyButtonDown,
                MouseClickCondition.WhileUp => AreAllButtonsUp,
                _ => IsAnyButtonDown
            };
        }
        else
        {
            _requiredButton = button;
            Condition = condition switch
            {
                MouseClickCondition.OnClick => WasButtonClicked,
                MouseClickCondition.OnRelease => WasButtonReleased,
                MouseClickCondition.WhileDown => IsButtonDown,
                MouseClickCondition.WhileUp => IsButtonUp,
                _ => IsAnyButtonDown
            };
        }
    }

    private MouseEventListener(
        MouseEventHandler handler,
        bool windowMustBeFocused,
        MouseScrollCondition condition) : this(handler, windowMustBeFocused)
    {
        Condition = condition switch
        {
            MouseScrollCondition.OnScrollUp => WasScrolledUp,
            MouseScrollCondition.OnScrollDown => WasScrolledDown,
            _ => WasScrolledUp
        };
    }

    private MouseEventListener(
        MouseEventHandler handler,
        bool windowMustBeFocused,
        MouseMoveCondition condition) : this(handler, windowMustBeFocused)
    {
        Condition = condition switch
        {
            _ => IsMoving
        };
    }


    // Static methods.
    public static void ListenForInput()
    {
        // Update fields.
        StatePrev = StateCur;
        StateCur = Mouse.GetState();

        Point Position = StateCur.Position;
        VirtualPositionCur.X = (Position.X - DisplayInfo.XOffset) / DisplayInfo.XScale;
        VirtualPositionCur.Y = (Position.Y - DisplayInfo.YOffset) / DisplayInfo.YScale;

        bool WindowIsNotActive = !MainGame.Instance.IsActive;


        // Listen for inputs.
        foreach (MouseEventListener Listener in s_listeners)
        {
            if (Listener.WindowMustBeFocused && WindowIsNotActive) continue;
            else if (Listener.Condition.Invoke())
            {
                Listener._eventInfo.EndLocation = StateCur.Position;
                Listener.Handler.Invoke(Listener._eventInfo);
            }
        }
    }

    public static MouseEventListener AddClickListener(
        MouseEventHandler handler,
        bool windowMustBeFocused,
        MouseClickCondition condition,
        MouseButton button)
    {
        MouseEventListener Listener = new MouseEventListener(handler, windowMustBeFocused, condition, button);
        s_listeners.Add(Listener);
        return Listener;
    }

    public static MouseEventListener AddScrollListener(
        MouseEventHandler handler,
        bool windowMustBeFocused,
        MouseScrollCondition condition)
    {
        MouseEventListener Listener = new MouseEventListener(handler, windowMustBeFocused, condition);
        s_listeners.Add(Listener);
        return Listener;
    }

    public static MouseEventListener AddMoveListener(
        MouseEventHandler handler,
        bool windowMustBeFocused,
        MouseMoveCondition condition)
    {
        MouseEventListener Listener = new MouseEventListener(handler, windowMustBeFocused, condition);
        s_listeners.Add(Listener);
        return Listener;
    }

    public static void RemoveListener(MouseEventListener listener) => s_listeners.Remove(listener);

    public static void ClearListeners() => s_listeners.Clear();


    // Private methods.

    /* Buttons */
    private bool IsAnyButtonDown()
    {
        return StateCur.LeftButton == ButtonState.Pressed
            || StateCur.MiddleButton == ButtonState.Pressed
            || StateCur.MiddleButton == ButtonState.Pressed;
    }

    private bool AreAllButtonsUp()
    {
        return StateCur.LeftButton == ButtonState.Released
            && StateCur.MiddleButton == ButtonState.Released
            && StateCur.MiddleButton == ButtonState.Released;
    }

    private bool WasAnyButtonClicked()
    {
        byte PressedCountPrev = 0;
        if (StatePrev.LeftButton == ButtonState.Pressed) PressedCountPrev++;
        if (StatePrev.MiddleButton == ButtonState.Pressed) PressedCountPrev++;
        if (StatePrev.RightButton == ButtonState.Pressed) PressedCountPrev++;

        byte PressedCountCur = 0;
        if (StateCur.LeftButton == ButtonState.Pressed) PressedCountCur++;
        if (StateCur.MiddleButton == ButtonState.Pressed) PressedCountCur++;
        if (StateCur.RightButton == ButtonState.Pressed) PressedCountCur++;

        return PressedCountCur > PressedCountPrev;
    }

    private bool WasAnyButtonReleased()
    {
        byte PressedCountPrev = 0;
        if (StatePrev.LeftButton == ButtonState.Pressed) PressedCountPrev++;
        if (StatePrev.MiddleButton == ButtonState.Pressed) PressedCountPrev++;
        if (StatePrev.RightButton == ButtonState.Pressed) PressedCountPrev++;

        byte PressedCountCur = 0;
        if (StateCur.LeftButton == ButtonState.Pressed) PressedCountCur++;
        if (StateCur.MiddleButton == ButtonState.Pressed) PressedCountCur++;
        if (StateCur.RightButton == ButtonState.Pressed) PressedCountCur++;

        if (PressedCountCur > PressedCountPrev)
        {
            _eventInfo.StartLocation = StateCur.Position;
            return false;
        }
        else return PressedCountCur < PressedCountPrev;
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

    private bool IsButtonDown() => GetButtonState(StateCur) == ButtonState.Pressed;

    private bool IsButtonUp() => GetButtonState(StateCur) == ButtonState.Released;

    private bool WasButtonClicked()
    {
        return GetButtonState(StatePrev) == ButtonState.Released
            && GetButtonState(StateCur) == ButtonState.Pressed;
    }

    private bool WasButtonReleased()
    {
        if (GetButtonState(StatePrev) == ButtonState.Released
            && GetButtonState(StateCur) == ButtonState.Pressed)
        {
            _eventInfo.StartLocation = StateCur.Position;
            return false;
        }
        else return GetButtonState(StatePrev) == ButtonState.Pressed
            && GetButtonState(StateCur) == ButtonState.Released;
    }


    /* Scrolling */
    private bool WasScrolledUp() => StateCur.ScrollWheelValue > StatePrev.ScrollWheelValue;

    private bool WasScrolledDown() => StateCur.ScrollWheelValue < StatePrev.ScrollWheelValue;


    /* Moving */
    private bool IsMoving() => StateCur.X != StatePrev.X && StateCur.Y != StatePrev.Y;


    // Inherited methods.
    public void StopListening() => RemoveListener(this);
}