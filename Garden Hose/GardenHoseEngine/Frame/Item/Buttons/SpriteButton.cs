using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;

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
    private readonly Dictionary<ButtonEvent, ButtonListener> _handlers = new();
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
                or ButtonEvent.RightRelease => GetInputListenerLeftMiddleRight(buttonEvent),

                ButtonEvent.Scroll
                or ButtonEvent.ScrollDown
                or ButtonEvent.ScrollUp => GetInputListenerScroll(buttonEvent),

                _ => throw new EnumValueException(nameof(buttonEvent), buttonEvent)
            };

            _handlers.Add(buttonEvent, new(Listener, handler));
            return;
        }

        _handlers.Add(buttonEvent, new(null, handler));
    }

    public void RemoveHandler(ButtonEvent buttonEvent)
    {
        _handlers.Remove(buttonEvent);
    }

    public void ClearHandlers()
    {
        _handlers.Clear();
    }


    // Private methods.
    private IInputListener GetInputListenerLeftMiddleRight(ButtonEvent buttonEvent)
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

        return IInputListener.CreateSingleMouseButton(true, Condition, Button);
    }

    private IInputListener GetInputListenerScroll(ButtonEvent buttonEvent)
    {
        ScrollDirection Direction = buttonEvent switch
        {
            ButtonEvent.ScrollUp => ScrollDirection.Up,
            ButtonEvent.ScrollDown => ScrollDirection.Down,
            ButtonEvent.Scroll => ScrollDirection.Any
        };

        return IInputListener.CreateMouseScroll(true, Direction);
    }


    // Inherited methods.
    public void Update(IProgramTime time)
    {
        foreach (ButtonListener BHandler in _handlers.Values)
        {
            if ((BHandler.InputListener?.Listen() ?? false) && IsMouseOverButton)
            {
                BHandler.Handler.Invoke(this, EventArgs.Empty);
            }
        }

        bool IsCurrentMouseOverButton = IsMouseOverButton;
        ButtonListener Listener;

        if (IsCurrentMouseOverButton)
        {
            if (!_wasMouseOverButton && _handlers.TryGetValue(ButtonEvent.Hover, out Listener))
            {
                Listener.Handler.Invoke(this, EventArgs.Empty);
            }
            if (_handlers.TryGetValue(ButtonEvent.Hovering, out Listener))
            {
                Listener.Handler.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            if (_wasMouseOverButton && _handlers.TryGetValue(ButtonEvent.Unhover, out Listener))
            {
                Listener.Handler.Invoke(this, EventArgs.Empty);
            }
            if (_handlers.TryGetValue(ButtonEvent.NotHovering, out Listener))
            {
                Listener.Handler.Invoke(this, EventArgs.Empty);
            }
        }

        _wasMouseOverButton = IsCurrentMouseOverButton;
    }
}