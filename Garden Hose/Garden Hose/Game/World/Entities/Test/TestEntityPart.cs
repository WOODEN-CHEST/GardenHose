using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Particle;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
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
        _damageParticleSettings = new(WorldMaterial.Test, "particle_test")
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
    protected override void OnPartDestroy(Vector2 collisionLocation, float forceAmount)
    {
        if (IsMainPart)
        {
            Entity.Delete();
            return;
        }

        ParentLink!.ParentPart.UnlinkPart(this);
        ParticleEntity.CreateParticles(Entity.World!, _damageParticleSettings,
            new Range(8, 16), collisionLocation, Entity.Motion, 0.2f, MathHelper.PiOver4, Entity);
    }

    protected override void OnPartDamage(Vector2 collisionLocation, float forceAmount)
    {
        ParticleEntity.CreateParticles(Entity.World!, _damageParticleSettings,
            new Range(4, 8), collisionLocation, Entity.Motion, 0.2f, MathHelper.PiOver4, Entity);
    }

    protected override void OnPartBreakOff(Vector2 collisionLocation, float forceAmount)
    {

    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        assetManager.GetAnimation(_damageParticleSettings.AnimationName);
    }
}