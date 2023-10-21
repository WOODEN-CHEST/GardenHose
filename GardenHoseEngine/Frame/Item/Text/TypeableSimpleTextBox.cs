﻿using GardenHoseEngine.Audio;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Text;

public class TypeableSimpleTextBox : SimpleTextBox
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
                UserInput.AddListener(_clickListener);
            }
            else
            {
                _clickListener.StopListening();
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
                UserInput.TextInput += OnTextInputEvent;

                UserInput.AddListener(_endKeyListener);
                UserInput.AddListener(_homeKeyListener);
                UserInput.AddListener(_escapeKeyListener);
                UserInput.AddListener(_leftKeyListener);
                UserInput.AddListener(_rightKeyListener);
                UserInput.AddListener(_upKeyListener);
                UserInput.AddListener(_downKeyListener);
            }
            else
            {
                UserInput.TextInput -= OnTextInputEvent;

                _endKeyListener.StopListening();
                _homeKeyListener.StopListening();
                _escapeKeyListener.StopListening();
                _leftKeyListener.StopListening();
                _rightKeyListener.StopListening();
                _upKeyListener.StopListening();
                _downKeyListener.StopListening();
            }

            _isFocused = value;
        }
    }

    public Color CursorColor { get; set; } = Color.Red;

    public Predicate<TextInputEventArgs> TextTypeCallback { get; set; }

    public Func<bool> TextDeleteCallback { get; set; }


    // Private fields
    private string[] _formattedLines;

    private bool _isTypeable = false;

    private bool _isFocused;
    private Point _cursorPosition = Point.Zero;
    private Vector2 _cursorPositionVisual = Vector2.Zero;
    private float _cursorTimeSeconds;
    private const float CURSOR_BLINK_TIME_SECONDS = 0.53f;

    private IInputListener _clickListener;
    private IInputListener _endKeyListener;
    private IInputListener _homeKeyListener;
    private IInputListener _escapeKeyListener;
    private IInputListener _rightKeyListener;
    private IInputListener _leftKeyListener;
    private IInputListener _upKeyListener;
    private IInputListener _downKeyListener;


    // Constructors.
    public TypeableSimpleTextBox(SpriteFont font, string text) 
        : base(font, text)
    {
        _clickListener = MouseListenerCreator.SingleButton(this, true,
                    MouseCondition.OnClick, OnUserRightClickEvent, MouseButton.Left);

        _endKeyListener = KeyboardListenerCreator.SingleKey(this, KeyCondition.OnPress,
                    OnEndKeyPressEvent, Keys.End);
        _homeKeyListener = KeyboardListenerCreator.SingleKey(this, KeyCondition.OnPress,
                    OnHomeKeyPressEvent, Keys.Home);
        _escapeKeyListener = KeyboardListenerCreator.SingleKey(this, KeyCondition.OnPress,
            OnEscapeKeyPressEvent, Keys.Escape);
        _leftKeyListener = KeyboardListenerCreator.SingleKey(this, KeyCondition.OnPress,
            OnLeftKeyPressEvent, Keys.Left);
        _rightKeyListener = KeyboardListenerCreator.SingleKey(this, KeyCondition.OnPress,
            OnRightKeyPressEvent, Keys.Right);
        _upKeyListener = KeyboardListenerCreator.SingleKey(this, KeyCondition.OnPress,
            OnUpKeyPressEvent, Keys.Up);
        _downKeyListener = KeyboardListenerCreator.SingleKey(this, KeyCondition.OnPress,
            OnDownKeyPressEvent, Keys.Down);
    }


    // Methods.
    /* Cursor. */
    public void SetCursorPosition(Vector2 pixelLocation)
    {
        (Vector2 CornerMin, _) = GetBoxBounds();

        // Determine line.
        _cursorPosition.Y = (int)((pixelLocation.Y - CornerMin.Y) / Font.LineSpacing);
        _cursorPosition.Y = Math.Clamp(_cursorPosition.Y, 0, _formattedLines.Length - 1);
        string Line = _formattedLines[_cursorPosition.Y];

        // Determine character in line.
        DeltaValue<float> LinePixelPosition = new(CornerMin.X);
        bool CharWasFound = false;

        for (_cursorPosition.X = 0; _cursorPosition.X < Line.Length; _cursorPosition.X++)
        {
            LinePixelPosition.Update(
                LinePixelPosition.Current + Font.MeasureString(Line[_cursorPosition.X].ToString()).X);

            if (LinePixelPosition.Current > pixelLocation.X)
            {
                CharWasFound = true;
                break;
            }
        }

        if (!CharWasFound)
        {
            LinePixelPosition.Update(LinePixelPosition.Current);
        }

        UpdateCursor();
    }

    public void SetCursorPosition(Point position)
    {
        if (position.X < 0)
        {
            position.Y -= 1;
            position.Y = Math.Clamp(position.Y, 0, _formattedLines.Length - 1);
            position.X = _formattedLines[position.Y].Length;

            _cursorPosition = position;
            UpdateCursor();
            return;
        }


        position.Y = Math.Clamp(position.Y, 0, _formattedLines.Length - 1);

        if (position.X > _formattedLines[position.Y].Length)
        {
            position.Y += 1;
            if (position.Y < _formattedLines.Length)
            {
                position.X = 0;
            }
            else
            {
                position.Y -= 1;
                position.X = _formattedLines[position.Y].Length;
            }
        }

        _cursorPosition = position;
        UpdateCursor();
    }

    public void SetCursorPosition(int realTextIndex)
    {
        realTextIndex = Math.Clamp(realTextIndex, 0, RealText.Length);
        int ExistingLength = 0;
        Point Position = Point.Zero;

        for (int i = 0; i < _formattedLines.Length; i++, Position.Y += 1)
        {
            ExistingLength += _formattedLines[i].Length + 1;

            if (ExistingLength > realTextIndex)
            {
                ExistingLength -= _formattedLines[i].Length + 1;
                break;
            }
        }

        Position.X = ExistingLength + realTextIndex;

        _cursorPosition = Position;
        UpdateCursor();
    }


    // Private methods.
    private void OnUserRightClickEvent(object? sender, EventArgs args)
    {
        if (!IsLocationOverBox(UserInput.VirtualMousePosition.Current))
        {
            IsFocused = false;
            return;
        }

        IsFocused = true;
        SetCursorPosition(UserInput.VirtualMousePosition.Current);
    }

    private bool IsLocationOverBox(Vector2 location)
    {
        (Vector2 CornerMin, Vector2 CornerMax) = GetBoxBounds();

        return (CornerMin.X <= location.X) && (location.X <= CornerMax.X)
            && (CornerMin.Y <= location.Y) && (location.Y <= CornerMax.Y);
    }


    /* Cursor */
    private int GetRealTextCursorPosition()
    {
        int Position = 0;

        for (int i = 0; i < _cursorPosition.Y; i++)
        {
            Position += _formattedLines[i].Length + 1;
        }

        Position += _cursorPosition.X;
        return Position;
    }

    private void SyncVisualCursor()
    {
        _cursorPositionVisual = GetBoxBounds().Min;
        _cursorPositionVisual.Y += _cursorPosition.Y * Font.LineSpacing;
        _cursorPositionVisual.X += Font.MeasureString(_formattedLines[_cursorPosition.Y][0.._cursorPosition.X]).X;
    }

    private void UpdateCursor()
    {
        _cursorTimeSeconds = 0;
        SyncVisualCursor();
    }


    /* Events. */
    private void OnTextInputEvent(object? sender, TextInputEventArgs args)
    {
        if (args.Character == '\b')
        {
            UserTextInputDelete();
        }
        else
        {
            UserTextInputType(args);
        }
    }

    private void OnEndKeyPressEvent(object? sender, EventArgs args)
    {
        SetCursorPosition(new Point(_formattedLines[_cursorPosition.Y].Length, _cursorPosition.Y));
    }

    private void OnHomeKeyPressEvent(object? sender, EventArgs args)
    {
        SetCursorPosition(new Point(0, _cursorPosition.Y));
    }

    private void OnEscapeKeyPressEvent(object? sender, EventArgs args) => IsFocused = false;

    private void OnLeftKeyPressEvent(object? sender, EventArgs args)
    {
        SetCursorPosition(new Point(_cursorPosition.X - 1, _cursorPosition.Y));
    }

    private void OnRightKeyPressEvent(object? sender, EventArgs args)
    {
        SetCursorPosition(new Point(_cursorPosition.X + 1, _cursorPosition.Y));
    }

    private void OnUpKeyPressEvent(object? sender, EventArgs args)
    {
        SetCursorPosition(new Point(_cursorPosition.X, _cursorPosition.Y - 1));
    }

    private void OnDownKeyPressEvent(object? sender, EventArgs args)
    {
        SetCursorPosition(new Point(_cursorPosition.X, _cursorPosition.Y + 1));
    }



    /* User input. */
    private void UserTextInputDelete()
    {
        if (TextDeleteCallback?.Invoke() ?? false)
        {
            return;
        }

        DeleteCharAt(GetRealTextCursorPosition());
    }

    private void UserTextInputType(TextInputEventArgs args)
    {
        if (TextTypeCallback?.Invoke(args) ?? false)
        {
            return;
        }

        char Character = args.Character;
        if (args.Character == '\r')
        {
            Character = '\n';
        }

        if ((' ' <= Character) && (Character <= '~') || (Character == '\n'))
        {
            PutCharAt(GetRealTextCursorPosition(), Character);
        }
    }

    /* Text modification. */
    private void DeleteCharAt(int realTextIndex)
    {
        // Early exit at start of text.
        if (realTextIndex == 0)
        {
            return;
        }

        // Delete based on position in text and text's length.
        realTextIndex = Math.Clamp(realTextIndex, 0, RealText.Length);
        string TextToSet;

        if (RealText.Length <= 1)
        {
            TextToSet = string.Empty;
        }
        else if (realTextIndex == RealText.Length)
        {
            TextToSet = RealText[0..^1];
        }
        else
        {
            string FirstHalf = RealText[..(realTextIndex - 1)];
            string SecondHalf = RealText[realTextIndex..];
            TextToSet = FirstHalf + SecondHalf;
        }

        Text = TextToSet;
        SetCursorPosition(new Point(_cursorPosition.X - 1, _cursorPosition.Y));
    }

    private void PutCharAt(int realTextIndex, char character)
    {
        realTextIndex = Math.Clamp(realTextIndex, 0, RealText.Length);
        string TextToSet;

        // Add based on position in text  and text's length.
        if (realTextIndex == 0)
        {
            TextToSet = character.ToString() + RealText;
        }
        else if (realTextIndex == RealText.Length)
        {
            TextToSet = RealText + character;
        }
        else
        {
            string FirstHalf = RealText[..realTextIndex];
            string SecondHalf = RealText[realTextIndex..];
            TextToSet = FirstHalf + character + SecondHalf;
        }

        Text = TextToSet;
        SetCursorPosition(new Point(_cursorPosition.X + 1, _cursorPosition.Y));
    }



    /* Other */
    private (Vector2 Min, Vector2 Max) GetBoxBounds()
    {
        Vector2 Min = Position - VectorOrigin;
        Vector2 Max = Position - VectorOrigin + MaxSize;
        return (Min, Max);
    }

    // Inherited methods.
    protected override void FormatText()
    {
        base.FormatText();
        _formattedLines = FormattedText.Split('\n');
    }

    public override void Draw()
    {
        if (!IsVisible) return;

        base.Draw();

        if (!IsFocused ) return;

        _cursorTimeSeconds += GameFrameManager.PassedTimeSeconds;
        if (_cursorTimeSeconds > CURSOR_BLINK_TIME_SECONDS)
        {
            _cursorTimeSeconds %= CURSOR_BLINK_TIME_SECONDS * 2f;
            return;
        }

        GameFrameManager.SpriteBatch.Draw(Display.SinglePixel,
            Display.ToRealPosition(_cursorPositionVisual),
            null,
            CursorColor,
            0f,
            Vector2.Zero,
            Display.ToRealScale(new Vector2(2.5f, Font.LineSpacing)),
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }
}