using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using GardenHoseEngine.Frame.Item.Text;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.UI.Buttons.Connector;


internal partial class ConnectorRectangleButton : ConnectorElement
{
    // Internal static fields.
    internal static readonly Vector2 SQUARE_BUTTON_SIZE = new(200f, 200f);
    internal static readonly Vector2 NARROW_BUTTON_SIZE = new(400f, 200f);
    internal static readonly Vector2 NORMAL_BUTTON_SIZE = new(800f, 200f);
    internal static readonly Vector2 WIDE_BUTTON_SIZE = new(1600f, 200f);
    internal static readonly Vector2 PANEL_GLOW_SIZE_DIFFERENCE = new(8f, 8f);


    // Internal fields.
    internal bool IsClickable { get; set; } = true;

    internal bool IsClickingResetOnClick { get; set; } = true;

    internal override bool IsFunctional
    {
        get => base.IsFunctional;
        set
        {
            base.IsFunctional = value;
            SetButtonHandlers();
        }
    }

    internal override float Scale
    {
        get => base.Scale;
        set
        {
            base.Scale =  value;

            _panel.Size = (Size - PANEL_GLOW_SIZE_DIFFERENCE) * Scale;
            _glowButton.Size = Size * Scale;
            _connector.Size = CONNECTOR_SIZE * Scale;
            _receiver.Scale = value;
            _text.Scale = value;
            _maxReceiverDistance = DEFAULT_MAX_RECEIVER_DISTANCE * Scale;
            _maxBounceDistance = DEFAULT_MAX_BOUNCE_DISTANCE * Scale;
        }
    }

    internal string Text
    {
        get => _text.Text;
        set => _text.Text = value;
    }

