using GardenHose.Engine.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Engine.Frame.UI.Item;

public class SpriteButton : SpriteItem, IUpdateableItem
{
    // Fields.
    public GameFrame ParentFrame;
    public bool Enabled = true;
    public ButtonComponent[] Components
    {
        get => _components;
        set
        {
            _components = value;
        }
    }

    public Action OnClickEvent
    {
        set => _onClickEvent = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Action OnReleaseEvent
    {
        set => _onReleaseEvent = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Action OnHoverEvent
    {
        set => _onHoverEvent = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Action OnUnHoverEvent
    {
        set => _onUnHoverEvent = value ?? throw new ArgumentNullException(nameof(value));
    }





    // Private fields.
    private ButtonComponent[] _components;

    private MouseEventListener _clickListener;
    private Action _onClickEvent;

    private MouseEventListener _releaseListener;
    private Action _onReleaseEvent;

    private Action _onHoverEvent;
    private Action _onUnHoverEvent;


    // Constructors.
    public SpriteButton(
        Vector2 position,
        Vector2 size,
        float rotation,

        GameFrame parentFrame,
        params ButtonShape[] shape

        ) : base()
    {
        ParentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
        ParentFrame.UpdateableItems.Add(this);
    }


    // Methods.
    public void Update()
    {
        if (!Enabled) return;
    }


    // Inherited methods.
    public void Dispose()
    {
        _clickListener?.StopListening();
        _releaseListener?.StopListening();
        ParentFrame.UpdateableItems.Remove(this);
    }
}
