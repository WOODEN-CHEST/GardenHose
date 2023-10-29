using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

            Entity.OnPartCollisionBoundChange(this, _collisionBounds);
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

            Entity.OnPartParentChange(this, _parentLink);
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

    internal PhysicalEntityPart(ICollisionBound[] collisionBounds, WorldMaterialInstance material, PhysicalEntity entity)
    {
        Entity = entity;
        CollisionBounds = collisionBounds;
        MaterialInstance = material;
    }


    // Internal methods.
    /* Parts. */
    internal void LinkPart(PhysicalEntityPart part, Vector2 distance)
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

        Entity.OnPartSubPartChange(this, SubPartLinks);
        SubPartChange?.Invoke(this, SubPartLinks);
    }

    internal void LinkEntityAsPart(PhysicalEntity entity, Vector2 distance)
    {
        LinkPart(entity.MainPart, distance);
    }

    internal void UnlinkPart(PhysicalEntityPart part)
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

        Entity.OnPartSubPartChange(this, SubPartLinks);
        SubPartChange?.Invoke(this, SubPartLinks);
    }

    internal List<PartLink> GetPathFromMainPart()
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
    internal void TestCollisionPlanet()
    {
        if (CollisionBounds == null)
        {
            return;
        }

        //GameWorld World = Entity.World!;

        //foreach (ICollisionBound Bound in CollisionBounds)
        //{
        //    switch (Bound.Type)
        //    {
        //        case CollisionBoundType.Rectangle:
        //            TestColRectToBall((RectangleCollisionBound)Bound, 
        //                Position + Bound.Offset,
        //                World.Planet.CollisionBound, 
        //                World.Planet.Position + World.Planet.CollisionBound.Offset);
        //            break;

        //        case CollisionBoundType.Ball:
        //            TestColBallToBall();
        //            break;

        //        default:
        //            throw new NotImplementedException("Testing collision not implemented for " +
        //                $"collision bound type \"{Bound.Type}\" (int value of {(int)Bound.Type})");
        //    }
        //}
    }


    /* Properties. */
    internal void SetPositionAndRotation(Vector2 position, float rotation)
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

    internal void DrawCollisionBounds()
    {
        foreach (ICollisionBound Bound in CollisionBounds!)
        {
            Bound.Draw(Position, CombinedRotation, Entity.World!);
        }
    }


    // Private methods.
    private void TestColRectToBall(RectangleCollisionBound rect, 
        Vector2 rectPosition,
        BallCollisionBound ball, 
        Vector2 ballPosition)
    {
        Vector2[] Vertices = rect.GetVertices(Position, CombinedRotation);
        List<Vector2> CollisionPoints = null!;

        // Find closest point so that a testable ray can be created.
        float ClosestDistance = float.PositiveInfinity;
        Vector2 ClosestVertex = Vector2.Zero;

        foreach (Vector2 Vertex in Vertices)
        {
            float Distance = Vector2.Distance(ballPosition, Vertex);
            if (Distance < ClosestDistance)
            {
                ClosestDistance = Distance;
                ClosestVertex = Vertex;
            }
        }

        Ray BallToRectRay = new(ballPosition, ClosestVertex);

        /* Find collision points. This is done by getting rays from the edge vertices,
         * then finding intersection points in said rays, then testing if the intersection 
         * point is inside of the edge's limits. If so, a collision has occurred.*/
        for (int EdgeIndex = 0; EdgeIndex < Vertices.Length; EdgeIndex++)
        {
            Edge Edge = new(Vertices[EdgeIndex], Vertices[(EdgeIndex + 1) % Vertices.Length]);
            Ray EdgeRay = new(Edge);

            Vector2 CollisionPoint = Ray.GetIntersection(EdgeRay, BallToRectRay);

            float MinX = Math.Min(Edge.StartVertex.X, Edge.EndVertex.X);
            float MaxX = Math.Max(Edge.StartVertex.X, Edge.EndVertex.X);
            float MinY = Math.Min(Edge.StartVertex.Y, Edge.EndVertex.Y);
            float MaxY = Math.Max(Edge.StartVertex.Y, Edge.EndVertex.Y);

            if (Vector2.Distance(ballPosition, CollisionPoint) <= ball.Radius
                && (MinX <= CollisionPoint.X) && (CollisionPoint.X <= MaxX)
                && (MinY <= CollisionPoint.Y) && (CollisionPoint.Y <= MaxY))
            {
                CollisionPoints ??= new();
                CollisionPoints.Add(CollisionPoint);
            }
        }

        if (CollisionPoints != null)
        {
            Vector2 SurfaceNormal = Vector2.Normalize(rectPosition - ballPosition);

            //PushOutOfBall(CollisionPoints[0], ball);
            //OnCollision(CollisionPoints[0], Vector2.Zero, SurfaceNormal);
        }
    }

    private void TestColBallToBall()
    {
        throw new NotImplementedException();
    }

    private void TestColRectToRect()
    {
        throw new NotImplementedException();
    }
}