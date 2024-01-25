using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHoseEngine.Frame.Item;

public class PositionalItem : IDrawableItem
{
    // Fields.
    public virtual bool IsVisible { get; set; }

    public virtual Effect? Shader { get; set; }

    public virtual Vector2 Position { get; private init; }

    public virtual float Rotation { get; set; } = 0f;



    // Constructors.
    public PositionalItem()
    {
        Position = Vector2.Zero;
    }


    // Inherited methods.
    public virtual void Draw(DrawInfo info) { }
}