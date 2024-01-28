using GardenHose.Frames.Global;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Text;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace GardenHose.UI.Buttons.Connector;

internal class DropDownList<OptionType> : ConnectorElement
{
    // Internal fields.
    internal OptionType[]? Options
    {
        get
        {
            if (_options == null)
            {
                return null;
            }

            return _options.Select((option) => option.Value).ToArray();
        }
        set
        {
            SetOptions(value);
        }
    }

    internal OptionType? SelectedOption
    {
        get
        {
            if (_options == null || _options.Length == 0)
            {
                return default;
            }
            return _options[_selectedOptionIndex].Value;
        }
    }

    internal int SelectedOptionIndex
    {
        get => _selectedOptionIndex;
        set
        {
            if (_options == null)
            {
                return;
            }

            SelectOption(_options![Math.Clamp(value, 0, _options.Length - 1)]);
        }
    }

    internal override Vector2 Scale 
    {
        get => base.Scale;
        set
        {
            base.Scale = value;
            _panel.Scale.Vector = (s_panelSize / _panel.TextureSize) * value;
            _glow.Scale.Vector = (s_panelSize / _glow.TextureSize) * value;

            float TextScale = value.X * TEXT_SCALE;
            float TextLength = _text.RealPixelSize.X * TextScale;
            float MaxTextLength = (s_panelSize.X - TEXT_PADDING_X) * Scale.X;
            if (TextLength > MaxTextLength)
            {
                float NewScale = MaxTextLength / TextLength;
                _text.Scale = new Vector2(NewScale) * TextScale;
            }
            else
            {
                _text.Scale = value * TEXT_SCALE;
            }

            if (_options == null) return;

            foreach (var Option in _options)
            {
                Option.Scale = value;
            }
        }
    }

    internal bool IsHovered
    {
        get
        {
            Vector2 HalfSize = _glow.TextureSize * 0.5f * _panel.Scale;
            HalfSize.Y *= _itemStep;
            Vector2 StartPosition = Position - HalfSize;
            Vector2 EndPosition = Position + HalfSize;

            for (int i = 0; i < _expandedOptionCount; i++)
            {
                EndPosition.Y += (_options![0].Button.Size.Y * _options[0].Button.Scale.Y) * _itemStep;
            }

            float MinY;
            float MaxY;
            (MinY, MaxY) = StartPosition.Y < EndPosition.Y ? 
                (StartPosition.Y, EndPosition.Y) 
                : (EndPosition.Y, StartPosition.Y);

            Vector2 MousePosition = UserInput.VirtualMousePosition.Current;
            return (StartPosition.X <= MousePosition.X && MousePosition.X <= EndPosition.X
                && MinY <= MousePosition.Y && MousePosition.Y <= MaxY);
        }
    }

