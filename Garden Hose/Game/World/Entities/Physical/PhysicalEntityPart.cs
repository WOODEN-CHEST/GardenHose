﻿using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Material;
using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace GardenHose.Game.World.Entities;


internal class PhysicalEntityPart
{
    // Fields.
    internal bool IsMainPart => ParentLink == null;

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

    public virtual float Temperature => MaterialInstance.Temperature;

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

            Entity.PartCollisionBoundChange();
            CollisionBoundChange?.Invoke(this, _collisionBounds);
        }
    }

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

            Entity.PartChange();
            ParentChange?.Invoke(this, _parentLink);
        }
    }

    internal virtual PhysicalEntity Entity { get; init; }

    internal virtual WorldMaterialInstance MaterialInstance { get; set; }

    internal event EventHandler<Vector2>? Collision;

    internal event EventHandler<ICollisionBound[]?>? CollisionBoundChange;

    internal event EventHandler<PartLink?>? ParentChange;

    internal event EventHandler<PartLink[]?>? SubPartChange;


    // Private fields.
    private ICollisionBound[]? _collisionBounds;
    private float _selfRotation = 0f;

    private PartLink? _parentLink;


    // Constructors.
    internal PhysicalEntityPart(ICollisionBound[] collisionBounds, WorldMaterial material, PhysicalEntity entity)
        : this(collisionBounds, material.CreateInstance(), entity) { }

    internal PhysicalEntityPart(ICollisionBound[] collisionBounds, 
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

        PartLink Link = new PartLink(this, part, Entity, distance);
        NewPartLinks[^1] = Link;
        SubPartLinks = NewPartLinks;

        part.ParentLink = Link;

        Entity.PartChange();
        SubPartChange?.Invoke(this, SubPartLinks);
    }

    internal virtual void LinkEntityAsPart(PhysicalEntity entity, Vector2 distance)
    {
        LinkPart(entity.MainPart, distance);
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

        Entity.PartChange();
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
    internal virtual void TestCollisionAgainstEntity(PhysicalEntity entity)
    {
        if (CollisionBounds == null)
        {
            return;
        }

        foreach (ICollisionBound CollisionBound in CollisionBounds)
        {
            TestBoundAgainstEntity(this, CollisionBound, entity);
        }
    }

    internal (Vector2[] points, Vector2 surfaceNormal)? TestBoundAgainstBound(ICollisionBound selfBound,
        ICollisionBound targetBound, PhysicalEntityPart targetPart)
    {
        if (selfBound.Type == CollisionBoundType.Rectangle && targetBound.Type == CollisionBoundType.Rectangle)
        {
            return GetCollisionPointsRectToRect();
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
            return GetCollisionPointsBallToBall();
        }
        else
        {
            throw new NotSupportedException("Unknown bound types, cannot test collision. " +
               $"Bound type 1: \"{selfBound}\" (int value of {(int)selfBound.Type}), " +
               $"Bound type 1: \"{targetBound}\" (int value of {(int)targetBound.Type}), ");
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
    internal virtual void Tick()
    {
        SelfRotation += AngularMotion * GameFrameManager.PassedTimeSeconds;

        if (SubPartLinks ==  null)
        {
            return;
        }

        foreach (PartLink Link in SubPartLinks)
        {
            Link.LinkedPart.Tick();
        }
    }

    internal virtual void Draw(bool drawCollisionBounds)
    {
        if (drawCollisionBounds && (_collisionBounds != null))
        {
            DrawCollisionBounds();
        }

        if (SubPartLinks == null)
        {
            return;
        }

        foreach (PartLink Link in SubPartLinks)
        {
            Link.LinkedPart.Draw(drawCollisionBounds);
        }
    }

    internal virtual void DrawCollisionBounds()
    {
        foreach (ICollisionBound Bound in CollisionBounds!)
        {
            Bound.Draw(Position, CombinedRotation, Entity.World!);
        }
    }


    // Protected methods.
    /* Collision. */
    protected virtual void TestBoundAgainstEntity(PhysicalEntityPart selfPart,
        ICollisionBound selfBound,
        PhysicalEntity targetEntity)
    {
        foreach (PhysicalEntityPart TargetPart in targetEntity.Parts)
        {
            if (TargetPart.CollisionBounds == null)
            {
                return;
            }

            (Vector2[] points, Vector2 surfaceNormal)? CollisionData = null;

            foreach (ICollisionBound TargetBound in TargetPart.CollisionBounds) // At this point it is a 4 layer deep foreach loop...
            {
                CollisionData = TestBoundAgainstBound(selfBound, TargetBound, TargetPart);

                if (CollisionData == null)
                {
                    continue;
                }

                CollisionCase Case = new()
                {
                    EntityA = Entity,
                    EntityB = targetEntity,
                    PartA = selfPart,
                    PartB = TargetPart,
                    BoundA = selfBound,
                    BoundB = TargetBound,
                    CollisionPoints = CollisionData.Value.points,
                    SurfaceNormal = CollisionData.Value.surfaceNormal
                };

                Entity.PartCollision(Case);
            }
        }
    }

    protected virtual (Vector2[] points, Vector2 surfaceNormal)? GetCollisionPointsRectToRect()
    {
        // Get collision points.

        throw new NotImplementedException();
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

    protected virtual (Vector2[] points, Vector2 surfaceNormal)? GetCollisionPointsBallToBall()
    {
        throw new NotImplementedException();
    }
}