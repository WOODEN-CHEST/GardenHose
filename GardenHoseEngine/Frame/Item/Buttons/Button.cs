using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GardenHoseEngine.Frame.Item.Buttons;


public class Button : ITimeUpdatable
{
    // Fields.
    [MemberNotNull(nameof(_components))]
    public IButtonComponent[] Components
    {
        get => _components;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length == 0)
            {
                throw new ArgumentException("No button components in array.", nameof(value));
            }

            _components = value;
        }
    }

    public Vector2 Position { get; set; } = Vector2.Zero;

    public Vector2 Scale { get; set; } = Vector2.One;


    // Private fields.
    private IButtonComponent[] _components;
    private readonly Dictionary<ButtonEvent, (EventHandler Handler, IInputListener? Listener)> _eventHandlers = new();
    private DeltaValue<bool> _isHovered = new(false);
    private bool _shouldUpdate = false;


    // Constructors.
    public Button(params IButtonComponent[] components)
    {
        Components = components;
    }


    // Methods.
    public void SetEventHandler(ButtonEvent buttonEvent, EventHandler handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        if (_eventHandlers.ContainsKey(buttonEvent))
        {
            _eventHandlers[buttonEvent].Listener?.StopListening();
        }

        if (buttonEvent is ButtonEvent.Hovering or ButtonEvent.NotHovering
            or ButtonEvent.OnHover or ButtonEvent.OnUnhover)
        {
            _eventHandlers[buttonEvent] = (handler, null);
            UpdateShouldUpdated();
        }
        else
        {
            _eventHandlers[buttonEvent] = (handler, GetInputListener(buttonEvent, handler));
            UserInput.AddListener(_eventHandlers[buttonEvent].Listener!);
        }
    }

    public void RemoveEventHandler(ButtonEvent buttonEvent)
    {
        if (_eventHandlers.ContainsKey(buttonEvent))
        {
            _eventHandlers[buttonEvent].Listener?.StopListening();
        }
        _eventHandlers.Remove(buttonEvent);

        UpdateShouldUpdated();
    }

    public void ClearEventHandlers()
    {
        foreach (var HandlerAndListener in _eventHandlers.Values)
        {
            HandlerAndListener.Listener?.StopListening();
        }
        _eventHandlers.Clear();

        UpdateShouldUpdated();
    }

    public bool IsMouseOverButton() => IsMouseOverButton(UserInput.VirtualMousePosition.Current);

    public bool IsMouseOverButton(Vector2 locationToTest)
    {
        foreach (var Component in _components)
        {
            if (Component.IsLocationOverButton(locationToTest, Position, Scale))
            {
                return true;
            }
        }
        return false;
    }


    // Private methods.
    private IInputListener GetInputListener(ButtonEvent eventType, EventHandler handler)
    {
        return eventType switch
        {
            ButtonEvent.LeftClick
            or ButtonEvent.MiddleClick
            or ButtonEvent.RightClick
            or ButtonEvent.LeftHold
            or ButtonEvent.MiddleHold
            or ButtonEvent.RightHold => GetInputListenerSingleButton(eventType, handler),

            ButtonEvent.LeftRelease
            or ButtonEvent.MiddleRelease
            or ButtonEvent.RightRelease => GetInputListenerOnRelease(eventType, handler),

            ButtonEvent.Scroll
            or ButtonEvent.ScrollDown
            or ButtonEvent.ScrollUp => GetInputListenerScroll(eventType, handler),

            _ => throw new EnumValueException(nameof(eventType), nameof(ButtonEvent),
                eventType.ToString(), (int)eventType)
        };
    }

    private IInputListener GetInputListenerSingleButton(ButtonEvent eventType, EventHandler handler)
    {
        MouseButton MouseButton = ((int)eventType % 3) switch
        {
            0 => MouseButton.Left,
            1 => MouseButton.Middle,
            2 => MouseButton.Right,
        };

        MouseCondition Condition = ((int)eventType / 3) switch
        {
            0 => MouseCondition.OnClick,
            1 => MouseCondition.OnRelease,
            2 => MouseCondition.WhileDown
        };

        return MouseListenerCreator.SingleButton(this, true, Condition,
            (sender, args) =>
            {
                if (IsMouseOverButton())
                {
                    handler.Invoke(this, EventArgs.Empty);
                }
            },
            MouseButton);
    }

    private IInputListener GetInputListenerOnRelease(ButtonEvent eventType, EventHandler handler)
    {
        MouseButton MouseButton = ((int)eventType % 3) switch
        {
            0 => MouseButton.Left,
            1 => MouseButton.Middle,
            2 => MouseButton.Right,
        };

        return MouseListenerCreator.SingleButton(this, true, MouseCondition.OnRelease,
            (sender, args) =>
            {
                if (IsMouseOverButton() && IsMouseOverButton(args.StartPosition))
                {
                    handler.Invoke(this, EventArgs.Empty);
                }
            },
            MouseButton);
    }

    private IInputListener GetInputListenerScroll(ButtonEvent eventType, EventHandler handler)
    {
        ScrollDirection Direction = eventType switch
        {
            ButtonEvent.Scroll => ScrollDirection.Any,
            ButtonEvent.ScrollUp => ScrollDirection.Up,
            ButtonEvent.ScrollDown => ScrollDirection.Down
        };

        return MouseListenerCreator.Scroll(this, true, Direction,
            (sender, args) =>
            {
                if (IsMouseOverButton())
                {
                    handler.Invoke(this, EventArgs.Empty);
                }
            });
    }

    private void UpdateShouldUpdated()
    {
        _shouldUpdate = _eventHandlers.ContainsKey(ButtonEvent.OnHover)
            || _eventHandlers.ContainsKey(ButtonEvent.OnUnhover)
            || _eventHandlers.ContainsKey(ButtonEvent.Hovering)
            || _eventHandlers.ContainsKey(ButtonEvent.NotHovering);
    }


    // Inherited methods.
    public void Update()
    {
        if (!_shouldUpdate) return;

        _isHovered.Update(IsMouseOverButton());

        if (_isHovered.Current)
        {
            if (!_isHovered.Previous && _eventHandlers.ContainsKey(ButtonEvent.OnHover))
            {
                _eventHandlers[ButtonEvent.OnHover].Handler.Invoke(this, EventArgs.Empty);
            }

            if (_eventHandlers.ContainsKey(ButtonEvent.Hovering))
            {
                _eventHandlers[ButtonEvent.Hovering].Handler.Invoke(this, EventArgs.Empty);
            }
        }
        if (!_isHovered.Current)
        {
            if (_isHovered.Previous && _eventHandlers.ContainsKey(ButtonEvent.OnUnhover))
            {
                _eventHandlers[ButtonEvent.OnUnhover].Handler?.Invoke(this, EventArgs.Empty);
            }

            if (_eventHandlers.ContainsKey(ButtonEvent.NotHovering))
            {
                _eventHandlers[ButtonEvent.NotHovering].Handler.Invoke(this, EventArgs.Empty);
            }
        }
    }
}