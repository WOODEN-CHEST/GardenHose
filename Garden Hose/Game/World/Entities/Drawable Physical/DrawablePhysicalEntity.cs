using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHoseServer.World;
using GardenHoseServer.World.Entities;
using Microsoft.Xna.Framework.Graphics;


namespace GardenHose.Game.World.Entities;

internal abstract class DrawablePhysicalEntity : PhysicalEntity, IDrawableItem
{
    // Fields.
    public virtual bool IsVisible { get; set; } = true;
    public virtual Effect? Shader { get; set; }


    // Protected fields.
    protected Effect? AppliedShader;


    // Constructors.
    public DrawablePhysicalEntity(EntityType type, GameWorld world, ILayer layer) : base(type, world)
    {
        layer.AddDrawableItem(this);
    }

    // Inherited methods.
    public abstract void Draw(float passedTimeSeconds, SpriteBatch spriteBatch);
}
