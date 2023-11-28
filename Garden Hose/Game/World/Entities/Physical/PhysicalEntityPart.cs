using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Material;
using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace GardenHose.Game.World.Entities;


internal abstract class PhysicalEntityPart
{
    // Fields.
    internal virtual PhysicalEntity Entity { get; init; }


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

    internal virtual bool IsInvulnerable { get; set; } = false;


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

    internal virtual PartLink[]? SubPartLinks { get; private set; } = null;

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


    /* Events. */
    internal event EventHandler<Vector2>? Collision;

    internal event EventHandler<ICollisionBound[]?>? CollisionBoundChange;

    internal event EventHandler<PartLink?>? ParentChange;

    internal event EventHandler<PartLink[]?>? SubPartChange;

    internal event EventHandler<PhysicalEntityPart>? PartDamage;

    internal event EventHandler<PhysicalEntityPart>? PartBreak;


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


        var NewPartLinks = SubPartLinks == null ? new PartLink[1] : new PartLink[SubPartLinks.Length + 1];
        SubPartLinks?.CopyTo(NewPartLinks, 0);

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
        if (SubPartLinks == null)
        {
            return;
        }

        List<PartLink> Links = new(SubPartLinks);

        foreach (PartLink Link in Links)
        {
            if (Link.LinkedPart == part)
            {
                Links.Remove(Link);
                break;
            }
        }

        SubPartLinks = Links.Count == 0 ? null : Links.ToArray();

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
        if (CollisionBounds == null)
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
        Collision?.Invoke(this, location);
        ApplyForce(location, appliedForce);
    }

    internal virtual void ApplyForce(Vector2 location, float forceAmount)
    {
        const float SOUND_FORCE_DOWNSCALE = 0.7f;
        if (forceAmount >= (MaterialInstance.Material.Resistance * SOUND_FORCE_DOWNSCALE))
        {

        }

        if (forceAmount >= MaterialInstance.Material.Resistance && !IsInvulnerable)
        {
            MaterialInstance.CurrentStrength -= forceAmount;

            if (MaterialInstance.Stage == WorldMaterialStage.Destroyed)
            {
                Entity.OnPartDestroy();
                PartBreak?.Invoke(this, this);
                OnPartDestroy();
                return;
            }
            else
            {
                Entity.OnPartDamage();
                PartDamage?.Invoke(this, this);
                OnPartDamage();
            }
        }

        if ((ParentLink != null) && (forceAmount >= ParentLink.LinkStrength))
        {
            OnPartBreakOff();
        }
    }

    /* Properties. */
    internal virtual void SetPositionAndRotation(Vector2 position, float rotation)
    {
        Position = position;
        CombinedRotation = SelfRotation + rotation;

        if (SubPartLinks == null)
        {
            return;
        }

        foreach (PartLink Link in SubPartLinks)
        {
            Link.LinkedPart.SetPositionAndRotation(
                Vector2.Transform(Link.LinkDistance, Matrix.CreateRotationZ(CombinedRotation)) + Position,
                CombinedRotation);
        }
    }

    /* Game flow. */
    internal virtual void ParallelTick()
    {
        SelfRotation += AngularMotion * Entity.World!.PassedTimeSeconds;

        if (SubPartLinks != null)
        {
            foreach (PartLink Link in SubPartLinks)
            {
                Link.LinkedPart.ParallelTick();
            }
        }
    }

    internal virtual void SequentialTick()
    {
        if (SubPartLinks != null)
        {
            foreach (PartLink Link in SubPartLinks)
            {
                Link.LinkedPart.SequentialTick();
            }
        }

        MaterialTick();
    }

    /* Drawing. */
    internal virtual void Draw()
    {
        if (SubPartLinks == null)
        {
            return;
        }

        foreach (PartLink Link in SubPartLinks)
        {
            Link.LinkedPart.Draw();
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


    // Protected methods.
    /* Collision. */
    protected virtual CollisionCase[] TestBoundAgainstEntity(ICollisionBound selfBound,
        PhysicalEntity targetEntity)
    {
        List<CollisionCase> CollisionCases = new();

        foreach (PhysicalEntityPart TargetPart in targetEntity.Parts)
        {
            if (TargetPart.CollisionBounds == null)
            {
                return CollisionCases.ToArray();
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
                CollisionCase Case = new()
                {
                    EntityA = Entity,
                    EntityB = targetEntity,
                    PartA = this,
                    PartB = TargetPart,
                    BoundA = selfBound,
                    BoundB = TargetBound,
                    CollisionPoints = CollisionData.Value.points,
                    SurfaceNormal = CollisionData.Value.surfaceNormal
                };

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
        Ray[] rect2Rays = new Ray[]
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
            Ray PrimaryEdgeRay = new Ray(PrimaryEdge);

            for (int EdgeIndex2 = 0; EdgeIndex2 < rect2Rays.Length; EdgeIndex2++)
            {
                Vector2? CollisionPoint = Ray.GetIntersection(PrimaryEdgeRay, rect2Rays[EdgeIndex2]);

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
        Vector2[] Vertices = rect.GetVertices(rectSourcePart.Position, CombinedRotation);

        Vector2 RectPosition = rectSourcePart.Position + rect.Offset;
        Vector2 BallPosition = ballSourcePart.Position + ball.Offset;

        Circle BallCircle = new(ball.Radius, BallPosition);

        // Iterate over edge rays, find intersections with circle.
        for (int EdgeIndex = 0; EdgeIndex < Vertices.Length; EdgeIndex++)
        {
            Edge Edge = new(Vertices[EdgeIndex], Vertices[(EdgeIndex + 1) % Vertices.Length]);
            Ray EdgeRay = new(Edge);

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


    /* Physics. */
    protected virtual void MaterialTick()
    {
        MaterialInstance.HeatByTouch(Entity.World!.AmbientMaterial, Entity.World.PassedTimeSeconds);

        if (MaterialInstance.Temperature > MaterialInstance.Material.BoilingPoint)
        {

        }
        else if (MaterialInstance.Temperature > MaterialInstance.Material.MeltingPoint)
        {

        }
    }


    /* Parts. */
    protected abstract void OnPartDamage();

    protected abstract void OnPartDestroy();

    protected abstract void OnPartBreakOff();
}