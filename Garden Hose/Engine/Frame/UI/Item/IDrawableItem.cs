using Microsoft.Xna.Framework.Graphics;

namespace GardenHose.Engine.Frame.UI.Item;

public interface IDrawableItem
{
    // Properties.
    public bool IsVisible { get; set; }
    public Effect Shader { get; set; }


    // Methods.
    public void Draw();
}