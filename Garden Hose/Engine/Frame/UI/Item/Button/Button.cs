using GardenHose.Engine.IO;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace GardenHose.Engine.Frame.UI.Item;

/* Code duplication, yay! */
public abstract class Button : ColoredItem, IUpdateableItem
{
    // Fields.
    public readonly GameFrame ParentFrame;
    public bool IsUpdated { get; set; } = true;
    public ButtonComponent[] Components
    {
        get => _components;
        set
        {
            if (value.Length == 0) throw new ArgumentException("No button components.");
            _components = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public EventHandler<MouseEventArgs> OnLeftClick
    {
        set => SetClickHandler(value, ref _leftClickListener, MouseCondition.OnClick, MouseButton.Left);
    }
    public EventHandler<MouseEventArgs> OnLeftRelease
    {
        set => SetClickHandler(value, ref _leftReleaseListener, MouseCondition.OnRelease, MouseButton.Left);
    }
    public EventHandler<MouseEventArgs> OnLeftHold
    {
        set => SetClickHandler(value, ref _leftHoldListener, MouseCondition.WhileDown, MouseButton.Left);
    }

    public EventHandler<MouseEventArgs> OnMiddleClick
    {
        set => SetClickHandler(value, ref _middleClickListener, MouseCondition.OnClick, MouseButton.Middle);
    }
    public EventHandler<MouseEventArgs> OnMiddleRelease
    {
        set => SetClickHandler(value, ref _middleReleaseListener, MouseCondition.OnRelease, MouseButton.Middle);
    }
    public EventHandler<MouseEventArgs> OnMiddleHold
    {
        set => SetClickHandler(value, ref _middleHoldListener, MouseCondition.WhileDown, MouseButton.Middle);
    }

    public EventHandler<MouseEventArgs> OnRightClick
    {
        set => SetClickHandler(value, ref _rightClickListener, MouseCondition.OnClick, MouseButton.Right);
    }
    public EventHandler<MouseEventArgs> OnRightRelease
    {
        set => SetClickHandler(value, ref _rightReleaseListener, MouseCondition.OnRelease, MouseButton.Right);
    }
    public EventHandler<MouseEventArgs> OnRightHold
    {
        set => SetClickHandler(value, ref _rightHoldListener, MouseCondition.WhileDown, MouseButton.Right);
    }

    public EventHandler<MouseEventArgs> OnSrollAny
    {
        set => SetScrollHandler(value, ref _scrollAnyListener, ScrollDirection.Any);
    }
    public EventHandler<MouseEventArgs> OnScrollUp
    {
        set => SetScrollHandler(value, ref _scrollUpListener, ScrollDirection.Up);
    }
    public EventHandler<MouseEventArgs> OnScrollDown
    {
        set => SetScrollHandler(value, ref _scrollDownListener, ScrollDirection.Down);
    }

    public EventHandler OnHover;
    public EventHandler OnUnHover;


    // Private fields.
    private ButtonComponent[] _components;

    private MouseListener _leftClickListener;
    private MouseListener _leftReleaseListener;
    private MouseListener _leftHoldListener;

    private MouseListener _middleClickListener;
    private MouseListener _middleReleaseListener;
    private MouseListener _middleHoldListener;

    private MouseListener _rightClickListener;
    private MouseListener _rightReleaseListener;
    private MouseListener _rightHoldListener;

    private MouseListener _scrollAnyListener;
    private MouseListener _scrollUpListener;
    private MouseListener _scrollDownListener;

    private bool _mouseOverPrev;


    // Constructors.
    public Button(GameFrame parentFrame,
        params ButtonComponent[] components)
    {
        ParentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
        Components = components;

        ParentFrame.UpdateableItems.Add(this);
    }


    // Methods.
    public bool IsMouseOverButton()
    {
        foreach (var Component in _components) if (Component.IsMouseOver()) return true;
        return false;
    }


    // Private methods.
    private void SetClickHandler(EventHandler<MouseEventArgs> handler, 
        ref MouseListener listener,
        MouseCondition condition,
        MouseButton button)
    {
        listener?.StopListening();
        if (handler == null) return;
        listener = UserInput.AddMouseClickListener(this, ParentFrame, condition, button, handler);
    }

    private void SetScrollHandler(EventHandler<MouseEventArgs> handler,
        ref MouseListener listener,
        ScrollDirection direction)
    {
        listener?.StopListening();
        if (handler == null) return;
        listener = UserInput.AddMouseScrollListener(this, ParentFrame, direction, handler);
    }


    // Inherited methods.
    public void Update()
    {
        if (!IsUpdated) return;

        bool IsMouseOver = IsMouseOverButton();

        if (IsMouseOver && !_mouseOverPrev && (OnHover != null))
        {
            Mouse.SetCursor(MouseCursor.Hand);
            OnHover.Invoke(this, EventArgs.Empty);
        }
        else if (!IsMouseOver && _mouseOverPrev && (OnUnHover != null))
        {
            Mouse.SetCursor(MouseCursor.Arrow);
            OnUnHover.Invoke(this, EventArgs.Empty);
        }

        _mouseOverPrev = IsMouseOver;
    }

    public void Delete()
    {
        _leftClickListener?.StopListening();
        _leftReleaseListener?.StopListening();
        _leftHoldListener?.StopListening();

        _middleClickListener?.StopListening();
        _middleReleaseListener?.StopListening();
        _middleHoldListener?.StopListening();

        _rightClickListener?.StopListening();
        _rightReleaseListener?.StopListening();
        _rightHoldListener?.StopListening();

        ParentFrame.UpdateableItems.Remove(this);
    }
}