    internal override Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            SetItemPositions(value);
        }
    }

    internal Vector2 Size { get; private init; }
    internal FloatColor ClickedColor { get; set; } = DEFAULT_CLICKED_COLOR;
    internal EventHandler? ClickHandler { get; set; }


    // Private fields.
    /* Constants. */
    private const float MIN_POWER = 0f;
    private const float MAX_POWER = 1f;
    private const float MIN_CONNECTION = 0f;
    private const float MAX_CONNECTION = 1f;
    private const float MIN_CLICK_POWER = 0f;
    private const float MAX_CLICK_POWER = 1f;


    private const float CLICK_POWER_CHANGE_SPEED = 2.5f;
    private const float CONNECTION_SPEED_UP = 12f;
    private const float CONNECTION_SPEED_DOWN = 4f;
    private const float POWER_CHANGE_SPEED = 4f;

    private const float DEFAULT_MAX_RECEIVER_DISTANCE = 200f;
    private const float DEFAULT_MAX_BOUNCE_DISTANCE = 40f;

    private const float MIN_BOUNCE_PERIOD = 0f;
    private const float MAX_BOUNCE_PERIOD = MathF.PI;
    private const float BOUNCE_SPEED = 5f * MathF.PI;

    private const float TEXT_SCALE = 2f;



    /* Properties. */
    private readonly SpriteButton _glowButton;
    private readonly SpriteItem _panel;
    private readonly SpriteItem _connector;
    private readonly ConnectorReceiver _receiver;

    private bool _isHovered = false;
    private float _power = 0f;
    private float _connection = 0f;
    private float _clickPower = 0f;

    private readonly Vector2 _directionUnit;
    private float _maxReceiverDistance = DEFAULT_MAX_RECEIVER_DISTANCE;

    private FloatColor _lightColor;

    private float _bouncePeriod = MathF.PI;
    private float _maxBounceDistance = DEFAULT_MAX_BOUNCE_DISTANCE;

    private readonly FittedText _text;


    // Constructors.
    internal ConnectorRectangleButton(Direction connectDirection, RectangleButtonType type, ConnectorAssetCollection assets)
    {
        switch (type)
        {
            case RectangleButtonType.Square:
                Size = SQUARE_BUTTON_SIZE;
                _panel = new(assets.SquarePanel, Size - PANEL_GLOW_SIZE_DIFFERENCE);
                _glowButton = new(assets.SquareGlow, Size);
                break;

            case RectangleButtonType.Narrow:
                Size = NARROW_BUTTON_SIZE;
                _panel = new(assets.NarrowPanel, Size - PANEL_GLOW_SIZE_DIFFERENCE);
                _glowButton = new(assets.NarrowGlow, Size);
                break;

            case RectangleButtonType.Normal:
                Size = NORMAL_BUTTON_SIZE;
                _panel = new(assets.NormalPanel, Size - PANEL_GLOW_SIZE_DIFFERENCE);
                _glowButton = new(assets.NormalGlow, Size);
                break;

            case RectangleButtonType.Wide:
                Size = WIDE_BUTTON_SIZE;
                _panel = new(assets.WidePanel, Size - PANEL_GLOW_SIZE_DIFFERENCE);
                _glowButton = new(assets.WideGlow, Size);
                break;

            default:
                throw new EnumValueException(nameof(type), type);
        }
        _receiver = new(connectDirection, assets);
        _connector = new(assets.Connector, CONNECTOR_SIZE) { Rotation = GetConnectorAngle(connectDirection) };

        _text = new(string.Empty, assets.TextFont)
        {
            TextOrigin = Origin.Center,
            IsShadowEnabled = true,
            ShadowColor = TEXT_SHADOW_COLOR,
            ShadowOffset = TEXT_SHADOW_OFFSET,
            FittingSizePixels = Size.X - PANEL_GLOW_SIZE_DIFFERENCE.X,
            Scale = TEXT_SCALE
        };
        
        _directionUnit = GetDirectionVector(connectDirection);

        IsFunctional = true;
    }

    // Private methods.
    /* Event handlers. */
    private void OnButtonHoverEvent(object? sender, EventArgs args) => _isHovered = true;

    private void OnButtonUnHoverEvent(object? sender, EventArgs args) => _isHovered = false;

    private void OnButtonClickEvent(object? sender, EventArgs args)
    {
        if (!IsClickable) return;

        ClickHandler?.Invoke(this, EventArgs.Empty);
        _clickPower = MAX_CLICK_POWER;

        if (IsClickingResetOnClick)
        {
            IsClickable = false;
        }
    }


    /* Other */
    private void SetButtonHandlers()
    {
        if (IsFunctional)
        {
            _glowButton.SetHandler(ButtonEvent.Hovering, OnButtonHoverEvent);
            _glowButton.SetHandler(ButtonEvent.NotHovering, OnButtonUnHoverEvent);
            _glowButton.SetHandler(ButtonEvent.LeftClick, OnButtonClickEvent);
        }
        else
        {
            _glowButton.ClearHandlers();
        }
    }

    private void UpdateReceiver()
    {
        _receiver.Position = Position + (Size * 0.5f * _directionUnit) + (_maxReceiverDistance * (MAX_CONNECTION - _connection) * _directionUnit);
        _receiver.Opacity = _connection;
    }

    private void AnimateBounce(IProgramTime time)
    {
        _bouncePeriod = Math.Min(_bouncePeriod + BOUNCE_SPEED * time.PassedTimeSeconds, MAX_BOUNCE_PERIOD);
        Vector2 BouncedPosition = Position + (MathF.Sin(_bouncePeriod) * (-_directionUnit) * _maxBounceDistance);
        SetItemPositions(BouncedPosition);
    }

    private void SetItemPositions(Vector2 position)
    {
        _glowButton.Position = position;
        _panel.Position = position;
        _receiver.Position = position + (Size * 0.5f * _directionUnit);
    }


    // Inherited methods. 
    public override void Update(IProgramTime time)
    {
        if (!IsFunctional) return;

        _connection = Math.Clamp(_connection + (_isHovered ? CONNECTION_SPEED_UP : CONNECTION_SPEED_DOWN) * time.PassedTimeSeconds,
            MIN_CONNECTION, MAX_CONNECTION);

        if ((_connection == MAX_CONNECTION) && (_power != MAX_POWER))
        {
            _bouncePeriod = MIN_BOUNCE_PERIOD;
        }

        _power = _connection == MAX_CONNECTION ? MAX_POWER : Math.Max(_power - POWER_CHANGE_SPEED * time.PassedTimeSeconds, MIN_POWER);
        _clickPower = Math.Min(_clickPower - CLICK_POWER_CHANGE_SPEED * time.PassedTimeSeconds, MIN_CLICK_POWER);

        if (_clickPower == MIN_CLICK_POWER)
        {
            _lightColor = FloatColor.InterpolateRGB(UNSELECTED_COLOR, SELECTED_COLOR, _power);
        }
        else
        {
            _lightColor = FloatColor.InterpolateRGB(UNSELECTED_COLOR, FloatColor.InterpolateRGB(SELECTED_COLOR, ClickedColor, _clickPower), _power);
        }

        _glowButton.Mask = _lightColor;
        _receiver.LightColor = _lightColor;

        // Do not change order here, bounce alters position and thus alters the receiver's position when updated in UpdateReceiver.
        if (_bouncePeriod <= MAX_BOUNCE_PERIOD)
        {
            AnimateBounce(time);
        }

        UpdateReceiver();
    }



    public override void Draw(IDrawInfo info)
    {
        if (!IsVisible) return;

        _panel.Draw(info);
        _connector.Draw(info);
        _glowButton.Draw(info);
        _receiver.Draw(info);
        _text.Draw(info);
    }
}