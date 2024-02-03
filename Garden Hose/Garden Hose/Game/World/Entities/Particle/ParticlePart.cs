using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Particle;


internal class ParticlePart : PhysicalEntityPart
{
    // Private fields.
    private readonly ParticleSettings _settings;

    private Vector2 _baseSize;
    private float _scale = 1f;
    private float _scaleChangeSpeed;
    private const float MIN_SCALE = 0f;


    // Constructors.
    internal ParticlePart(ParticleEntity entity, ParticleSettings settings) 
        : base(settings.Material, entity)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _baseSize = settings.GetSize();
        _scaleChangeSpeed = settings.GetScaleChangePerSecond();

        if (settings.CollisionRadius > 0f)
        {
            CollisionBounds = new ICollisionBound[] { new BallCollisionBound(settings.CollisionRadius) };
        }

        PartSprite Sprite = new(settings.AnimationName) { Size = _baseSize };
        AddSprite(new PartSpriteCollection(Sprite, Sprite, Sprite, Sprite));
    }


    // Inherited methods.
    [TickedFunction(false)]
    internal override void Tick(GHGameTime time)
    {
        base.Tick(time);

        _scale = Math.Max(_scale + (time.WorldTime.PassedTimeSeconds * _scaleChangeSpeed), MIN_SCALE);
    }

    internal override void Draw()
    {
        Sprites[0].Opacity = ((ParticleEntity)Entity).FadeStatus;
        _sprites[0].Scale = ParticleScale;
        base.Draw();
    }
}