    internal int ExpandedOptionCount
    {
        get => _expandedOptionCount;
        set
        {
            if (_options ==  null)
            {
                throw new InvalidOperationException("Options are null, cannot set expanded option count.");
            }

            _expandedOptionCount = Math.Clamp(value, 0, _options.Length);
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


    // Private static fields.
    private static readonly Vector2 s_panelSize = new(200, 50);
    private static readonly FloatColor s_hoverColor = new(118, 20, 255, 255);
    private static readonly FloatColor s_hoverColorText = s_hoverColor * 2f;


    // Private fields.
    private readonly SpriteItem _panel = new(NormalPanelAnim!);
    private readonly SpriteItem _glow = new(NormalGlowAnim!);

    private float _itemStep;

    private DropDownListOption<OptionType>[]? _options;
    private int _selectedOptionIndex;

    private Direction _connectDirection;
    private int _expandedOptionCount = 0;
    private float _optionExpansionTimer = 0f;
    private const float TIME_PER_EXPANSION = 1 / 40f;

    private readonly SimpleTextBox _text = new(GlobalFrame.GeEichFontLarge, "")
    {
        IsShadowEnabled = true,
        ShadowColor = TEXT_SHADOW_COLOR,
        ShadowOffset = TEXT_SHADOW_OFFSET,
        Origin = Origin.Center
    };
    private const float TEXT_SCALE = 0.51f;
    private const float TEXT_PADDING_X = 10f;
    private const float TEXT_PADDING_Y = 2.5f;

    private float _hoverStrength;
    private const float HOVER_STRENGTH_CHANGE_SPEED = 8f;
    private float _clickStrength;
    private const float CLICK_STRENGTH_CHANGE_SPEED = 2f;


    // Constructors.
    internal DropDownList(Direction connectiDirection, OptionType[]? options, int selectedOption)
    {
        if (connectiDirection is Direction.Up or Direction.Down)
        {
            throw new ArgumentNullException(nameof(connectiDirection), "Connect direction may only be left or right.");
        }
        _connectDirection = connectiDirection;

        SetOptions(options);
        SelectedOptionIndex = selectedOption;
        Scale = Vector2.One;
    }


    // Internal methods.
    internal void SetOptions(OptionType[]? options)
    {
        if (options == null || options.Length < 1)
        {
            _options = null;
            Text = "Empty List";
            return;
        }

        _options = new DropDownListOption<OptionType>[options.Length];
        for (int i = 0; i < _options.Length; i++)
        {
            _options[i] = new(options[i], _connectDirection, Scale, SelectOption);
            _options[i].Button.Text = options[i]!.ToString()!;
        }
        SelectedOptionIndex = _selectedOptionIndex;
    }


    // Private methods.
    private void SelectOption(DropDownListOption<OptionType> selectedOption)
    {
        if (_options == null)
        {
            throw new InvalidOperationException("Cannot select option, options are null.");
        }

        int Index = 0;
        for (; Index < _options.Length; Index++)
        {
            if (_options[Index] == selectedOption)
            {
                break;
            }
        }

        _selectedOptionIndex = Math.Clamp(Index, 0, _options.Length);
        Text = SelectedOption!.ToString()!;
        _clickStrength = 1f;
    }

    private void UpdateButtonAbility()
    {
        for (int i = 0; i < _options!.Length; i++)
        {
            bool IsButtonEnabled = i < ExpandedOptionCount;
            _options[i].Button.IsFunctional = IsButtonEnabled;

            if (!IsButtonEnabled)
            {
                _options[i].Button.ConnectionStrength = 0f;
            }
        }
    }

    private void UpdateColors(bool isHovering)
    {
        _glow.Mask = FloatColor.InterpolateRGB(UNSELECTED_COLOR, 
            FloatColor.InterpolateRGB(s_hoverColor, DEFAULT_CLICKED_COLOR, _clickStrength), _hoverStrength);
        _text.Mask = FloatColor.InterpolateRGB(FloatColor.White, 
            FloatColor.InterpolateRGB(s_hoverColorText, DEFAULT_CLICKED_COLOR, _clickStrength), _hoverStrength);

        _hoverStrength = Math.Clamp(_hoverStrength + (isHovering ?
             HOVER_STRENGTH_CHANGE_SPEED * GameFrameManager.PassedTimeSeconds :
             -(HOVER_STRENGTH_CHANGE_SPEED * GameFrameManager.PassedTimeSeconds)), 0f, 1f);

        _clickStrength = Math.Clamp(_clickStrength - CLICK_STRENGTH_CHANGE_SPEED * GameFrameManager.PassedTimeSeconds, 0f, 1f);
    }


    // Inherited methods.
    public override void Update()
    {
        _panel.Position.Vector = Position;
        _glow.Position.Vector = Position;
        _text.Position.Vector = Position + (new Vector2(0f, TEXT_PADDING_Y) * Scale);

        bool IsHovering = IsHovered;
        UpdateColors(IsHovering);
        _itemStep = Position.Vector.Y > Display.VirtualSize.Y * 0.5f ? -1f : 1f;

        if (_options == null)
        {
            return;
        }
        
        _optionExpansionTimer += GameFrameManager.PassedTimeSeconds;

        if (_optionExpansionTimer > TIME_PER_EXPANSION)
        {
            ExpandedOptionCount += IsHovering ? 1 : -1;
            _optionExpansionTimer = 0f;
            UpdateButtonAbility();
        }

        float Step = Position.Vector.Y > Display.VirtualSize.Y * 0.5f ? -1f : 1f;
        for (int i = 0; i < _expandedOptionCount; i++)
        {
            var Option = _options![i];

            Vector2 ButtonPosition = Position;
            ButtonPosition.Y += ((s_panelSize.Y) + (i * Option.Button.Size.Y * Option.Scale.Y)) * Step;
            Option.Button.Position.Vector = ButtonPosition;
            Option.Button.Update();
        }
    }

    public override void Draw()
    {
        _panel.Rotation = 0f;
        _glow.Rotation = 0f;
        _panel.Draw();
        _glow.Draw();

        _text.Draw();

        if (_options == null)
        {
            return;
        }

        for (int i = 0; i < _expandedOptionCount; i++)
        {
            _options[i].Draw();
        }
    }
}