﻿using GardenHose.Game.World.Material;
using System;

namespace GardenHose.Game.World.Entities.Test;


internal class TestEntityPart : PhysicalEntityPart
{
    // Constructors.
    internal TestEntityPart(ICollisionBound[] bounds, WorldMaterial material, TestEntity entity) 
        : base(bounds, material, entity)
    {
    }


    // Inherited methods.
    protected override void OnBreakPart()
    {
        if (IsMainPart)
        {
            Entity.Delete();
            return;
        }

        ParentLink!.ParentPart.UnlinkPart(this);
    }

    protected override void OnPartDamage()
    {
        //ParticleSettings Settings = new(WorldMaterial.Test, () => Entity!.World!.Game.AssetManager.ParticleTest)
        //{
        //    CountMin = 8,
        //    CountMax = 16,

        //    Lifetime = 6f,
        //    RandomLifetimeBonus = 3f,

        //    Motion = Entity.Motion,
        //    RandomMotionBonus = 20f,
        //    MotionDirectionRandomness = MathF.PI / 4f,
        //    Position = Position,
        //    Scale = 0.3f,
        //    RandomScaleBonus = 0.125f,
        //};

        //ParticleEntity.CreateParticles(Entity.World!, Settings);
    }
}