using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHose.Game.World;
using Microsoft.Xna.Framework.Graphics;
using GardenHoseEngine.Frame.Item.Basic;
using GardenHoseEngine;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace GardenHose.Game.World.Entities;

internal abstract class DrawablePhysicalEntity : PhysicalEntity, IDrawableItem
{
    // Fields.
    public virtual bool IsVisible { get; set; } = true;

    public virtual Effect? Shader { get; set; }


    // Internal fields.
    internal virtual DrawLayer Layer { get; set; } = DrawLayer.Bottom;

    internal bool DrawCollisionBox = false;


    // Protected fields.
    protected Line VisualLine { get; private init; } = new() { Thickness= 10f };


    // Constructors.
    public DrawablePhysicalEntity(EntityType type, GameWorld? world, CollisionBound[] collisionBounds) 
        : base(type, world, collisionBounds) { }


    // Inherited methods.
    public virtual void Draw()
    {
        if (DrawCollisionBox)
        {
            foreach (CollisionBound Bound in CollisionBounds)
            {
                Bound.Draw(VisualLine, World!);
            }
        }
    }
}
