using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace GardenHoseEngine.Frame.Item.Buttons;


public class SpriteButton : SpriteItem, ITimeUpdatable
{
    // Fields.
    public ButtonLocationTestType TestType { get; set; } = ButtonLocationTestType.Area;

    public bool IsMouseOverButton
    {
        get
        {
            if (TestType == ButtonLocationTestType.Area)
            {
                Vector2 Center = SpriteCenter;
                Vector2 HalfSize = Size * 0.5f;

                Vector2 VectorFromCenterToMouse = UserInput.VirtualMousePosition.Current - Center;
                Vector2 TransformedMousePosition = UserInput.VirtualMousePosition.Current - VectorFromCenterToMouse
                    + Vector2.Transform(VectorFromCenterToMouse, Matrix.CreateRotationZ(-Rotation));

                return (TransformedMousePosition.X >= Center.X - HalfSize.X)
                    && (TransformedMousePosition.X <= Center.X + HalfSize.X)
                    && (TransformedMousePosition.Y >= Center.Y - HalfSize.Y)
                    && (TransformedMousePosition.Y <= Center.Y + HalfSize.Y);
            }
            return Vector2.Distance(UserInput.VirtualMousePosition.Current, Position + (TextureSize * 0.5f)
                - ActiveAnimation.GetFrame().Origin) <= Math.Max(Size.X, Size.Y);
        }
    }



    // Private fields.
    private readonly Dictionary<ButtonEvent, ButtonHandler> _handlers = new();
    private bool _wasMouseOverButton = false;


    // Constructors.
    public SpriteButton(AnimationInstance animationInstance) : base(animationInstance) { }

    public SpriteButton(AnimationInstance animationInstance, Vector2 size) : base(animationInstance, size) { }




    // Methods.
    public void SetHandler(ButtonEvent buttonEvent, EventHandler handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        RemoveHandler(buttonEvent);

        if ((int)buttonEvent < (int)ButtonEvent.Hover)
        {
            IInputListener Listener = buttonEvent switch
            {
                ButtonEvent.LeftClick
                or ButtonEvent.MiddleClick
                or ButtonEvent.RightClick
                or ButtonEvent.LeftHold
                or ButtonEvent.MiddleHold
                or ButtonEvent.RightHold 
                or ButtonEvent.LeftRelease
                or ButtonEvent.MiddleRelease
                or ButtonEvent.RightRelease => GetInputListenerLeftMiddleRight(buttonEvent, handler),

                ButtonEvent.Scroll
                or ButtonEvent.ScrollDown
                or ButtonEvent.ScrollUp => GetInputListenerScroll(buttonEvent, handler),

                _ => throw new EnumValueException(nameof(buttonEvent), buttonEvent)
            };

            _handlers.Add(buttonEvent, new(Listener, handler));
            return;
        }

        _handlers.Add(buttonEvent, new(null, handler));
    }

    public void RemoveHandler(ButtonEvent buttonEvent)
    {
        if (_handlers.TryGetValue(buttonEvent, out ButtonHandler? Handler))
        {
            Handler!.StopListening();
            _handlers.Remove(buttonEvent);
        }
    }

    public void ClearHandlers(ButtonEvent buttonEvent)
    {
        foreach (ButtonHandler Handler in _handlers.Values)
        {
            Handler.StopListening();
        }
        _handlers.Clear();
    }


    // Private methods.
    private IInputListener GetInputListenerLeftMiddleRight(ButtonEvent buttonEvent, EventHandler handler)
    {
        MouseButton Button = ((int)buttonEvent % 3) switch
        {
            0 => MouseButton.Left,
            1 => MouseButton.Middle,
            2 => MouseButton.Right
        };

        MouseCondition Condition = ((int)buttonEvent / 3) switch
        {
            0 => MouseCondition.OnClick,
            1 => MouseCondition.OnRelease,
            2 => MouseCondition.WhileDown,
            _ => throw new EnumValueException(nameof(buttonEvent), buttonEvent)
        };

        return MouseListenerCreator.SingleButton(true, Condition, handler, Button);
    }

    private IInputListener GetInputListenerScroll(ButtonEvent buttonEvent, EventHandler handler)
    {
        ScrollDirection Direction = ((int)buttonEvent % 3) switch
        {
            0 => ScrollDirection.Up,
            1 => ScrollDirection.Down,
            2 => ScrollDirection.Any
        };

        return MouseListenerCreator.Scroll(true, Direction, handler);
    }

    // Inherited methods.
    public void Update(IProgramTime time)
    {
        bool IsCurrentMouseOverButton = IsMouseOverButton;
        ButtonHandler? Handler;

        if (IsCurrentMouseOverButton)
        {
            if (!_wasMouseOverButton && _handlers.TryGetValue(ButtonEvent.Hover, out Handler))
            {
                Handler?.InvokeEvent(this);
            }
            if (_handlers.TryGetValue(ButtonEvent.Hovering, out Handler))
            {
                Handler?.InvokeEvent(this);
            }
        }
        else
        {
            if (_wasMouseOverButton && _handlers.TryGetValue(ButtonEvent.Unhover, out Handler))
            {
                Handler?.InvokeEvent(this);
            }
            if (_handlers.TryGetValue(ButtonEvent.NotHovering, out Handler))
            {
                Handler?.InvokeEvent(this);
            }
        }


        _wasMouseOverButton = IsCurrentMouseOverButton;
    }
}