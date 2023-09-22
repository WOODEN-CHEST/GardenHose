using GardenHose.Frames.Global;
using GardenHoseEngine;
using GardenHoseEngine.Collections;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using GardenHoseEngine.Frame.Item.Text;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GardenHose.UI.Buttons.Connector;

internal partial class ConnectorButton : ITimeUpdatable, ITimeUpdater
{
    // Fields.
    public ITimeUpdater Updater { get; private init; }


    // Internal fields.
    internal IDrawer? Drawer
    {
        get => _panel.Drawer;
        set
        {
            IDrawer Drawer = _panel.Drawer;

            if (value == Drawer) return;

            
            if (Drawer != null)
            {
                RemoveAllDrawables();
            }

            _panel.Drawer = value;
            _glow.Drawer = value;
            _connector.Drawer = value;
            _receiver.Drawer = value;
            _receiverLights.Drawer = value;
            _text.Drawer = value;

            if (value != null)
            {
                AddAllDrawables();
            }
        }
    }

    internal bool IsClickable { get; set; } = true;

    internal bool IsClickingResetOnClick { get; set; } = true;

    internal bool IsUpdated { get; set; } = true;

    internal bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value == _isEnabled) return;

            _isEnabled = value;
            if (_isEnabled)
            {
                Enable();
            }
            else
            {
                Disable();
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

    /* Button properties */
    private readonly Vector2 _size;
    private Vector2 _scale;
    private readonly Direction _connectDirection;
    private readonly Vector2 _directionUnitVector;

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
    private const double CLICK_COLOR_FADE_SPEED = 2.5d;
    private const double COLOR_CHANGE_SPEED_UP = 12d;
    private const double COLOR_CHANGE_SPEED_DOWN =4d;

    /* Functional button object. */
    private readonly Button _button;
    private bool _isButtonHovered = false;


    /* Receiver */
    private DeltaValue<float> _connectionStrengthDelta = new(0f);
    private const float MAX_CONNECTION_STRENGTH = 1f;
    private const float MIN_CONNECTION_STRENGTH = 0f;
    private Vector2 _receiverDistance;
    private const float RECEIVER_MAX_DISTANCE = 100f;

    /* Bounce animation. */
    private float BounceProgress
    {
        get => _bouncePeriod;
        set => _bouncePeriod = Math.Clamp(value, 0f, MathF.PI);
    }
    private float _bouncePeriod = MathF.PI;
    private const float MAX_BOUNCE_DISTANCE = 15f;
    private const float BOUNCE_SPEED = 5f * MathF.PI;


    /* Text. */
    private readonly SimpleTextBox _text;


    // Constructors.
    private ConnectorButton(ITimeUpdater updater,
        IDrawer? drawer,
        Direction connectDirection,
        SpriteItem panel,
        SpriteItem glow,
        Vector2 position,
        Vector2 scale,
        Vector2 size)
    {
        Updater = updater;
        Updater.AddUpdateable(this);

        _connectDirection = connectDirection;
        _size = size;

        _panel = panel;
        _glow = glow;
        _connector = new(updater, GH.Engine.Display, drawer, s_connectorInstance!);
        _receiver = new(updater, GH.Engine.Display, drawer, s_receiverInstance!);
        _receiverLights = new(updater, GH.Engine.Display, drawer, s_receiverLightsInstance!);

        _button = new(GH.Engine.UserInput, updater, new RectangleButtonComponent());
        CreateButtonHandlers();

        _text = new(updater, GH.Engine.Display, drawer, GlobalFrame.GeEichFontLarge, string.Empty);
        _text.Origin = Origin.Center;
        _text.IsShadowEnabled = true;
        _text.ShadowColor = new(50, 50, 50, 255);
        _text.ShadowOffset = 0.08f;

        _position = new(updater, position);
        Scale = scale;
        SyncItemRotations();
        
        _directionUnitVector = connectDirection switch
        {
            Direction.Right => new(1f, 0f),
            Direction.Left => new(-1f, 0f),
            Direction.Up => new(0f, -1f),
            Direction.Down => new(0f, 1f),
        };
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
        float BounceDistance = MathF.Sin(_bouncePeriod) * MAX_BOUNCE_DISTANCE;

        Vector2 Position = _position;
        Position.X -= BounceDistance * _directionUnitVector.X;
        Position.Y -= BounceDistance * _directionUnitVector.Y;

        // Set positions.
        _panel.Position.Vector = Position;
        _glow.Position.Vector = Position;
        _button.Position = _position; // On purpose.

        Vector2 ConnectorPosition = Position;
        Vector2 ReceiverPosition = Position;

        switch (_connectDirection)
        {
            case Direction.Right:
                ConnectorPosition += new Vector2((_size.X / 2f) * Scale.X, 0f);
                ReceiverPosition = ConnectorPosition;
                ReceiverPosition.X += (1f - _connectionStrengthDelta.Current) * RECEIVER_MAX_DISTANCE;
                break;

            case Direction.Left:
                ConnectorPosition += new Vector2(-((_size.X / 2f) * Scale.X), 0f);
                ReceiverPosition = ConnectorPosition;
                ReceiverPosition.X -= (1f - _connectionStrengthDelta.Current) * RECEIVER_MAX_DISTANCE;
                break;

            case Direction.Up:
                ConnectorPosition += new Vector2(0f, -((_size.Y / 2f) * Scale.Y));
                ReceiverPosition = ConnectorPosition;
                ReceiverPosition.Y -= (1f - _connectionStrengthDelta.Current) * RECEIVER_MAX_DISTANCE;
                break;

            case Direction.Down:
                ConnectorPosition += new Vector2(0f, (_size.Y / 2f) * Scale.Y);
                ReceiverPosition = ConnectorPosition;
                ReceiverPosition.Y += (1f - _connectionStrengthDelta.Current) * RECEIVER_MAX_DISTANCE;
                break;

            default:
                throw new EnumValueException(nameof(_connectDirection), nameof(Direction),
                    _connectDirection.ToString(), (int)_connectDirection);
        }

        _connector.Position.Vector = ConnectorPosition;
        _receiver.Position.Vector = ReceiverPosition;
        _receiverLights.Position.Vector = ReceiverPosition;

        _text.Position.Vector = Position;
        _text.Position.Vector.Y += 5f;
    }

    private void SyncItemRotations()
    {
        float Rotation = (_connectDirection) switch
        {
            Direction.Right => 0f,
            Direction.Left => MathF.PI,
            Direction.Up => -(MathF.PI / 2f),
            Direction.Down => MathF.PI / 2f,

            _ => throw new EnumValueException(nameof(_connectDirection), nameof(Direction),
                    _connectDirection.ToString(), (int)_connectDirection)
        };

        _connector.Rotation = Rotation;
        _receiver.Rotation = Rotation;
        _receiverLights.Rotation = Rotation;
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
    private void Disable()
    {
        Updater.RemoveUpdateable(this);

        _button.ClearEventHandlers();

        if (Drawer != null)
        {
            AddAllDrawables();
        }
    }

    private void Enable()
    {
        Updater.AddUpdateable(this);

        CreateButtonHandlers();

        if (Drawer != null)
        {
            RemoveAllDrawables();
        }
    }

    private void CreateButtonHandlers()
    {
        _button.SetEventHandler(ButtonEvent.OnHover, OnButtonHoverEvent);
        _button.SetEventHandler(ButtonEvent.OnUnhover, OnButtonUnHoverEvent);
        _button.SetEventHandler(ButtonEvent.LeftClick, OnButtonClickEvent);
    }

    private void AddAllDrawables()
    {
        Drawer!.RemoveDrawableItem(_panel);
        Drawer.RemoveDrawableItem(_glow);
        Drawer.RemoveDrawableItem(_connector);
        Drawer.RemoveDrawableItem(_receiver);
        Drawer.RemoveDrawableItem(_receiverLights);
        Drawer.RemoveDrawableItem(_text);
    }

    private void RemoveAllDrawables()
    {
        Drawer!.AddDrawableItem(_panel);
        Drawer.AddDrawableItem(_glow);
        Drawer.AddDrawableItem(_connector);
        Drawer.AddDrawableItem(_receiver);
        Drawer.AddDrawableItem(_receiverLights);
        Drawer.AddDrawableItem(_text);
    }


    // Inherited methods. 
    public void Update(TimeSpan passedTime)
    {
        if (!IsUpdated) return;

        //  Click amount.
        ClickColorAmount -= (float)(passedTime.TotalSeconds * CLICK_COLOR_FADE_SPEED);

        // Connection.
        if (_isButtonHovered)
        {
            ConnectionStrength += (float)(passedTime.TotalSeconds * COLOR_CHANGE_SPEED_UP);
        }
        else
        {
            ConnectionStrength -= (float)(passedTime.TotalSeconds * COLOR_CHANGE_SPEED_DOWN);
        }

        // Bounce animation.
        BounceProgress += (float)(passedTime.TotalSeconds * BOUNCE_SPEED);

        // Update items.
        SyncItemPositions();
    }

    public void ForceRemove()
    {
        Updater.RemoveUpdateable(this);
        _button.ClearEventHandlers();
    }

    public void AddUpdateable(ITimeUpdatable item) { }

    public void RemoveUpdateable(ITimeUpdatable item) { }
}