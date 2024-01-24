using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.UI.Buttons.Connector;

internal class ConnectorSlider : ConnectorElement
{
    // Internal fields.
    internal float Value
    {
        get => _value;
        set
        {
            _value = Math.Clamp(value, MinValue, MaxValue);

            if (Step != 0f)
            {
                _value = MathF.Round(_value / Step) * _value;
            }
        }
    }

    internal float MinValue
    {
        get => _minValue;
        set
        {
            _minValue = value;

            if (_minValue > MaxValue)
            {
                _maxValue = _minValue;
            }
        }
    }

    internal float MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;

            if (_maxValue < MinValue)
            {
                _minValue = _maxValue;
            }
        }
    }

    internal float Step
    {
        get => _step;
        set
        {
            if (value < 0f)
            {
                throw new ArgumentException("Step cannot be negative.");
            }
        }
    }

    internal override Vector2 Scale
    { 
        get => base.Scale;
        set
        {
            base.Scale = value;

            _panel.Scale.Vector = (s_size / _panel.TextureSize);
            _glow.Scale.Vector = (s_size / _panel.TextureSize);
            _valuePanel.Scale.Vector = value;
            _valueGlow.Scale.Vector = value;
            _connector.Scale.Vector = value;
            _receiver.Scale.Vector = value;
            _receiverLights.Scale.Vector = value;

            _button.Scale = value;

            _valuePanelOffset = (s_halfSize + new Vector2(ADDITIONAL_VALUE_PANEL_OFFSET)
                + (_valueGlow.TextureSize / 2f)) * value * _valuePanelDirectionUnit;
            _pointerOffset = (s_halfSize + new Vector2(ADDITIONAL_POINTER_OFFSET)) * value * _pointerDirectionUnit;
        }
    }


    // Private static fields.
    private static readonly Vector2 s_size = new(800f, 100f);
    private static readonly Vector2 s_halfSize = s_size / 2f;


    // Private fields.
    private float _value = 0.5f;
    private float _minValue = 0f;
    private float _maxValue = 1f;
    private float _step = 0.2f;

    private readonly Vector2 _valuePanelDirectionUnit;
    private readonly Vector2 _pointerDirectionUnit;
    private readonly Vector2 _minSliderPosition;
    private readonly Vector2 _maxSliderPosition;

    private readonly SpriteItem _panel = new(SliderPanelInstance!);
    private readonly SpriteItem _glow = new(SliderGlowInstance!);
    private readonly SpriteItem _valuePanel = new(SquarePanelInstance!);
    private readonly SpriteItem _valueGlow = new(SquareGlowInstance!);
    private readonly SpriteItem _pointer = new(SliderPointerInstance!);
    private readonly SpriteItem _connector = new(ConnectorInstance!);
    private readonly SpriteItem _receiver = new(ReceiverInstance!);
    private readonly SpriteItem _receiverLights = new(ReceiverLightsInstance!);

    private readonly Button _button = new(new IButtonComponent[] { new RectangleButtonComponent(s_size) });

    private Vector2 _valuePanelOffset;
    private const float ADDITIONAL_VALUE_PANEL_OFFSET = 40f;
    private Vector2 _pointerOffset;
    private const float ADDITIONAL_POINTER_OFFSET = -15f;


    // Constructors.
    internal ConnectorSlider(Direction sliderDirection, float minValue, float maxValue, float value)
    {
        MinValue = minValue;
        MaxValue = maxValue;
        Value = value;

        _valuePanelDirectionUnit = -(GetDirectionVector(sliderDirection));
        float Rotation = GetConnectorAngle(sliderDirection);

        _panel.Rotation = Rotation;
        _glow.Rotation = Rotation;
        Rotation -= MathF.PI / 2f;

        _pointerDirectionUnit = Vector2.Transform(_valuePanelDirectionUnit, Matrix.CreateRotationZ(MathF.PI / 2f));
        _connector.Rotation = Rotation;
        _receiver.Rotation = Rotation;
        _receiverLights.Rotation = Rotation;

        Scale = Vector2.One;
    }


    // Private methods.
    private void UpdatePositions()
    {
        Position.Update();
        _panel.Position.Vector = Position;
        _glow.Position.Vector = Position;
        _valuePanel.Position.Vector = Position + _valuePanelOffset;
        _valueGlow.Position.Vector = Position + _valuePanelOffset;

        UpdatePointer();
        //_connector.Position.Vector = Position;  
        //_receiver.Position.Vector = Position;
        //_receiverLights.Position.Vector = Position; 
    }

    private void UpdatePointer()
    {
        float ValueAmount = (Value - MinValue) / (MaxValue - MinValue);
        if (float.IsInfinity(ValueAmount))
        {
            ValueAmount = 0f;
        }
        float PositionAmount = (ValueAmount * 2f) - 1f;

        Vector2 PointerPosition = Position + _pointerOffset + (s_halfSize * _valuePanelDirectionUnit * PositionAmount);
        _pointer.Position.Vector = PointerPosition;

        Vector2 ConnectorPosition = PointerPosition + (_pointer.TextureSize * _pointerDirectionUnit);
        _connector.Position.Vector = ConnectorPosition;
        _receiver.Position.Vector = ConnectorPosition;
        _receiverLights.Position.Vector = ConnectorPosition;
    }
    

    // Inherited methods.
    public override void Draw()
    {
        _panel.Draw();
        _glow?.Draw();
        _valuePanel?.Draw();
        _valueGlow?.Draw();
        _pointer.Draw();
        _connector?.Draw();
        _receiver?.Draw();
        _receiverLights?.Draw();
    }

    public override void Update()
    {
        UpdatePositions();
    }
}