using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.UI.Buttons.Connector;

internal class ConnectorToggle : ConnectorElement
{
    // Internal fields.
    internal bool Value
    {
        get => _value;
        set => _value = value;
    }

    internal override Vector2 Scale
    {
        get => base.Scale;
        set
        {
            base.Scale = value;
            _panel.Scale.Vector = (s_size / _panel.TextureSize) * value;
            _glow.Scale.Vector = (s_size / _panel.TextureSize) * value;
            _centerGlow.Scale.Vector = value;
            _connector.Scale.Vector = value;
            _receiver.Scale.Vector = value;
            _receiverLights.Scale.Vector = value;

            _button.Scale = value;
            _maxBounceDistance = value * MAX_BOUNCE_DISTANCE * (-_directionUnit);

            _maxReceiverDistance = MAX_RECEIVER_DISTANCE * (_directionUnit.Y == 0f ? Scale.X : Scale.Y);
        }
    }

    internal float ConnectionStrength
    {
        get => _connectionStrength;
        set
        {
            _connectionStrength = Math.Clamp(value, 0f, 1f);
        }
    }


    // Private static fields.
    private readonly static Vector2 s_size = new(200f, 200f);
    private readonly static Vector2 s_halfSize = s_size / 2f;
    private static readonly FloatColor s_toggledColor = new(245, 51, 167, 255);


    // Private fields.
    private bool _value;

    private readonly Vector2 _directionUnit;

    private SpriteItem _panel = new(SquarePanelInstance!);
    private SpriteItem _glow = new(SquareGlowInstance!);
    private SpriteItem _centerGlow = new(ToggleLightInstance!);
    private SpriteItem _connector = new(ConnectorInstance!);
    private SpriteItem _receiver = new(ReceiverInstance!);
    private SpriteItem _receiverLights = new(ReceiverLightsInstance!);

    private Button _button = new(new RectangleButtonComponent(s_size));

    private const float MAX_RECEIVER_DISTANCE = 250f;
    private float _maxReceiverDistance;
    private float _connectionStrength;
    private const float MAX_FALSE_CONNECTION_STRENGTH =  0.7f;
    private const float CONNECTION_STRENGTH_CHANGE =  6f;
    private bool _increaseStrength = false;

    private const float MAX_BOUNCE_DISTANCE = 35f;
    private const float BOUNCE_SPEED = 5f * MathF.PI;
    private Vector2 _maxBounceDistance;
    private float _bouncePeriod = MathF.PI;

    private float _colorStrength = 0f;
    private const float COLOR_CHANGE_SPEED = 4f;


    // Constructors.
    internal ConnectorToggle(Direction connectDirection)
    {
        int a = 5;
        _directionUnit = GetDirectionVector(connectDirection);
        float ConnectorAngle = GetConnectorAngle(connectDirection);
        _connector.Rotation = ConnectorAngle;
        _receiver.Rotation = ConnectorAngle;
        _receiverLights.Rotation = ConnectorAngle;

        Scale = Vector2.One;
        _button.SetEventHandler(ButtonEvent.Hovering, OnButtonHovering);
        _button.SetEventHandler(ButtonEvent.NotHovering, OnButtonNotHovering);
        _button.SetEventHandler(ButtonEvent.LeftClick, OnButtonClickEvent);
    }


    // Private methods.
    private void OnButtonHovering(object? sender, EventArgs args)
    {
        _increaseStrength = true;
    }

    private void OnButtonNotHovering(object? sender, EventArgs args)
    {
        _increaseStrength = Value;
    }

    private void OnButtonClickEvent(object? sender, EventArgs args)
    {
        Value = !Value;
    }

    private void UpdateConnectionStrength()
    {
        float Change = GameFrameManager.PassedTimeSeconds * CONNECTION_STRENGTH_CHANGE;

        if (_increaseStrength)
        {
            if (Value)
            {
                float PrevStrength = ConnectionStrength;
                ConnectionStrength += Change;

                if (ConnectionStrength == 1f && ConnectionStrength != PrevStrength)
                {
                    _bouncePeriod = 0f;
                }
                return;
            }

            if (ConnectionStrength > MAX_FALSE_CONNECTION_STRENGTH)
            {
                ConnectionStrength= Math.Max(ConnectionStrength -= Change, MAX_FALSE_CONNECTION_STRENGTH);
                return;
            }

            ConnectionStrength = Math.Min(ConnectionStrength + Change, MAX_FALSE_CONNECTION_STRENGTH);
            return;
        }

        ConnectionStrength -= Change;
    }

    private void UpdatePositions()
    {
        Position.Update();

        Vector2 VisualPosition = Position + (_maxBounceDistance * MathF.Sin(_bouncePeriod));
        _bouncePeriod = Math.Min(_bouncePeriod + GameFrameManager.PassedTimeSeconds * BOUNCE_SPEED, MathF.PI);

        _panel.Position.Vector = VisualPosition;
        _glow.Position.Vector = VisualPosition;
        _centerGlow.Position.Vector = VisualPosition;
        _button.Position = VisualPosition;

        Vector2 ConnectorPosition = VisualPosition + (s_halfSize * _directionUnit * Scale);
        _connector.Position.Vector = ConnectorPosition;
        Vector2 ReceiverPosition = ConnectorPosition + (_directionUnit * (1 - ConnectionStrength) * _maxReceiverDistance);
        _receiver.Position.Vector = ReceiverPosition;
        _receiverLights.Position.Vector = ReceiverPosition;
    }

    private void UpdateColor()
    {
        if (ConnectionStrength == 1f)
        {
            _colorStrength = 1f;
        }
        else
        {
            _colorStrength = Math.Max(_colorStrength - GameFrameManager.PassedTimeSeconds * COLOR_CHANGE_SPEED, 0f);
        }

        Color ColorMask = FloatColor.InterpolateRGB(UNSELECTED_COLOR, s_toggledColor, _colorStrength);
        _glow.Mask = ColorMask;
        _centerGlow.Mask = ColorMask;
        _receiverLights.Mask = ColorMask;
    }


    // Inherited methods.
    public override void Draw()
    {
        _panel.Draw();
        _glow.Draw();
        _centerGlow.Draw();
        _connector.Draw();
        _receiverLights.Draw();
        _receiver.Draw();
    }

    public override void Update()
    {
        UpdatePositions();
        _button.Update();
        UpdateConnectionStrength();
        UpdateColor();

        _receiver.Opacity = ConnectionStrength* (1f / MAX_FALSE_CONNECTION_STRENGTH);
        _receiverLights.Opacity = ConnectionStrength* (1f  / MAX_FALSE_CONNECTION_STRENGTH);
    }
}