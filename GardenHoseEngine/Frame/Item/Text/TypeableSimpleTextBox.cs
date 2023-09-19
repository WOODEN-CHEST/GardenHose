using GardenHoseEngine.Audio;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Text;

public class TypeableSimpleTextBox : SimpleTextBox, ITimeUpdateable
{
    // Fields.
    public bool IsTypeable
    {
        get => _isTypeable;
        set
        {
            if (value == _isTypeable) return;

            if (value == true)
            {
                _input.AddListener(MouseListenerCreator.SingleButton(_input, this, null, true,
                    MouseCondition.OnClick, OnUserRightClickEvent, MouseButton.Left));
            }
        }
    }

    public bool IsFocused
    {
        get => _isFocused;
        set
        {
            if (value == _isFocused) return;

            if (value)
            {
                _input.TextInput += OnTextInputEvent;
            }
            else
            {
                _input.TextInput -= OnTextInputEvent;
            }
        }
    }

    public Color CursorColor { get; set; } = Color.Red;

    public ITimeUpdater Updater { get; private set; }


    // Private fields
    private string[] _formattedLines;

    private UserInput _input;
    private Texture2D _singlePixel;
    private bool _isTypeable = false;

    private bool _isFocused;
    private Point _cursorPosition = Point.Zero;
    private Vector2 _cursorPositionVisual = Vector2.Zero;
    private double _cursorTimeSeconds;
    private const double CURSOR_BLINK_TIME_SECONDS = 0.53d;

    private Sound test;


    // Constructors.
    public TypeableSimpleTextBox(ITimeUpdater updater, IVirtualConverter converter, IDrawer drawer, 
        Texture2D singlePixel, UserInput input, Sound sound, DynamicFont font, string text) 
        : base(updater, converter, drawer, font, text)
    {
        test = sound;
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _singlePixel = singlePixel ?? throw new ArgumentNullException(nameof(singlePixel));
        Updater = updater ?? throw new ArgumentNullException(nameof(updater));
    }


    // Private methods.
    private void OnUserRightClickEvent(object? sender, EventArgs args)
    {
        if (!IsLocationOverBox(_input.VirtualMousePosition.Current))
        {
            IsFocused = false;
            return;
        }

        IsFocused = true;
        _cursorTimeSeconds = 0d;
        SetCursorPosition(_input.VirtualMousePosition.Current);

        test.CreateSoundInstance();
    }

    private bool IsLocationOverBox(Vector2 location)
    {
        (Vector2 CornerMin, Vector2 CornerMax) = GetBoxBounds();

        return (CornerMin.X <= location.X) && (location.X <= CornerMax.X)
            && (CornerMin.Y <= location.Y) && (location.Y <= CornerMax.Y);
    }

    private void SetCursorPosition(Vector2 location)
    {
        (Vector2 CornerMin, _) = GetBoxBounds();

        // Determine line.
        _cursorPosition.Y = (int)((location.Y - CornerMin.Y) / Font.SpriteFont.LineSpacing);
        _cursorPosition.Y = Math.Clamp(_cursorPosition.Y, 0, _formattedLines.Length - 1);
        string Line = _formattedLines[_cursorPosition.Y];

        // Determine character in line.
        DeltaValue<float> LinePixelPosition = new(CornerMin.X);
        bool CharWasFound = false;

        for (_cursorPosition.X = 0; _cursorPosition.X < Line.Length; _cursorPosition.X++)
        {
            LinePixelPosition.Update(
                LinePixelPosition.Current + Font.MeasureString(Line[_cursorPosition.X].ToString()).X);

            if (LinePixelPosition.Current > location.X)
            {
                CharWasFound = true;
                break;
            }
        }

        if (!CharWasFound)
        {
            _cursorPosition.X += 1;
            LinePixelPosition.Update(LinePixelPosition.Current);
        }


        // Visual position.
        Vector2 VisualPosition = CornerMin;
        VisualPosition.Y += _cursorPosition.Y * Font.SpriteFont.LineSpacing;
        VisualPosition.X = LinePixelPosition.Previous;

        _cursorPositionVisual = VisualPosition;
    }

    private (Vector2 Min, Vector2 Max) GetBoxBounds()
    {
        Vector2 Min = Position - Origin;
        Vector2 Max = Position - Origin + MaxSize;
        return (Min, Max);
    }

    private void OnTextInputEvent(object? sender, TextInputEventArgs args)
    {

    }

    private void TextInputDelete()
    {

    }

    private void TextInputType(TextInputEventArgs args)
    {

    }

    private int GetRealTextCursorPosition()
    {
        int Position = 0;

        for (int i = 0; i < _cursorPosition.Y - 1; i++)
        {
            Position += _formattedLines[i].Length;
        }

        Position += _cursorPosition.X;
        return Position;
    }


    // Inherited methods.
    protected override void FormatText()
    {
        base.FormatText();
        _formattedLines = FormattedText.Split('\n');
    }

    public void Update(TimeSpan passedTime)
    {
        if (!(IsTypeable && IsFocused) ) return;
    }

    public override void Draw(TimeSpan passedTime, SpriteBatch spriteBatch)
    {
        if (!IsVisible) return;

        base.Draw(passedTime, spriteBatch);

        if (!IsFocused ) return;

        _cursorTimeSeconds += passedTime.TotalSeconds;
        if (_cursorTimeSeconds > CURSOR_BLINK_TIME_SECONDS)
        {
            _cursorTimeSeconds %= CURSOR_BLINK_TIME_SECONDS * 2d;
            return;
        }

        spriteBatch.Draw(_singlePixel,
            Converter.ToRealPosition(_cursorPositionVisual),
            null,
            CursorColor,
            0f,
            Vector2.Zero,
            Converter.ToRealScale(new Vector2(2.5f, Font.SpriteFont.LineSpacing)),
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }
}