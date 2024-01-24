using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Particle;


internal class ParticlePart : PhysicalEntityPart
{
    // Internal fields.
    internal float Lifetime { get; set; } = 4f;

    internal float RandomAdditionalLifetime { get; set; } = 2f;

    internal Vector2 ParticleScale { get; set; }


    // Private fields.
    private readonly ParticleSettings _settings;


    // Constructors.
    public ParticlePart(ParticleEntity entity, ParticleSettings settings) 
        : base(settings.Material, entity)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        ParticleScale = new(settings.GetScale());

        if (settings.CollisionRadius > 0)
        {
            CollisionBounds = new ICollisionBound[] { new BallCollisionBound(settings.CollisionRadius) };
        }

        AddSprite(new(settings.AnimationName) 
        { 
            ColorMask = settings.GetColor(), 
            Scale = ParticleScale
        });
    }


    // Inherited methods.
    [TickedFunction(false)]
    internal override void Draw()
    {
        Sprites[0].Opacity = ((ParticleEntity)Entity).FadeStatus;
        Sprites[0].Scale = ParticleScale;
        base.Draw();
    }
}