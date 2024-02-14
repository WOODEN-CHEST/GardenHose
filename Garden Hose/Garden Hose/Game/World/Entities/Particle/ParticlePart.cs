﻿using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHoseEngine.Frame.Item;
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

    private PartSprite _particleSprite;


    // Constructors.
    internal ParticlePart(ParticleEntity? entity, ParticleSettings settings) 
        : base(settings.Material, entity)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _baseSize = settings.GetSize();
        _scaleChangeSpeed = settings.GetScaleChangePerSecond();

        if (settings.CollisionRadius > 0f)
        {
            CollisionBounds = new ICollisionBound[] { new BallCollisionBound(settings.CollisionRadius) };
        }

        _particleSprite = new PartSprite(settings.AnimationName) { Size = _baseSize };
        AddSprite(_particleSprite);
    }


    // Inherited methods.
    [TickedFunction(false)]
    internal override void Tick(GHGameTime time)
    {
        base.Tick(time);

        _scale = Math.Max(_scale + (time.WorldTime.PassedTimeSeconds * _scaleChangeSpeed), MIN_SCALE);
    }

    internal override void Draw(IDrawInfo info)
    {
        _particleSprite.Opacity = ((ParticleEntity)Entity!).FadeStatus;
        _particleSprite.Size = _baseSize * _scale;
        base.Draw(info);
    }

    protected override object CopyInfoToNewObject(PhysicalEntityPart newPart)
    {
        base.CopyInfoToNewObject(newPart);

        ParticlePart Particle = (ParticlePart)newPart;
        Particle._scale = _scale;
        Particle._baseSize = _baseSize;
        Particle._scaleChangeSpeed = _scaleChangeSpeed;

        return newPart;
    }

    public override object Clone()
    {
        return CopyInfoToNewObject(new ParticlePart(Entity as ParticleEntity, _settings));
    }
}