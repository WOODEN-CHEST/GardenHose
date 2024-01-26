using Microsoft.Xna.Framework.Graphics;


namespace GardenHoseEngine.Frame.Item;


public interface IDrawInfo
{
    // Internal fields.
    public SpriteBatch SpriteBatch { get; }
    public IProgramTime Time { get; }
}