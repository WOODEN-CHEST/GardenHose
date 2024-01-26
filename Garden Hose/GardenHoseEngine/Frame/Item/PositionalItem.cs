using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHoseEngine.Frame.Item;

public class PositionalItem : IDrawableItem
{
    // Fields.
    public virtual bool IsVisible { get; set; } = true;

    public virtual Effect? Shader { get; set; }

    public virtual Vector2 Position { get; set; } = Vector2.Zero;

    public virtual float Rotation { get; set; } = 0f;



    // Inherited methods.
    public virtual void Draw(IDrawInfo info) { }
}