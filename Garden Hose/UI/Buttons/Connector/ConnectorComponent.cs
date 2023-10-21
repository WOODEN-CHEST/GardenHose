using GardenHoseEngine.Frame.Item;
using GardenHoseEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GardenHose.UI.Buttons.Connector;

internal abstract partial class ConnectorElement : ITimeUpdatable, IDrawableItem
{
    // Fields.
    public bool IsVisible { get; set; } = true;

    public Effect? Shader { get; set; }


    // Internal fields.
    internal virtual bool IsEnabled { get; set; } = true;

    internal virtual AnimVector2 Position { get; private init; } = new();

    internal virtual Vector2 Scale { get; set; } = Vector2.One;


    // Constructors.
    internal ConnectorElement() { }
    

    // Inherited methods.
    public virtual void Draw()
    {

    }

    public virtual void Update()
    {

    }
}