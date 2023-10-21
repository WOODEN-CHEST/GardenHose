using GardenHose.Frames.Global;
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

    // Internal fields.
    internal bool IsClickable { get; set; } = true;

    internal bool IsClickingResetOnClick { get; set; } = true;


    internal override bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            if (value == base.IsEnabled) return;

            base.IsEnabled = value;
            if (IsEnabled)
            {
                IsClickable = true;
                CreateButtonHandlers();
            }
            else
            {
                _button.ClearEventHandlers();
            }
        }
    }

    internal FloatColor ClickedColor { get; set; } = DefaultClickedColor;

    internal override Vector2 Scale
    {
        get => base.Scale;
        set
        {
            base.Scale =  value;

            _panel.Scale.Vector = (Size / _panel.TextureSize) * value;
            _glow.Scale.Vector = (Size / _glow.TextureSize) * value;
            _connector.Scale.Vector = value;
            _receiver.Scale.Vector = value;
            _receiverLights.Scale.Vector =  value;

            float TextScale = value.X * TEXT_SCALE;
            float TextLength = _text.RealPixelSize.X * TextScale;
            float MaxTextLength = (Size.X - TEXT_PADDING_X) * Scale.X;
            if (TextLength > MaxTextLength)
            {
                float NewScale = MaxTextLength / TextLength;
                _text.Scale = new Vector2(NewScale) * TextScale;
            }
            else
            {
                _text.Scale = value * TEXT_SCALE;
            }

            _button.Scale = value;

            _maxReceiverDistance = MAX_RECEIVER_DISTANCE * (_directionUnit.Y == 0f ? Scale.X : Scale.Y);
            _maxBounceDistance = value * MAX_BOUNCE_DISTANCE * (-_directionUnit);

            SyncAllPositions();
        }
    }

    internal Vector2 Size { get; private init; }

    internal float ConnectionStrength
    {
        get => _connectionStrengthDelta.Current;
        set
        {
            _connectionStrengthDelta.Update(Math.Clamp(value, MIN_CONNECTION_STRENGTH, MAX_CONNECTION_STRENGTH));
            SyncConnectionProperties();
        }
    }

    internal string Text
    {
        get => _text.Text;
        set
        {
            _text.Text = value;
            Scale = base.Scale;
        }
    }

    internal EventHandler? ClickHandler { get; set; }


    // Private fields.
    /* Button properties */
    private readonly Vector2 _halfSize;
    private readonly Vector2 _directionUnit;
    
    /* Visual components. */
    private SpriteItem _panel;
    private SpriteItem _connector;
    private readonly SpriteItem _receiver;
    private readonly SpriteItem _receiverLights;
    private SpriteItem _glow;

    /* Glow. */
    private FloatColor _glowColor;
    private float ClickColorAmount
    {
        get => _clickColorAmount;
        set => _clickColorAmount = Math.Clamp(value, 0f, 1f);
    }
    private float _clickColorAmount = 0f;
    private const float CLICK_COLOR_FADE_SPEED = 2.5f;
    private const float COLOR_CHANGE_SPEED_UP = 12f;
    private const float COLOR_CHANGE_SPEED_DOWN = 4f;

    /* Functional button object. */
    private readonly Button _button;
    private bool _isButtonHovered = false;

    /* Receiver */
    private DeltaValue<float> _connectionStrengthDelta = new(0f);
    private const float MAX_CONNECTION_STRENGTH = 1f;
    private const float MIN_CONNECTION_STRENGTH = 0f;
    private float _maxReceiverDistance = MAX_RECEIVER_DISTANCE;
    private const float MAX_RECEIVER_DISTANCE = 200f;

    /* Bounce animation. */
    private const float MAX_BOUNCE_DISTANCE = 35f;
    private const float BOUNCE_SPEED = 5f * MathF.PI;
    private float _bouncePeriod = MathF.PI;
    private Vector2 _maxBounceDistance;
    


    /* Text. */
    private readonly SimpleTextBox _text;
    private const float TEXT_SCALE = 2f;
    private const float TEXT_PADDING_X = 40f;
    private const float TEXT_PADDING_Y = 10f;


    // Constructors.
    internal ConnectorRectangleButton(Direction connectDirection,
        SpriteItem panel,
        SpriteItem glow,
        Vector2 position,
        Vector2 scale,
        Vector2 size)
    {
        // Create components.
        _panel = panel;
        _glow = glow;
        _connector = new(ConnectorInstance!);
        _receiver = new(ReceiverInstance!);
        _receiverLights = new(ReceiverLightsInstance!);

        _text = new(GlobalFrame.GeEichFontLarge, string.Empty)
        {
            Origin = Origin.Center,
            IsShadowEnabled = true,
            ShadowColor = TextShadowColor,
            ShadowOffset = TextShadowOffset
        };
        

        // Assign readonly properties.
        _directionUnit = GetDirectionVector(connectDirection);

        Size = size;
        _halfSize = Size / 2f;

        _button = new(new RectangleButtonComponent(Size));
        CreateButtonHandlers();

        float Rotation = GetConnectorAngle(connectDirection);

        _connector.Rotation = Rotation;
        _receiver.Rotation = Rotation;
        _receiverLights.Rotation = Rotation;

        Position.Vector = position;
        Scale = scale;
    }

    // Private methods.
    /* Event handlers. */
    private void OnButtonHoverEvent(object? sender, EventArgs args)
    {
        _isButtonHovered = true;
    }

    private void OnButtonUnHoverEvent(object? sender, EventArgs args)
    {
        _isButtonHovered = false;
    }

    private void OnButtonClickEvent(object? sender, EventArgs args)
    {
        if (!IsClickable) return;

        ClickHandler?.Invoke(this, EventArgs.Empty);
        ClickColorAmount = 1f;

        if (IsClickingResetOnClick)
        {
            IsClickable = false;
        }
    }


    /* Button components. */
    private void SyncAllPositions()
    {
        // Bounce.
        Vector2 BounceDistance = MathF.Sin(_bouncePeriod) * _maxBounceDistance;
        Vector2 VisualPosition = Position + BounceDistance;

        // Main positions.
        _panel.Position.Vector = VisualPosition;
        _glow.Position.Vector = VisualPosition;
        _button.Position = Position;
        _text.Position.Vector = VisualPosition;
        _text.Position.Vector.Y += TEXT_PADDING_Y * Scale.Y;

        // Connector and receiver positions.
        Vector2 ConnectorPosition = VisualPosition + (_halfSize * _directionUnit) * Scale;
        Vector2 ReceiverPosition = ConnectorPosition + (_directionUnit * (1f - ConnectionStrength) * _maxReceiverDistance);

        _connector.Position.Vector = ConnectorPosition;
        _receiver.Position.Vector = ReceiverPosition;
        _receiverLights.Position.Vector = ReceiverPosition;

        
    }

    private void SyncConnectionProperties()
    {
        _glowColor = UnselectedColor;

        if (_connectionStrengthDelta.Current < _connectionStrengthDelta.Previous
            || _connectionStrengthDelta.Current == MAX_CONNECTION_STRENGTH)
        {
            _glowColor.R = (SelectedColor.R + (ClickedColor.R - SelectedColor.R) * _clickColorAmount);
            _glowColor = FloatColor.InterpolateRGB(UnselectedColor, 
                FloatColor.InterpolateRGB(SelectedColor, ClickedColor, _clickColorAmount), ConnectionStrength);
        }

        _glow.Mask = _glowColor;
        _receiverLights.Mask = (ConnectionStrength == MAX_CONNECTION_STRENGTH) ? _glowColor  : UnselectedColor;
        _text.Mask = _glowColor * 2.5f;

        _receiver.Opacity = ConnectionStrength;
        _receiverLights.Opacity = ConnectionStrength;

        if (_connectionStrengthDelta.Current == 1f && _connectionStrengthDelta.Previous != 1f)
        {
            _bouncePeriod = 0f;
        }
    }


    /* Other */
    private void CreateButtonHandlers()
    {
        _button.SetEventHandler(ButtonEvent.OnHover, OnButtonHoverEvent);
        _button.SetEventHandler(ButtonEvent.OnUnhover, OnButtonUnHoverEvent);
        _button.SetEventHandler(ButtonEvent.LeftClick, OnButtonClickEvent);
    }


    // Inherited methods. 
    public override void Update()
    {
        if (!IsEnabled) return;

        // Update items.
        Position.Update();
        SyncAllPositions();
        _button.Update();

        //  Click amount.
        ClickColorAmount -= GameFrameManager.PassedTimeSeconds * CLICK_COLOR_FADE_SPEED;

        // Bounce animation.
        _bouncePeriod = Math.Clamp(_bouncePeriod + (GameFrameManager.PassedTimeSeconds * BOUNCE_SPEED), 0f, MathF.PI);

        // Connection.
        if (_isButtonHovered)
        {
            ConnectionStrength += GameFrameManager.PassedTimeSeconds * COLOR_CHANGE_SPEED_UP;
        }
        else
        {
            ConnectionStrength -= GameFrameManager.PassedTimeSeconds * COLOR_CHANGE_SPEED_DOWN;
        }
    }

    public override void Draw()
    {
        if (!IsVisible) return;

        _panel.Draw();
        _glow.Draw();
        _connector.Draw();
        _receiverLights.Draw();
        _receiver.Draw();
        _text.Draw();
    }
}