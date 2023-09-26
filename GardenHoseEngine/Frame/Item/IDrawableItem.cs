using Microsoft.Xna.Framework.Graphics;

namespace GardenHoseEngine.Frame.Item;

public interface IDrawableItem
{
    // Fields.
    public const float DEFAULT_LAYER_DEPTH = 0f;


    // Properties.
    public bool IsVisible { get; set; }

    public Effect? Shader { get; set; }


    // Methods.
    public void Draw(float passedTimeSeconds, SpriteBatch spriteBatch);
}