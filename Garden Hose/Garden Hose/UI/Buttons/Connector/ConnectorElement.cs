using GardenHoseEngine.Frame.Item;
using GardenHoseEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GardenHoseEngine.Frame;
using System;

namespace GardenHose.UI.Buttons.Connector;

internal abstract partial class ConnectorElement : ITimeUpdatable, IDrawableItem
{
    // Fields.
    public bool IsVisible { get; set; } = true;
    public Effect? Shader { get; set; }


    // Internal static fields.
    internal static readonly FloatColor UNSELECTED_COLOR = new(0.4f, 0.4f, 0.4f, 1f);
    internal static readonly FloatColor SELECTED_COLOR = new(0.737f, 0.08f, 0.768f, 1f);
    internal static readonly FloatColor DEFAULT_CLICKED_COLOR = new(0.458f, 0.631f, 1f, 1f);

    internal static readonly Color TEXT_SHADOW_COLOR = new(20, 20, 20, 255);
    internal static readonly Vector2 CONNECTOR_SIZE = new(79f, 142f);
    internal static readonly Vector2 TEXT_SHADOW_OFFSET = new(0.08f);


    // Internal fields.
    internal virtual bool IsFunctional { get; set; } = true;
    internal virtual Vector2 Position { get; set; } = Vector2.Zero;
    internal virtual float Scale { get; set; } = 1.0f;


    // Constructors.
    internal ConnectorElement() { }


    // Static internal methods.
    internal static float GetConnectorAngle(Direction direction)
    {
        return direction switch
        {
            Direction.Right => 0f,
            Direction.Left => MathF.PI,
            Direction.Up => -(MathF.PI / 2),
            Direction.Down => MathF.PI / 2f,
            _ => throw new EnumValueException(nameof(direction), direction)
        };
    }

    internal static Vector2 GetDirectionVector(Direction direction)
    {
        return direction switch
        {
            Direction.Left => new(-1f, 0f),
            Direction.Right => new(1f, 0f),
            Direction.Up => new(0f, -1f),
            Direction.Down => new(0f, 1f),
            _ => throw new EnumValueException(nameof(direction), direction),
        };
    }


    // Inherited methods.
    public virtual void Draw(IDrawInfo info) { }

    public virtual void Update(IProgramTime programTime) { }
}