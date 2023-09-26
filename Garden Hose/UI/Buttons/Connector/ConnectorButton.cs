using GardenHose.Frames.Global;
using GardenHoseEngine;
using GardenHoseEngine.Collections;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using GardenHoseEngine.Frame.Item.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GardenHose.UI.Buttons.Connector;

internal partial class ConnectorButton : ITimeUpdatable, IDrawableItem
{
    // Fields.
    public bool IsVisible { get; set; } = true;
    public Effect? Shader { get; set; }


    // Internal fields.
    internal bool IsClickable { get; set; } = true;

    internal bool IsClickingResetOnClick { get; set; } = true;


    internal bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value == _isEnabled) return;

            _isEnabled = value;
            if (_isEnabled)
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

    internal FloatColor ClickedColor { get; set; } = s_defaultClickedColor;

    internal AnimVector2 Position
    {
        get => _position;
    }

    internal Vector2 Scale
    {
        get => _scale;
        set
        {
            _scale =  value;

            _panel.Scale.Vector = (_size / _panel.TextureSize) * value;
            _glow.Scale.Vector = (_size / _glow.TextureSize) * value;
            _connector.Scale.Vector = value;
            _receiver.Scale.Vector = value;
            _receiverLights.Scale.Vector =  value;

            _text.Scale = value * 3f;

            _button.Components[0] = new RectangleButtonComponent(
                new Vector2(_size.X * value.X, _size.Y * value.Y));

            SyncItemPositions();
        }
    }

    internal Vector2 Size => _size;

    internal float ConnectionStrength
    {
        get => _connectionStrengthDelta.Current;
        set
        {
            _connectionStrengthDelta.Update(Math.Clamp(value, 0f, 1f));
            SyncConnectionProperties();
        }
    }

    internal string Text
    {
        get => _text.Text;
        set
        {
            _text.Text = value;
        }
    }

    internal EventHandler ClickHandler { get; set; }


    // Private fields.
    private AnimVector2 _position;
    private Vector2 _scale;

    /* Button properties */
    private readonly Vector2 _size;
    private readonly Vector2 _halfSize;
    private readonly Vector2 _directionUnit;
    
    

    private bool _isEnabled = true;

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
    private const float COLOR_CHANGE_SPEED_DOWN =4f;

    /* Functional button object. */
    private readonly Button _button;
    private bool _isButtonHovered = false;


    /* Receiver */
    private DeltaValue<float> _connectionStrengthDelta = new(0f);
    private const float MAX_CONNECTION_STRENGTH = 1f;
    private const float MIN_CONNECTION_STRENGTH = 0f;
    private Vector2 _receiverDistance;
    private const float MAX_RECEIVER_DISTANCE = 100f;

    /* Bounce animation. */
    private const float MAX_BOUNCE_DISTANCE = 15f;
    private const float BOUNCE_SPEED = 5f * MathF.PI;
    private float _bouncePeriod = MathF.PI;
    


    /* Text. */
    private readonly SimpleTextBox _text;


    // Constructors.
    private ConnectorButton(Direction connectDirection,
        SpriteItem panel,
        SpriteItem glow,
        Vector2 position,
        Vector2 scale,
        Vector2 size)
    {
        // Create components.
        _panel = panel;
        _glow = glow;
        _connector = new(GH.Engine.Display, s_connectorInstance!);
        _receiver = new(GH.Engine.Display, s_receiverInstance!);
        _receiverLights = new(GH.Engine.Display, s_receiverLightsInstance!);

        _button = new(GH.Engine.UserInput, new RectangleButtonComponent());
        CreateButtonHandlers();

        _text = new(GH.Engine.Display, GlobalFrame.GeEichFontLarge, string.Empty)
        {
            Origin = Origin.Center,
            IsShadowEnabled = true,
            ShadowColor = new(50, 50, 50, 255),
            ShadowOffset = 0.08f
        };
        

        // Assign readonly properties.
        _directionUnit = connectDirection switch
        {
            Direction.Right => new(1f, 0f),
            Direction.Left => new(-1f, 0f),
            Direction.Up => new(0f, -1f),
            Direction.Down => new(0f, 1f),
        };
        _size = size;
        _halfSize = _size / 2f;
        float Rotation = connectDirection switch
        {
            Direction.Right => 0f,
            Direction.Left => MathF.PI,
            Direction.Up => -(MathF.PI / 2f),
            Direction.Down => MathF.PI / 2f,
            _ => throw new EnumValueException(nameof(connectDirection), nameof(Direction),
                    connectDirection.ToString(), (int)connectDirection)
        };
        _connector.Rotation = Rotation;
        _receiver.Rotation = Rotation;
        _receiverLights.Rotation = Rotation;

        // Assign changeable properties. 
        _position = new(position);
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
    private void SyncItemPositions()
    {
        // Bounce.
        Vector2 BounceDistance =  (-_directionUnit) * MathF.Sin(_bouncePeriod) * MAX_BOUNCE_DISTANCE;
        Vector2 VisualPosition = _position + BounceDistance;

        // Main positions.
        _panel.Position.Vector = VisualPosition;
        _glow.Position.Vector = VisualPosition;
        _button.Position = Position;
        _text.Position.Vector = VisualPosition;
        _text.Position.Vector.Y += 5f;

        // Connector and receiver positions.
        Vector2 ConnectorPosition = VisualPosition + (_halfSize * _directionUnit) * Scale;
        Vector2 ReceiverPosition = ConnectorPosition + (_directionUnit * (1f - ConnectionStrength) * MAX_RECEIVER_DISTANCE);

        _connector.Position.Vector = ConnectorPosition;
        _receiver.Position.Vector = ReceiverPosition;
        _receiverLights.Position.Vector = ReceiverPosition;

        
    }

    private void SyncConnectionProperties()
    {
        _glowColor = s_unselectedColor;

        if (_connectionStrengthDelta.Current < _connectionStrengthDelta.Previous
            || _connectionStrengthDelta.Current == MAX_CONNECTION_STRENGTH)
        {
            _glowColor.R = (s_selectedColor.R + (ClickedColor.R - s_selectedColor.R) * _clickColorAmount);
            _glowColor = FloatColor.InterpolateRGB(s_unselectedColor, 
                FloatColor.InterpolateRGB(s_selectedColor, ClickedColor, _clickColorAmount), ConnectionStrength);
        }

        _glow.Mask = _glowColor;
        _receiverLights.Mask = (ConnectionStrength == MAX_CONNECTION_STRENGTH) ? _glowColor  : s_unselectedColor;
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
    public void Update(float passedTimeSeconds)
    {
        if (!IsEnabled) return;

        // Update items.
        _position.Update(passedTimeSeconds);
        _button.Update(passedTimeSeconds);

        //  Click amount.
        ClickColorAmount -= passedTimeSeconds * CLICK_COLOR_FADE_SPEED;

        // Bounce animation.
        _bouncePeriod = Math.Clamp(_bouncePeriod + (passedTimeSeconds * BOUNCE_SPEED), 0f, MathF.PI);

        // Connection.
        if (_isButtonHovered)
        {
            ConnectionStrength += passedTimeSeconds * COLOR_CHANGE_SPEED_UP;
        }
        else
        {
            ConnectionStrength -= passedTimeSeconds * COLOR_CHANGE_SPEED_DOWN;
        }

        // Update items.
        SyncItemPositions();
    }

    public void Draw(float passedTimeSeconds, SpriteBatch spriteBatch)
    {
        if (!IsVisible) return;

        _panel.Draw(passedTimeSeconds, spriteBatch);
        _glow.Draw(passedTimeSeconds, spriteBatch);
        _connector.Draw(passedTimeSeconds, spriteBatch);
        _receiver.Draw(passedTimeSeconds, spriteBatch);
        _receiverLights.Draw(passedTimeSeconds, spriteBatch);
        _text.Draw(passedTimeSeconds, spriteBatch);
    }
}