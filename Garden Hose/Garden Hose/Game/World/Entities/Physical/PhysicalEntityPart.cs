using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace GardenHose.Game.World.Entities.Physical;


internal class PhysicalEntityPart
{
    // Fields.
    internal virtual PhysicalEntity? Entity
    {
        get => _entity;
        set
        {
            _entity = value;
            foreach (PartLink Link in SubPartLinks)
            {
                Link.LinkedPart.Entity = value;
            }
        }
    }


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
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value == _collisionBounds)
            {
                return;
            }

            _collisionBounds = value;
            Entity.ResetPartInfo();
            CollisionBoundChange?.Invoke(this, _collisionBounds);
        }
    }

    internal bool IsMainPart => Entity?.MainPart == this;

    internal virtual PartLink[] SubPartLinks { get; private set; } = Array.Empty<PartLink>();

    internal virtual PartLink? ParentLink { get; private set; }

    /* Material. */
    internal virtual WorldMaterialInstance MaterialInstance { get; set; }

    /* Drawing. */
    internal PartSprite[] Sprites => _sprites.ToArray();

    /* Events. */
    internal event EventHandler<CollisionEventArgs>? Collision;

    internal event EventHandler<ICollisionBound[]?>? CollisionBoundChange;

    internal event EventHandler<PartLink?>? ParentChange;

    internal event EventHandler<PartLink[]?>? SubPartChange;


    // Private fields.
    private readonly List<PartSprite> _sprites = new();

    private ICollisionBound[] _collisionBounds;
    private float _selfRotation = 0f;

    private PhysicalEntity? _entity = null;


    // Constructors.
    internal PhysicalEntityPart(ICollisionBound[]? collisionBounds, WorldMaterial material, PhysicalEntity? entity)
        : this(collisionBounds, material.CreateInstance(), entity) { }

    internal PhysicalEntityPart(WorldMaterial material, PhysicalEntity? entity)
        : this(null, material, entity) { }

    internal PhysicalEntityPart(WorldMaterialInstance material, PhysicalEntity? entity)
        : this(null, material, entity) { }

    internal PhysicalEntityPart(ICollisionBound[]? collisionBounds, 
        WorldMaterialInstance material,
        PhysicalEntity? entity)
    {
        Entity = entity;
        CollisionBounds = collisionBounds ?? Array.Empty<ICollisionBound>();
        MaterialInstance = material;
    }


    // Internal methods.
    internal virtual void Load(GHGameAssetManager assetManager)
    {
        MaterialInstance.Material.Load(assetManager);

        foreach (PartSprite Sprite in _sprites)
        {
            Sprite.Load(assetManager);
            Sprite.SetActiveSprite(MaterialInstance.Stage);
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
            throw new InvalidOperationException("Linked part already has a parent link.");
        }

        var NewPartLinks = new PartLink[SubPartLinks.Length + 1];
        SubPartLinks.CopyTo(NewPartLinks, 0);

        PartLink Link = new PartLink(this, part, distance, strength);
        NewPartLinks[^1] = Link;
        SubPartLinks = NewPartLinks;

        part.ParentLink = Link;
        part.Entity = Entity;

        Entity?.ResetPartInfo();
        SubPartChange?.Invoke(this, SubPartLinks);
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

        Entity?.ResetPartInfo();
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
        SelfRotation += AngularMotion * time.WorldTime.PassedTimeSeconds;

        foreach (PartLink Link in SubPartLinks)
        {
            Link.LinkedPart.Tick(time);
        }

        MaterialInstance.HeatByTouch(Entity!.World!.AmbientMaterial, time);
        MaterialInstance.Update(time, !Entity.IsInvulnerable);

        if (MaterialInstance.Material.Attraction > 0f)
        {
            AttractEntities(time);
        }
    }


    /* Drawing. */
    internal virtual void Draw(IDrawInfo info)
    {
        foreach (PartLink Link in SubPartLinks)
        {
            Link.LinkedPart.Draw(info);
        }

        foreach (PartSprite Sprite in _sprites)
        {
            Sprite.Draw(info, Entity!.World!.Player.Camera, this);
        }
    }

    internal virtual void DrawCollisionBounds(IDrawInfo info)
    {
        if (CollisionBounds.Length == 0)
        {
            return;
        }

        foreach (ICollisionBound Bound in CollisionBounds!)
        {
            Bound.Draw(Position, CombinedRotation, info, Entity!.World!.Player.Camera);
        }
    }

    internal void AddSprite(PartSprite sprite)
    {
        if (sprite == null)
        {
            throw new ArgumentNullException(nameof(sprite));
        }

        _sprites.Add(sprite);
    }

    internal void RemoveSprite(PartSprite sprite)
    {
        if (sprite == null)
        {
            throw new ArgumentNullException(nameof(sprite));
        }

        _sprites.Remove(sprite);
    }

    // Protected methods.
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
                / Vector2.Distance(Position, PhysicalWorldEntity.Position)) * time.WorldTime.PassedTimeSeconds;

            if (float.IsNaN(AttractionStrength) || !float.IsFinite(AttractionStrength))
            {
                AttractionStrength = 0f;
            }

            Vector2 AddedMotion = Position - PhysicalWorldEntity.Position;
            if (AddedMotion.LengthSquared() is 0f or -0f)
            {
                AddedMotion = -Vector2.UnitY;
            }
            AddedMotion.Normalize();

            AddedMotion *= AttractionStrength;

            PhysicalWorldEntity.Motion += AddedMotion;
        }
    }
}