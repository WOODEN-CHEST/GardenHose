using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseServer.World.Entities;

internal class TestEntity : PhysicalEntity
{
    // Constructors.
    public TestEntity(GameWorld world) : base(EntityType.Test, world) { }


    // Inherited methods.
    internal override void Delete() { }

    internal override void Tick(float timePassedSeconds)
    {
        World.PhysicsEngine.Simulate(this, timePassedSeconds);
    }

    internal override EntityData GetDataForMessage()
    {
        return new TestEntityData(this);
    }
}