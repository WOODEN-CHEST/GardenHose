using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Events;
using GardenHoseEngine.Frame.Animation;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities.Particle;

internal class ParticleEntity : PhysicalEntity
{
    // Internal fields.
    bool IsKilledByPlanets { get; set; } = true;

    internal float Lifetime { get; set; }

    internal float TimeAlive { get; set; } = 0f;

    internal float FadeStatus { get; private set; } = 0f;


    // Private fields.
    private float _scaleChangeSpeed;

    private readonly float _fadeInSpeed;
    private readonly float _fadeOutSpeed;
    private const float FADED_OUT = 0f;
    private const float FADED_IN = 1f;


    // Constructors.
    protected ParticleEntity(GameWorld? world, ParticleSettings settings, Vector2 motion, Vector2 position) 
        : base(EntityType.Particle, world)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        MainPart = new ParticlePart(this, settings);

        SelfPosition = position;
        Motion = motion;
        SelfRotation = settings.GetRotation();
        AngularMotion = settings.GetAngularMotion();
        _fadeInSpeed = 1f / settings.FadeInTime;
        _fadeOutSpeed = 1f / settings.FadeOutTime;
        _scaleChangeSpeed = settings.GetScaleChangePerSecond();
        Lifetime = settings.GetLifetime();

        if (float.IsInfinity(_fadeInSpeed))
        {
            FadeStatus = FADED_IN;
        }

        IsInvulnerable = true;
        MainPart.SetPositionAndRotation(Position, Rotation);
    }


    // Internal static methods.
    internal static ParticleEntity[] CreateParticles(GameWorld world,
        ParticleSettings settings,
        Range count,
        Vector2 position,
        Vector2 motion,
        float motionMagnitudeRandomness,
        float motionDirectionRandomnessAngle,
        PhysicalEntity? sourceEntity = null)
    {
        ParticleEntity[] Particles = new ParticleEntity[Random.Shared.Next(count.Start.Value, count.End.Value + 1)];

        Vector2 MotionNormal = Vector2.Zero;
        if (motion.LengthSquared() is not 0f or not -0f)
        {
            MotionNormal = Vector2.Normalize(motion);
        }

        for (int i = 0; i < Particles.Length; i++)
        {
            float Rotation = (Random.Shared.NextSingle() - 0.5f) * motionDirectionRandomnessAngle;
            float MotionMagnitudeMultiplier = 1f + (Random.Shared.NextSingle() - 0.5f) * motionMagnitudeRandomness;
            Vector2 Motion = Vector2.Transform(MotionNormal, Matrix.CreateRotationZ(Rotation)) * motion.Length() * MotionMagnitudeMultiplier;

            ParticleEntity Particle = new(world, settings, Motion, position);
            world.AddEntity(Particle);
            Particles[i] = Particle;
        }

        if (sourceEntity != null)
        {
            foreach (ParticleEntity Particle in Particles)
            {
                Particle.AddCollisionIgnorable(sourceEntity);
            }
        }

        return Particles;
    }


    // Inherited methods.
    [TickedFunction(false)]
    internal override void ParallelTick()
    {
        base.ParallelTick();

        TimeAlive += World!.PassedTimeSeconds;

        ((ParticlePart)MainPart).ParticleScale *= 1 + (World!.PassedTimeSeconds * _scaleChangeSpeed);

        if ((FadeStatus < FADED_IN) && (TimeAlive < Lifetime))
        {
            FadeStatus = Math.Clamp(FadeStatus + _fadeInSpeed * World!.PassedTimeSeconds, 0f, 1f);
        }
        else if (TimeAlive >= Lifetime)
        {
            FadeStatus = Math.Clamp(FadeStatus - _fadeOutSpeed * World!.PassedTimeSeconds, 0f, 1f);

            if (FadeStatus <= FADED_OUT)
            {
                Delete();
            }
        }
    }

    internal override void OnCollision(PhysicalEntity otherEntity,
        PhysicalEntityPart selfPart,
        PhysicalEntityPart otherPart,
        Vector2 selfMotion,
        Vector2 otherMotion,
        Vector2 selfRotationalMotionAtPoint,
        Vector2 otherRotationalMotionAtPoint,
        Vector2 surfaceNormal,
        Vector2 collisionPoint)
    {
        if (IsKilledByPlanets && (otherEntity.EntityType == EntityType.Planet) && (TimeAlive < Lifetime))
        {
            TimeAlive = Lifetime;
            FadeStatus = FADED_IN;
        }
    }
}