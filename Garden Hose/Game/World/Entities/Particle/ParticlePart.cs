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
    internal SpriteItem Sprite { get; set; }

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
    }


    // Inherited methods.
    [TickedFunction(false)]
    internal override void Draw()
    {
        Sprite.Scale.Vector = ParticleScale * Entity.World!.Zoom;
        Sprite.Position.Vector = Entity.World.ToViewportPosition(Position);
        Sprite.Opacity = ((ParticleEntity)Entity).FadeStatus;
        Sprite.Rotation = CombinedRotation;
        Sprite.Draw();

        base.Draw();
    }

    [TickedFunction(false)]
    internal override void ParallelTick()
    {
        base.ParallelTick();
        Sprite.ActiveAnimation.Update();
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        Sprite = new(assetManager.GetAnimation(_settings.AnimationName)!);
        Sprite.Mask = _settings.GetColor();
        Sprite.Scale.Vector = ParticleScale;
    }
}