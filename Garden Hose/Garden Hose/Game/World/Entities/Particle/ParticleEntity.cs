using GardenHose.Game.World.Entities.Physical;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities.Particle;

internal sealed class ParticleEntity : PhysicalEntity
{
    // Internal fields.
    internal float Lifetime { get; set; }
    internal float TimeAlive { get; set; } = 0f;
    internal float FadeStatus { get; private set; } = 0f;


    // Private fields.
    private float _fadeInSpeed;
    private float _fadeOutSpeed;
    private const float FADED_OUT = 0f;
    private const float FADED_IN = 1f;


    // Constructors.
    private ParticleEntity(ParticleSettings settings, Vector2 motion, Vector2 position) 
        : base(EntityType.Particle)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        MainPart = new ParticlePart(this, settings);

        Position = position;
        Motion = motion;
        Rotation = settings.GetRotation();
        AngularMotion = settings.GetAngularMotion();
        _fadeInSpeed = 1f / settings.FadeInTime;
        _fadeOutSpeed = 1f / settings.FadeOutTime;
        Lifetime = settings.GetLifetime();

        if (float.IsInfinity(_fadeInSpeed))
        {
            FadeStatus = FADED_IN;
        }

        IsInvulnerable = true;
        CommonMath.IsCalculated = false;
        CollisionHandler = new ParticleCollisionHandler(this);

        ZIndex = ZINDEX_PARTICLE;
    }

    private ParticleEntity() : base(EntityType.Particle)
    {
        CommonMath = new(this);
        CollisionHandler = new ParticleCollisionHandler(this);
        CommonMath.IsCalculated = false;
    }


    // Internal static methods.
    internal static ParticleEntity[] CreateParticles(GameWorld world,
        ParticleSettings settings,
        Range count,
        Vector2 position,
        Vector2 motion,
        float motionMagnitudeRandomness,
        float motionDirectionSpread,
        PhysicalEntity? sourceEntity)
    {
        ParticleEntity[] Particles = new ParticleEntity[Random.Shared.Next(count.Start.Value, count.End.Value + 1)];

        Vector2 MotionNormal = Vector2.Zero;
        if (motion.LengthSquared() is not 0f or not -0f)
        {
            MotionNormal = Vector2.Normalize(motion);
        }

        for (int i = 0; i < Particles.Length; i++)
        {
            float Rotation = (Random.Shared.NextSingle() - 0.5f) * motionDirectionSpread;
            Vector2 Motion = Vector2.Transform(MotionNormal, Matrix.CreateRotationZ(Rotation))
                * MathF.Pow(motion.Length(), (Random.Shared.NextSingle() - 0.5f) * 2f * motionMagnitudeRandomness);

            ParticleEntity Particle = new(settings, Motion, position);
            world.AddEntity(Particle);
            Particles[i] = Particle;

            if (sourceEntity != null)
            {
                Particle.CollisionHandler.AddCollisionIgnorable(sourceEntity);
            }
        }

        return Particles;
    }


    // Inherited methods.
    [TickedFunction(false)]
    internal override void Tick(GHGameTime time)
    {
        base.Tick(time);

        TimeAlive += time.WorldTime.PassedTimeSeconds;

        if ((FadeStatus < FADED_IN) && (TimeAlive < Lifetime))
        {
            FadeStatus = Math.Clamp(FadeStatus + _fadeInSpeed * time.WorldTime.PassedTimeSeconds, FADED_OUT, FADED_IN);
            return;
        }

        if (TimeAlive >= Lifetime)
        {
            FadeStatus = Math.Clamp(FadeStatus - _fadeOutSpeed * time.WorldTime.PassedTimeSeconds, FADED_OUT, FADED_IN);
        }

        if (FadeStatus <= FADED_OUT)
        {
            Delete();
        }
    }


    // Inherited methods.
    internal override Entity CloneDataToObject(Entity newEntity)
    {
        base.CloneDataToObject(newEntity);

        ParticleEntity Particle = (ParticleEntity)newEntity;

        Particle.Lifetime = Lifetime;
        Particle.TimeAlive = TimeAlive;
        Particle.FadeStatus = FadeStatus;
        Particle._fadeInSpeed = _fadeInSpeed;
        Particle._fadeOutSpeed = _fadeOutSpeed;

        return Particle;
    }

    internal override Entity CreateClone()
    {
        return CloneDataToObject(new ParticleEntity());
    }
}