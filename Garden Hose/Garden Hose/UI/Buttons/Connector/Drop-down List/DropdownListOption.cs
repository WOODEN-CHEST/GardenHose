using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.UI.Buttons.Connector;

internal class DropDownListOption<OptionType> : IDrawableItem, ITimeUpdatable
{
    // Fields.
    public bool IsVisible
    {
        get => Button.IsVisible;
        set => Button.IsVisible = value;
    }

    public Effect? Shader
    {
        get => Button.Shader;
        set => Button.Shader = value;
    }


    // Internal fields.
    internal ConnectorRectangleButton Button { get; private init; }

    internal OptionType Value { get; set; }

    internal AnimVector2 Position => Button.Position;

    internal Vector2 Scale
    {
        get => Button.Scale;
        set => Button.Scale = value * s_buttonScale;
    }


    // Private static fields.
    private static readonly Vector2 s_buttonScale = new Vector2(0.25f, 0.25f);


    // Private fields.
    private readonly Action<DropDownListOption<OptionType>> _clickHandler;


    // Constructors.
    public DropDownListOption(OptionType value, Direction connectDirection, 
        Vector2 scale,
        Action<DropDownListOption<OptionType>> clickHandler)
    {
        Value = value;
        Button = ConnectorElement.CreateNormalButton(connectDirection, Vector2.Zero, scale * s_buttonScale);
        Button.ClickHandler = OnClickEvent;
        Button.IsClickingResetOnClick = false;
        _clickHandler = clickHandler ?? throw new ArgumentNullException(nameof(clickHandler));
    }


    // Internal methods.
    internal void OnClickEvent(object? sender, EventArgs args) => _clickHandler.Invoke(this);

    public void Update()
    {
        Button.Update();
    }

    public void Draw()
    {
        Button.Draw();
    }
}