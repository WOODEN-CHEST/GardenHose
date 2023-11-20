using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Material;
using GardenHoseEngine.Frame.Animation;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities;

internal class ParticleEntity : PhysicalEntity
{
    // Internal fields.
    bool IsKilledByPlanets
    {
        get => ((ParticlePart)MainPart).IsKilledByPlanets;
        set
        {
            ((ParticlePart)MainPart).IsKilledByPlanets = value;
        }
    }

    internal float Lifetime { get; set; }

    internal float TimeAlive { get; set; } = 0f;


    // Private fields.
    private Func<SpriteAnimation> _animationProvider;
    private Color _particleColorMask;
    private float _particleScale;


    // Constructors.
    public ParticleEntity(GameWorld? world, ParticleSettings settings) : base(EntityType.Particle, world)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        MainPart = new ParticlePart(this, settings);

        SelfPosition = settings.Position;
        Motion = settings.GetMotion();
        SelfRotation = settings.GetRotation();
        AngularMotion = settings.GetAngularMotion();
        _particleScale = settings.GetScale();
        _animationProvider = settings.AnimationProvider;
        _particleColorMask = settings.GetColor();
        Lifetime = settings.GetLifetime();

        MainPart.SetPositionAndRotation(Position, Rotation);
    }


    // Internal static methods.
    internal static ParticleEntity[] CreateParticles(GameWorld world, ParticleSettings settings)
    {
        ParticleEntity[] Particles = new ParticleEntity[settings.GetCount()];

        for (int i = 0; i < Particles.Length; i++)
        {
            ParticleEntity Particle = new(world, settings);
            world.AddEntity(Particle);
            Particles[i] = Particle;
        }

        return Particles;
    }


    // Inherited methods.
    internal override void Tick()
    {
        base.Tick();

        TimeAlive += World!.PassedTimeSeconds;
        if (TimeAlive >= Lifetime)
        {
            Delete();
        }
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        ParticlePart Part = (ParticlePart)MainPart;

        Part.Sprite = new(_animationProvider.Invoke());
        Part.Sprite.Mask = _particleColorMask;
        Part.Sprite.Scale.Vector = new(_particleScale);
    }

    internal override void OnCollision(PhysicalEntity otherEntity,
        PhysicalEntityPart selfPart,
        PhysicalEntityPart otherPart,
        ICollisionBound selfBound,
        ICollisionBound otherBound,
        Vector2 surfaceNormal,
        Vector2 collisionPoint)
    {
        if (IsKilledByPlanets && otherEntity.EntityType == EntityType.Planet)
        {
            Delete();
        }
    }
}