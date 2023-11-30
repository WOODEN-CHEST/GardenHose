using GardenHose.Game.World.Material;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;


namespace GardenHose.Game.World.Entities;


internal class ParticlePart : PhysicalEntityPart
{
    // Internal fields.
    internal SpriteItem Sprite { get; set; }

    internal float Lifetime { get; set; } = 4f;

    internal float RandomAdditionalLifetime { get; set; } = 2f;

    internal Vector2 ParticleScale { get; set; }


    // Constructors.
    public ParticlePart(ParticleEntity entity, float scale, float radius, WorldMaterial material) : base(material, entity)
    {
        ParticleScale = new(scale);

        if (radius > 0)
        {
            CollisionBounds = new ICollisionBound[] { new BallCollisionBound(radius) };
        }
    }


    // Inherited methods.
    internal override void Draw()
    {
        Sprite.Scale.Vector = ParticleScale * Entity.World!.Zoom;
        Sprite.Position.Vector = Entity.World.ToViewportPosition(Position);
        Sprite.Opacity = ((ParticleEntity)Entity).FadeStatus;
        Sprite.Rotation = CombinedRotation;
        Sprite.Draw();

        base.Draw();
    }

    internal override void ParallelTick()
    {
        base.ParallelTick();
        Sprite.ActiveAnimation.Update();
    }

    protected override void OnPartDestroy() { }

    protected override void OnPartDamage() { }

    protected override void OnPartBreakOff() { }
}