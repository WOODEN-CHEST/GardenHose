using GardenHoseEngine.Frame.Item.Basic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities;

internal class BallCollisionBound : CollisionBound
{
    // Internal fields.
    internal float Radius { get; set; }


    // Constructors.
    public BallCollisionBound() : this(0f) { }

    public BallCollisionBound(float radius) : base(CollisionBoundType.Rectangle)
    {
        Radius = radius;
    }


    // Inherited methods.
    internal override void Draw(Line line, GameWorld world)
    {

    }
}
