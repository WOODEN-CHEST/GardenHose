using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Test;


internal class TestEntityPart : PhysicalEntityPart
{
    // Constructors.
    internal TestEntityPart(ICollisionBound[] bounds, WorldMaterial material, TestEntity entity) 
        : base(bounds, material, entity)
    {
        _damageParticleSettings = new(WorldMaterial.Test, () => Entity!.World!.Game.AssetManager.ParticleTest)
        {
            LifetimeMin = 6f,
            LifetimeMax = 8f,

            ScaleMin = 0.2f,
            ScaleMax = 0.4f,

            CollisionRadius = 6f,

            FadeInTime = 0.25f,
            FadeOutTime = 1f,

            ScaleChangePerSecondMin = 0.06f,
            ScaleChangePerSecondMax = 0.1f,

            AngularMotionMin = -1f,
            AngularMotionMax = 1f
        };
    }


    // Private fields.
    ParticleSettings _damageParticleSettings;

    // Inherited methods.
    protected override void OnPartDestroy()
    {
        if (IsMainPart)
        {
            Entity.Delete();
            return;
        }

        ParentLink!.ParentPart.UnlinkPart(this);
        ParticleEntity.CreateParticles(Entity.World!, _damageParticleSettings,
            new Range(8, 16), Position, Entity.Motion, 0.2f, MathHelper.PiOver4, Entity);
    }

    protected override void OnPartDamage()
    {
        ParticleEntity.CreateParticles(Entity.World!, _damageParticleSettings,
            new Range(1, 1), Position, Entity.Motion, 0.2f, MathHelper.PiOver4, Entity);
    }

    protected override void OnPartBreakOff()
    {

    }
}