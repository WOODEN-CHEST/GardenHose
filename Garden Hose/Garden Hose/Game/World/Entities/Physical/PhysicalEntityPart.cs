using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Particle;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Entities.Physical.Events;
using GardenHose.Game.World.Entities.Stray;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace GardenHose.Game.World.Entities.Physical;


internal class PhysicalEntityPart
{
    // Fields.
    internal virtual PhysicalEntity Entity { get; set; }


    /* Part properties. */
    internal virtual Vector2 Position { get; private set; }

    internal virtual float SelfRotation
    {
        get => _selfRotation;
        set
        {
            _selfRotation = value;
            SetPositionAndRotation(Position, CombinedRotation); // Forces sub-parts to update.
        }
    }

    internal virtual float CombinedRotation { get; private set; }

    internal virtual float AngularMotion { get; set; }

    public virtual float Mass
    {
        get
        {
            float TotalMass = 0f;

            if (CollisionBounds != null)
            {
                foreach (ICollisionBound CBound in CollisionBounds)
                {
                    TotalMass += CBound.GetArea() * MaterialInstance.Material.Density;
                }
            }  

            return TotalMass;
        }
    }


    /* Collision bounds and parts. */
    internal virtual ICollisionBound[] CollisionBounds
    {
        get => _collisionBounds;
        set
        {
            if (value == _collisionBounds)
            {
                return;
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _collisionBounds = value;
            Entity.OnPartCollisionBoundChange();
            CollisionBoundChange?.Invoke(this, _collisionBounds);
        }
    }

    internal bool IsMainPart => ParentLink == null;

    internal virtual PartLink[] SubPartLinks { get; private set; } = Array.Empty<PartLink>();

    internal virtual PartLink? ParentLink
    {
        get => _parentLink;
        set
        {
            if (value == _parentLink)
            {
                return;
            }

            _parentLink = value;

            Entity.OnPartChange();
            ParentChange?.Invoke(this, _parentLink);
        }
    }

    /* Material. */
    internal virtual WorldMaterialInstance MaterialInstance { get; set; }

    /* Drawing. */
    internal PhysicalEntityPartSprite[] PartSprites => Sprites.ToArray();

    /* Events. */
    internal event EventHandler<CollisionEventArgs>? Collision;

    internal event EventHandler<ICollisionBound[]?>? CollisionBoundChange;

    internal event EventHandler<PartLink?>? ParentChange;

    internal event EventHandler<PartLink[]?>? SubPartChange;

    internal event EventHandler<PartDamageEventArgs>? PartDamage;

    internal event EventHandler<PartDamageEventArgs>? PartDestroy;

    internal event EventHandler<PartDamageEventArgs>? PartBreakOff;


    // Protected fields.
    protected readonly List<PhysicalEntityPartSprite> Sprites = new();


    // Private fields.
    private ICollisionBound[] _collisionBounds;
    private float _selfRotation = 0f;

    private PartLink? _parentLink;


    // Constructors.
    internal PhysicalEntityPart(ICollisionBound[] collisionBounds, WorldMaterial material, PhysicalEntity entity)
        : this(collisionBounds, material.CreateInstance(), entity) { }

    internal PhysicalEntityPart(WorldMaterial material, PhysicalEntity entity)
        : this(null, material, entity) { }

    internal PhysicalEntityPart(WorldMaterialInstance material, PhysicalEntity entity)
        : this(null, material, entity) { }

    internal PhysicalEntityPart(ICollisionBound[] collisionBounds, 
        WorldMaterialInstance material,
        PhysicalEntity entity)
    {
        Entity = entity;
        CollisionBounds = collisionBounds;
        MaterialInstance = material;
    }


    // Internal methods.
    internal virtual void Load(GHGameAssetManager assetManager)
    {
        MaterialInstance.Material.Load(assetManager);

        foreach (PhysicalEntityPartSprite Sprite in Sprites)
        {
            Sprite.Load(assetManager);
        }
    }

    /* Parts. */
    internal virtual void LinkPart(PhysicalEntityPart part, Vector2 distance)
    {
        LinkPart(part, distance, float.PositiveInfinity);
    }

    internal virtual void LinkPart(PhysicalEntityPart part, Vector2 distance, float strength)
    {
        if (part == null)
        {
            throw new ArgumentNullException(nameof(part));
        }

        if (part.ParentLink != null)
        {
            throw new InvalidOperationException("Linked part already has a parent.");
        }


        var NewPartLinks = new PartLink[SubPartLinks.Length + 1];
        SubPartLinks.CopyTo(NewPartLinks, 0);

        PartLink Link = new PartLink(this, part, Entity, distance, strength);
        NewPartLinks[^1] = Link;
        SubPartLinks = NewPartLinks;

        part.ParentLink = Link;

        Entity.OnPartChange();
        SubPartChange?.Invoke(this, SubPartLinks);
    }

    internal virtual void LinkEntityAsPart(PhysicalEntity entity, Vector2 distance)
    {
        LinkPart(entity.MainPart, distance);
    }

    internal virtual void LinkEntityAsPart(PhysicalEntity entity, Vector2 distance, float strength)
    {
        LinkPart(entity.MainPart, distance, strength);
    }

    internal virtual void UnlinkPart(PhysicalEntityPart part)
    {
        if (SubPartLinks.Length == 0)
        {
            return;
        }

        List<PartLink> Links = new(SubPartLinks);

        foreach (PartLink Link in Links)
        {
            if (Link.LinkedPart == part)
            {
                Links.Remove(Link);
                part.ParentLink = null;
                part.Entity = null;
                break;
            }
        }

        SubPartLinks = Links.ToArray();

        Entity.OnPartChange();
        SubPartChange?.Invoke(this, SubPartLinks);
    }

    internal virtual List<PartLink> GetPathFromMainPart()
    {
        List<PartLink> Links = new();
        PartLink? Link = ParentLink;

        while (Link != null)
        {
            Links.Insert(0, Link);
            Link = Link.ParentPart.ParentLink;
        }

        return Links;
    }

    /* Collision. */


    internal void OnCollision(Vector2 location, float appliedForce)
    {
        MaterialInstance.HeatByCollision(appliedForce);
        PartDamageEventArgs Args = new(this, location, appliedForce);

        // Sound.
        const float SOUND_FORCE_DOWNSCALE = 0.7f;
        if (appliedForce >= (MaterialInstance.Material.Resistance * SOUND_FORCE_DOWNSCALE))
        {

        }

        // Damage.
        if ((appliedForce >= MaterialInstance.Material.Resistance) && !Entity.IsInvulnerable)
        {
            MaterialInstance.CurrentStrength -= appliedForce;

            if (MaterialInstance.Stage == WorldMaterialStage.Destroyed)
            {
                OnPartDestroy(location, appliedForce);
                Entity.OnPartDestroy(Args);
                PartDestroy?.Invoke(this, Args);
            }
            else
            {
                OnPartDamage(location, appliedForce);
                Entity.OnPartDamage(Args);
                PartDamage?.Invoke(this, Args);
            }
        }

        if ((ParentLink != null) && (appliedForce >= ParentLink.LinkStrength) && !Entity.IsInvulnerable)
        {
            OnPartBreakOff(location, appliedForce);
            PartBreakOff?.Invoke(this, Args);
        }

        Collision?.Invoke(this, new(location, appliedForce));
    }

    /* Properties. */
    internal virtual void SetPositionAndRotation(Vector2 position, float rotation)
    {
        Position = position;
        CombinedRotation = SelfRotation + rotation;

        foreach (PartLink Link in SubPartLinks)
        {
            Link.LinkedPart.SetPositionAndRotation(
                Vector2.Transform(Link.LinkDistance, Matrix.CreateRotationZ(CombinedRotation)) + Position,
                CombinedRotation);
        }
    }

    /* Game flow. */
    [TickedFunction(false)]
    internal virtual void Tick(GHGameTime time)
    {
        SelfRotation += AngularMotion * time.PassedWorldTimeSeconds;

        foreach (PartLink Link in SubPartLinks)
        {
            Link.LinkedPart.Tick(time);
        }

        MaterialInstance.HeatByTouch(Entity.World!.AmbientMaterial, time);
        MaterialInstance.Update(time, !Entity.IsInvulnerable);

        if (MaterialInstance.Material.Attraction > 0f)
        {
            AttractEntities(time);
        }
    }


    /* Drawing. */
    internal virtual void Draw()
    {
        foreach (PartLink Link in SubPartLinks)
        {
            Link.LinkedPart.Draw();
        }

        foreach (PhysicalEntityPartSprite Sprite in Sprites)
        {
            Sprite.Sprite.Update();
            Sprite.Draw(Entity.World!, Position, CombinedRotation);
        }
    }

    internal virtual void DrawCollisionBounds()
    {
        if (CollisionBounds.Length == 0)
        {
            return;
        }

        foreach (ICollisionBound Bound in CollisionBounds!)
        {
            Bound.Draw(Position, CombinedRotation, Entity.World!);
        }
    }

    internal void AddSprite(PhysicalEntityPartSprite sprite)
    {
        if (sprite == null)
        {
            throw new ArgumentNullException(nameof(sprite));
        }

        Sprites.Add(sprite);
    }

    internal void RemoveSprite(PhysicalEntityPartSprite sprite)
    {
        if (sprite == null)
        {
            throw new ArgumentNullException(nameof(sprite));
        }

        Sprites.Remove(sprite);
    }

    // Protected methods.
    /* Collision. */



    /* Parts. */
    protected void OnPartDamage(Vector2 collisionLocation, float forceApplied)
    {
        CreateDamageParticles(collisionLocation, forceApplied);
    }

    protected void OnPartDestroy(Vector2 collisionLocation, float forceApplied)
    {
        CreateDamageParticles(collisionLocation, forceApplied);

        if (!IsMainPart)
        {
            ParentLink!.ParentPart.UnlinkPart(this);
            return;
        }

        foreach (PartLink Link in SubPartLinks)
        {
            StrayEntity Stray = new(Entity.World,
                Link.LinkedPart,
                Link.LinkedPart.Position,
                Entity.Motion,
                Entity.Rotation);

            Entity.World!.AddEntity(Stray);
        }

        Entity.Delete();
    }

    protected void OnPartBreakOff(Vector2 collisionLocation, float forceApplied)
    {
        CreateDamageParticles(collisionLocation, forceApplied);

        PhysicalEntity OldEntity = Entity;
        ParentLink!.ParentPart.UnlinkPart(this);

        StrayEntity Stray = new(OldEntity.World,
                this,
                Position,
                OldEntity.Motion,
                OldEntity.Rotation);
        Stray.AddCollisionIgnorable(OldEntity);
        Entity.World!.AddEntity(Stray); 
    }

    protected void CreateDamageParticles(Vector2 collisionLocation, float forceApplied)
    {
        if (MaterialInstance.Material.DamageParticles == null)
        {
            return;
        }

        const int MAX_PARTICLES = 20;
        const float FORCE_DIVIDER = 7500f;
        const float MOTION_MAGNITUDE_RANDOMNESS = 0.5f;
        const float PARTICLE_SPREAD = MathF.PI / 1.75f;
        int ParticleCount = Math.Min(MAX_PARTICLES, (int)(forceApplied / FORCE_DIVIDER));

        ParticleEntity.CreateParticles(Entity.World!,
            MaterialInstance.Material.DamageParticles,
            new Range(ParticleCount / 2, ParticleCount),
            collisionLocation,
            Entity.Motion,
            MOTION_MAGNITUDE_RANDOMNESS,
            PARTICLE_SPREAD,
            Entity);
    }


    /* Physics. */
    [TickedFunction(false)]
    protected void AttractEntities(GHGameTime time)
    {
        foreach (Entity WorldEntity in Entity!.World!.Entities)
        {
            if ((WorldEntity == Entity) || (!WorldEntity.IsPhysical)) continue;

            PhysicalEntity PhysicalWorldEntity = (PhysicalEntity)WorldEntity;

            if (!PhysicalWorldEntity.IsAttractable) continue;

            const float ARBITRARY_ATTRACTION_INCREASE = 1000f;
            float AttractionStrength = (MaterialInstance.Material.Attraction * ARBITRARY_ATTRACTION_INCREASE
                / Vector2.Distance(Position, PhysicalWorldEntity.Position)) * time.PassedWorldTimeSeconds;

            if (float.IsNaN(AttractionStrength) || !float.IsFinite(AttractionStrength))
            {
                AttractionStrength = 0f;
            }

            Vector2 AddedMotion = Vector2.Normalize(Position - PhysicalWorldEntity.Position);
            AddedMotion.X = (float.IsFinite(AddedMotion.X) || !float.IsNaN(AddedMotion.X)) ? AddedMotion.X : 0f;
            AddedMotion.Y = (float.IsFinite(AddedMotion.Y) || !float.IsNaN(AddedMotion.Y)) ? AddedMotion.Y : 0f;

            AddedMotion *= AttractionStrength;

            PhysicalWorldEntity.Motion += AddedMotion;
        }
    }
}