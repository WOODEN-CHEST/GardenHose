using GardenHose.Game.AssetManager;
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
    internal virtual ICollisionBound[]? CollisionBounds
    {
        get => _collisionBounds;
        set
        {
            if (value == _collisionBounds)
            {
                return;
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
    private ICollisionBound[]? _collisionBounds;
    private float _selfRotation = 0f;

    private PartLink? _parentLink;


    // Constructors.
    internal PhysicalEntityPart(ICollisionBound[]? collisionBounds, WorldMaterial material, PhysicalEntity entity)
        : this(collisionBounds, material.CreateInstance(), entity) { }

    internal PhysicalEntityPart(WorldMaterial material, PhysicalEntity entity)
        : this(null, material, entity) { }

    internal PhysicalEntityPart(WorldMaterialInstance material, PhysicalEntity entity)
        : this(null, material, entity) { }

    internal PhysicalEntityPart(ICollisionBound[]? collisionBounds, 
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
    internal virtual CollisionCase[] TestCollisionAgainstEntity(PhysicalEntity entity)
    {
        if (CollisionBounds == null || MaterialInstance.State == WorldMaterialState.Gas)
        {
            return Array.Empty<CollisionCase>();
        }

        foreach (ICollisionBound CollisionBound in CollisionBounds)
        {
            return TestBoundAgainstEntity(CollisionBound, entity);
        }

        return Array.Empty<CollisionCase>();
    }

    internal (Vector2[] points, Vector2 surfaceNormal)? TestBoundAgainstBound(ICollisionBound selfBound,
        ICollisionBound targetBound, PhysicalEntityPart targetPart)
    {
        if (selfBound.Type == CollisionBoundType.Rectangle && targetBound.Type == CollisionBoundType.Rectangle)
        {
            return GetCollisionPointsRectToRect((RectangleCollisionBound)selfBound, (RectangleCollisionBound)targetBound, targetPart);
        }
        else if (selfBound.Type == CollisionBoundType.Ball && targetBound.Type == CollisionBoundType.Rectangle)
        {
            return GetCollisionPointsRectToBall((RectangleCollisionBound)targetBound,
                targetPart, (BallCollisionBound)selfBound, this);
        }
        else if (selfBound.Type == CollisionBoundType.Rectangle && targetBound.Type == CollisionBoundType.Ball)
        {
            return GetCollisionPointsRectToBall((RectangleCollisionBound)selfBound,
                this, (BallCollisionBound)targetBound, targetPart);
        }
        else if (selfBound.Type == CollisionBoundType.Ball && targetBound.Type == CollisionBoundType.Ball)
        {
            return GetCollisionPointsBallToBall((BallCollisionBound)selfBound, (BallCollisionBound)targetBound, targetPart);
        }
        else
        {
            throw new NotSupportedException("Unknown bound types, cannot test collision. " +
               $"Bound type 1: \"{selfBound}\" (int value of {(int)selfBound.Type}), " +
               $"Bound type 2: \"{targetBound}\" (int value of {(int)targetBound.Type}), ");
        }
    }

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
    internal virtual void ParallelTick()
    {
        SelfRotation += AngularMotion * Entity.World!.PassedTimeSeconds;

        foreach (PartLink Link in SubPartLinks)
        {
            Link.LinkedPart.ParallelTick();
        }

        MaterialInstance.HeatByTouch(Entity.World!.AmbientMaterial, Entity.World.PassedTimeSeconds);
        MaterialInstance.Update(Entity.World.PassedTimeSeconds, !Entity.IsInvulnerable);
    }

    [TickedFunction(false)]
    internal virtual void SequentialTick()
    {
        if (SubPartLinks != null)
        {
            foreach (PartLink Link in SubPartLinks)
            {
                Link.LinkedPart.SequentialTick();
            }
        }

        if (MaterialInstance.Material.Attraction > 0f)
        {
            AttractEntities();
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
        if (CollisionBounds == null)
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
    protected virtual CollisionCase[] TestBoundAgainstEntity(ICollisionBound selfBound,
        PhysicalEntity targetEntity)
    {
        List<CollisionCase> CollisionCases = new();

        foreach (PhysicalEntityPart TargetPart in targetEntity.Parts)
        {
            if (TargetPart.CollisionBounds == null || TargetPart.MaterialInstance.State == WorldMaterialState.Gas)
            {
                continue;
            }

            (Vector2[] points, Vector2 surfaceNormal)? CollisionData = null;

            foreach (ICollisionBound TargetBound in TargetPart.CollisionBounds) // At this point it is a 4 layer deep foreach loop...
            {
                //Test collision box bounds.
                float Radius1 = selfBound.GetRadius();
                float Radius2 = TargetBound.GetRadius();

                if (Vector2.Distance(Position + selfBound.Offset, TargetPart.Position + TargetBound.Offset) > Radius1 + Radius2)
                {
                    continue;
                }

                // Get collision data.
                CollisionData = TestBoundAgainstBound(selfBound, TargetBound, TargetPart);

                if (CollisionData == null)
                {
                    continue;
                }

                // Build case.
                CollisionCase Case = new(
                    Entity,
                    targetEntity,
                    this,
                    TargetPart,
                    selfBound,
                    TargetBound,
                    CollisionData.Value.surfaceNormal,
                    CollisionData.Value.points);
                CollisionCases.Add(Case);
            }
        }

        return CollisionCases.ToArray();
    }

    protected virtual (Vector2[] points, Vector2 surfaceNormal)? GetCollisionPointsRectToRect(RectangleCollisionBound rect1,
        RectangleCollisionBound rect2,
        PhysicalEntityPart rect2SourcePart)
    {
        // Prepare variables.
        Vector2[] VerticesRect1 = rect1.GetVertices(Position, CombinedRotation);
        Vector2[] VerticesRect2 = rect1.GetVertices(rect2SourcePart.Position, rect2SourcePart.CombinedRotation);
        Edge[] rect2Edges = new Edge[]
        {
            new Edge(VerticesRect2[0], VerticesRect2[1]),
            new Edge(VerticesRect2[1], VerticesRect2[2]),
            new Edge(VerticesRect2[2], VerticesRect2[3]),
            new Edge(VerticesRect2[3], VerticesRect2[0])
        };
        EquationRay[] rect2Rays = new EquationRay[]
        {
            new(rect2Edges[0]),
            new(rect2Edges[1]),
            new(rect2Edges[2]),
            new(rect2Edges[3]),
        };
        List<Vector2> CollisionPoints = new();

        // Find collision points.
        for (int EdgeIndex1 = 0; EdgeIndex1 < VerticesRect1.Length; EdgeIndex1++)
        {
            Edge PrimaryEdge = new Edge(VerticesRect1[EdgeIndex1], VerticesRect1[(EdgeIndex1 + 1) % VerticesRect1.Length]);
            EquationRay PrimaryEdgeRay = new EquationRay(PrimaryEdge);

            for (int EdgeIndex2 = 0; EdgeIndex2 < rect2Rays.Length; EdgeIndex2++)
            {
                Vector2? CollisionPoint = EquationRay.GetIntersection(PrimaryEdgeRay, rect2Rays[EdgeIndex2]);

                if  (CollisionPoint != null 
                    && PrimaryEdge.IsPointInEdgeArea(CollisionPoint.Value)
                    && rect2Edges[EdgeIndex2].IsPointInEdgeArea(CollisionPoint.Value))
                {
                    CollisionPoints.Add(CollisionPoint.Value);
                }
            }
        }

        // Return intersection points.
        if (CollisionPoints.Count == 0)
        {
            return null;
        }
        return (CollisionPoints.ToArray(), Vector2.Normalize((rect2SourcePart.Position + rect2.Offset) - (Position + rect1.Offset)));
    }

    protected virtual (Vector2[] points, Vector2 surfaceNormal)? GetCollisionPointsRectToBall(
        RectangleCollisionBound rect,
        PhysicalEntityPart rectSourcePart,
        BallCollisionBound ball,
        PhysicalEntityPart ballSourcePart)
    {
        // Prepare variables.
        List<Vector2> CollisionPoints = new();
        Vector2[] Vertices = rect.GetVertices(rectSourcePart.Position, rectSourcePart.CombinedRotation);

        Vector2 RectPosition = rectSourcePart.Position + rect.Offset;
        Vector2 BallPosition = ballSourcePart.Position + ball.Offset;

        Circle BallCircle = new(ball.Radius, BallPosition);

        // Iterate over edge rays, find intersections with circle.
        for (int EdgeIndex = 0; EdgeIndex < Vertices.Length; EdgeIndex++)
        {
            Edge Edge = new(Vertices[EdgeIndex], Vertices[(EdgeIndex + 1) % Vertices.Length]);
            EquationRay EdgeRay = new(Edge);

            Vector2[] Points = Circle.GetIntersections(BallCircle, EdgeRay);
            if (Points.Length != 0)
            {
                foreach (Vector2 Point in Points)
                {
                    if (Edge.IsPointInEdgeArea(Point))
                    {
                        CollisionPoints.Add(Point);
                    }
                }
            }
        }

        // Return intersection points.
        if (CollisionPoints.Count == 0)
        {
            return null;
        }

        return (CollisionPoints.ToArray(), Vector2.Normalize(RectPosition- BallPosition));
    }

    protected virtual (Vector2[] points, Vector2 surfaceNormal)? GetCollisionPointsBallToBall(BallCollisionBound ball1,
        BallCollisionBound ball2,
        PhysicalEntityPart ball2SourcePart
        )
    {
        // Prepare variables.
        Circle PrimaryCircle = new(ball1.Radius, Position + ball1.Offset);
        Circle SecondaryCircle = new(ball2.Radius, ball2SourcePart.Position + ball2.Offset);

        // Find collision points.
        Vector2[] CollisionPoints = Circle.GetIntersections(PrimaryCircle, SecondaryCircle);

        if (CollisionPoints.Length == 0)
        {
            return null;
        }

        return (CollisionPoints, Vector2.Normalize((ball2SourcePart.Position + ball2.Offset) - (Position + ball1.Offset)));
    }


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
    protected void AttractEntities()
    {
        foreach (Entity WorldEntity in Entity!.World!.Entities)
        {
            if ((WorldEntity == Entity) || (!WorldEntity.IsPhysical)) continue;

            PhysicalEntity PhysicalWorldEntity = (PhysicalEntity)WorldEntity;

            if (!PhysicalWorldEntity.IsAttractable) continue;

            const float ARBITRARY_ATTRACTION_INCREASE = 1000f;
            float AttractionStrength = (MaterialInstance.Material.Attraction * ARBITRARY_ATTRACTION_INCREASE
                / Vector2.Distance(Position, PhysicalWorldEntity.Position)) * Entity.World!.PassedTimeSeconds;

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