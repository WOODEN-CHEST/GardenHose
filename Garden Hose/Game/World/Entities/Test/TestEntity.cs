using GardenHose;
using GardenHose.Game.World.Entities;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item.Basic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseServer.World.Entities;

internal class TestEntity : DrawablePhysicalEntity
{
    // Private fields
    private Line VisualLine;


    // Constructors
    public TestEntity(GameWorld world, ILayer layer) : base(EntityType.Test, world, layer)
    {
        VisualLine = new(GH.Engine.Display, GH.Engine.SinglePixel)
        {
            Thickness = 10,
            Length = 10,
        };
    }

    // Inherited methods.
    public override void Draw(float passedTimeSeconds, SpriteBatch spriteBatch)
    {
        VisualLine.Position.Vector = (Position * World.Zoom) + World.ObjectVisualOffset;
        VisualLine.Scale.Vector.X = World.Zoom;
        VisualLine.Scale.Vector.Y = World.Zoom;
        VisualLine.Draw(passedTimeSeconds, spriteBatch);
    }

    internal override void Delete()
    {

    }
}