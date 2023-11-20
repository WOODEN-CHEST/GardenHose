using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;


namespace GardenHose.Game.World.Entities;


internal class ParticlePart : PhysicalEntityPart
{
    // Internal fields.
    internal SpriteItem Sprite { get; set; }

    internal bool IsKilledByPlanets { get; set; } = true;

    internal float Lifetime { get; set; } = 4f;

    internal float RandomAdditionalLifetime { get; set; } = 2f;

    internal Vector2 ParticleScale { get; set; }


    // Constructors.
    public ParticlePart(ParticleEntity entity, ParticleSettings settings) : base(settings.Material, entity)
    {
        ParticleScale = new(settings.GetScale());
        CollisionBounds = new ICollisionBound[] { new BallCollisionBound(settings.Radius) };
    }


    // Inherited methods.
    internal override void ApplyForce(Vector2 location, float forceAmount) { }

    internal override void Draw()
    {
        base.Draw();

        Sprite.Scale.Vector = ParticleScale * Entity.World!.Zoom;
        Sprite.Position.Vector = Entity.World.ToViewportPosition(Position);
        Sprite.Draw();
    }

    internal override void Tick()
    {
        base.Tick();
        Sprite.ActiveAnimation.Update();
    }

    protected override void OnBreakPart() { }

    protected override void OnPartDamage() { }
}