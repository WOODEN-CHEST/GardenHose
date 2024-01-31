using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GardenHose.Game.World.Entities.Physical.Collision;

internal class EntityCollisionHandler
{
    // Internal fields.
    internal PhysicalEntity Entity { get; private init; }
    internal bool IsCollisionEnabled { get; set; } = true;
    internal bool IsCollisionReactionEnabled { get; set; } = true;
    internal float BoundingLength { get; private set; }


    // Private fields.
    private readonly HashSet<PhysicalEntity> _collisionIgnorableEntities = new();
    private readonly HashSet<PhysicalEntity> _entitiesCollidedWith = new();


    // Constructors.
    internal EntityCollisionHandler(PhysicalEntity entity)
    {
        Entity = entity;
    }


    // Internal methods.
    /* Entity control. */
    internal virtual void AddCollisionIgnorable(PhysicalEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (_collisionIgnorableEntities.Add(entity))
        {
            entity.EntityDelete += OnCollisionIgnorableEntityDeleteEvent;
        }
    }

    internal virtual void RemoveCollisionIgnorable(PhysicalEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (_collisionIgnorableEntities.Remove(entity))
        {
            entity.EntityDelete -= OnCollisionIgnorableEntityDeleteEvent;
        }
    }

    internal virtual bool IsCollisionIgnored(PhysicalEntity targetEntity)
    {
        return _collisionIgnorableEntities.Contains(targetEntity);
    }

    internal void AddCollidedEntity(PhysicalEntity entity)
    {
        _entitiesCollidedWith.Add(entity);
    }

    internal virtual bool IsEntityCollidedWith(PhysicalEntity targetEntity)
    {
        return _entitiesCollidedWith.Contains(targetEntity);
    }

    internal void ClearCollidedEntities()
    {
        _entitiesCollidedWith.Clear();
    }

    internal bool CanCollideWithEntity(PhysicalEntity targetEntity)
    {
        return IsCollisionEnabled && !IsCollisionIgnored(targetEntity) && !IsEntityCollidedWith(targetEntity);
    }

    /* Collision. */
    internal virtual bool TestCollisionAgainstEntity(PhysicalEntity targetEntity, out CollisionCase[] collisions)
    {
        collisions = Array.Empty<CollisionCase>();

        // Early exits and special cases.
        if (!CanCollideWithEntity(targetEntity) || !targetEntity.CollisionHandler.CanCollideWithEntity(Entity))
        {
            return false;
        }

        // Test bounding circles.
        if (Vector2.Distance(Entity.Position, targetEntity.Position) > (BoundingLength + targetEntity.CollisionHandler.BoundingLength))
        {
            return false;
        }

        // Test parts against entity.
        List<CollisionCase> Collisions = new();

        foreach (PhysicalEntityPart SelfPart in Entity.Parts)
        {
            TestPartAgainstEntity(Collisions, SelfPart, targetEntity); // Populate list with collisions (if any occur).
        }

        collisions = Collisions.ToArray();

        if (collisions.Length != 0)
        {
            AddCollidedEntity(targetEntity);
            targetEntity.CollisionHandler.AddCollidedEntity(Entity);
            return true;
        }
        return false;
    }


    // Protected methods.
    protected virtual void TestPartAgainstEntity(List<CollisionCase> cases, PhysicalEntityPart selfPart, PhysicalEntity targetEntity)
    {
        if ((selfPart.CollisionBounds.Length == 0) || (selfPart.MaterialInstance.State == WorldMaterialState.Gas))
        {
            return;
        }

        foreach (ICollisionBound CollisionBound in selfPart.CollisionBounds)
        {
            foreach (PhysicalEntityPart TargetPart in targetEntity.Parts)
            {
                TestBoundAgainstPart(cases, selfPart, CollisionBound, TargetPart, targetEntity);
            }
        }
    }

    protected void TestBoundAgainstPart(List<CollisionCase> cases,
        PhysicalEntityPart selfPart, 
        ICollisionBound selfBound,
        PhysicalEntityPart targetPart,
        PhysicalEntity targetEntity)
    {
        if ((targetPart.CollisionBounds.Length == 0) || (targetPart.MaterialInstance.State == WorldMaterialState.Gas))
        {
            return;
        }

        foreach (ICollisionBound TargetBound in targetPart.CollisionBounds)
        {
            //Test bounding radius.
            if (Vector2.Distance(Entity.Position + selfBound.Offset, 
                targetPart.Position + TargetBound.Offset) > selfBound.BoundingRadius + TargetBound.BoundingRadius)
            {
                continue;
            }

            // Get collision data.
            Vector2[] CollisionPoints = TestBoundAgainstBound(selfBound, 
                TargetBound, 
                selfPart.Position, 
                targetPart.Position,
                selfPart.CombinedRotation,
                targetPart.CombinedRotation);

            if (CollisionPoints.Length == 0)
            {
                continue;
            }

            // Build case.
            Vector2 CollisionNormal = Entity.Position - targetEntity.Position;
            if (CollisionNormal.LengthSquared() is 0f or -0f)
            {
                CollisionNormal = -Vector2.UnitY;
            }
            CollisionNormal.Normalize();

            CollisionCase Case = new(Entity,
                targetEntity,
                selfPart,
                targetPart,
                selfBound,
                TargetBound,
                CollisionNormal,
                CollisionPoints);

            cases.Add(Case);
        }
    }

    protected virtual Vector2[] TestBoundAgainstBound(
        ICollisionBound selfBound,
        ICollisionBound targetBound, 
        Vector2 selfPartPosition,
        Vector2 targetPartPosition,
        float selfPartCombinedRotation,
        float targetPartCombinedRotation)
    {
        if ((selfBound.Type == CollisionBoundType.Rectangle) && (targetBound.Type == CollisionBoundType.Rectangle))
        {
            return GetCollisionPointsRectToRect((RectangleCollisionBound)selfBound, (RectangleCollisionBound)targetBound,
                selfPartPosition, targetPartPosition, selfPartCombinedRotation, targetPartCombinedRotation);
        }
        else if ((selfBound.Type == CollisionBoundType.Ball) && (targetBound.Type == CollisionBoundType.Rectangle))
        {
            return GetCollisionPointsRectToBall((RectangleCollisionBound)targetBound, (BallCollisionBound)selfBound,
                selfPartPosition, targetPartPosition, selfPartCombinedRotation, targetPartCombinedRotation);
        }
        else if ((selfBound.Type == CollisionBoundType.Rectangle) && (targetBound.Type == CollisionBoundType.Ball))
        {
            return GetCollisionPointsRectToBall((RectangleCollisionBound)selfBound, (BallCollisionBound)targetBound,
                selfPartPosition, targetPartPosition, selfPartCombinedRotation, targetPartCombinedRotation);
        }
        else if (selfBound.Type == CollisionBoundType.Ball && targetBound.Type == CollisionBoundType.Ball)
        {
            return GetCollisionPointsBallToBall((BallCollisionBound)selfBound, (BallCollisionBound)targetBound,
                selfPartPosition, targetPartPosition, selfPartCombinedRotation, targetPartCombinedRotation);
        }
        else
        {
            throw new NotSupportedException("Unknown bound types, cannot test collision. " +
               $"Bound type 1: \"{selfBound}\" (int value of {(int)selfBound.Type}), " +
               $"Bound type 2: \"{targetBound}\" (int value of {(int)targetBound.Type}), ");
        }
    }

    protected virtual Vector2[] GetCollisionPointsRectToRect(RectangleCollisionBound selfRect,
        RectangleCollisionBound targetRect,
        Vector2 selfPartPosition,
        Vector2 targetPartPosition,
        float selfPartCombinedRotation,
        float targetPartCombinedRotation)
    {
        // Prepare variables.
        Edge[] SelfRectEdges = selfRect.GetEdges(selfPartPosition, selfPartCombinedRotation);
        Edge[] TargetRectEdges = targetRect.GetEdges(selfPartPosition, selfPartCombinedRotation);
        EquationRay[] TargetRectRays =
        {
            new EquationRay(TargetRectEdges[0]),
            new EquationRay(TargetRectEdges[1]),
            new EquationRay(TargetRectEdges[2]),
            new EquationRay(TargetRectEdges[3]),
        };
        List<Vector2>? CollisionPoints = null;


        // Find collision points.
        for (int SelfEdgeIndex = 0; SelfEdgeIndex < SelfRectEdges.Length; SelfEdgeIndex++)
        {
            EquationRay PrimaryEdgeRay = new EquationRay(SelfRectEdges[SelfEdgeIndex]);

            for (int TargetEdgeIndex = 0; TargetEdgeIndex < TargetRectEdges.Length; TargetEdgeIndex++)
            {
                Vector2? CollisionPoint = EquationRay.GetIntersection(PrimaryEdgeRay, TargetRectRays[TargetEdgeIndex]);

                if ((CollisionPoint != null)
                    && SelfRectEdges[SelfEdgeIndex].IsPointInEdgeArea(CollisionPoint.Value)
                    && TargetRectEdges[TargetEdgeIndex].IsPointInEdgeArea(CollisionPoint.Value))
                {
                    CollisionPoints ??= new();
                    CollisionPoints.Add(CollisionPoint.Value);
                }
            }
        }

        return CollisionPoints?.ToArray() ?? Array.Empty<Vector2>();
    }

    protected virtual Vector2[] GetCollisionPointsRectToBall(
        RectangleCollisionBound rect,
        BallCollisionBound ball,
        Vector2 rectPartPosition,
        Vector2 ballPartPosition,
        float rectPartCombinedRotation,
        float ballPartCombinedRotation)
    {
        // Prepare variables.
        List<Vector2>? CollisionPoints = null;

        Edge[] RectEdges = rect.GetEdges(rectPartPosition, rectPartCombinedRotation);
        EquationRay[] RectRays =
        {
            new EquationRay(RectEdges[0]),
            new EquationRay(RectEdges[1]),
            new EquationRay(RectEdges[2]),
            new EquationRay(RectEdges[3]),
        };
        Circle BallCircle = new(ball.Radius, ball.GetFinalPosition(ballPartPosition, ballPartCombinedRotation));


        // Find collision points.
        for (int EdgeIndex = 0; EdgeIndex < RectEdges.Length; EdgeIndex++)
        {
            Vector2[] Points = Circle.GetIntersections(BallCircle, RectRays[EdgeIndex]);

            if (Points.Length != 0)
            {
                foreach (Vector2 Point in Points)
                {
                    if (RectEdges[EdgeIndex].IsPointInEdgeArea(Point))
                    {
                        CollisionPoints ??= new();
                        CollisionPoints.Add(Point);
                    }
                }
            }
        }

        // Return intersection points.
        return CollisionPoints?.ToArray() ?? Array.Empty<Vector2>();
    }

    protected virtual Vector2[] GetCollisionPointsBallToBall(
        BallCollisionBound ball1,
        BallCollisionBound ball2,
        Vector2 ball1PartPosition,
        Vector2 ball2PartPosition,
        float ball1PartCombinedRotation,
        float ball2PartCombinedRotation)
    {
        Circle PrimaryCircle = new(ball1.Radius, ball1.GetFinalPosition(ball1PartPosition, ball1PartCombinedRotation));
        Circle SecondaryCircle = new(ball2.Radius, ball2.GetFinalPosition(ball2PartPosition, ball2PartCombinedRotation));

        Vector2[] CollisionPoints = Circle.GetIntersections(PrimaryCircle, SecondaryCircle);
        return CollisionPoints;
    }


    // Private methods.
    private void CreateBoundingBox()
    {
        BoundingLength = 0f;

        if (Entity.MainPart == null)
        {
            return;
        }

        List<Vector2> Points = GetAllPointsInCollisionBounds();
        if (Points.Count == 0)
        {
            return;
        }

        Vector2 FurthestPoint = Points[0];
        foreach (Vector2 point in Points)
        {
            if (point.LengthSquared() > FurthestPoint.LengthSquared())
            {
                FurthestPoint = point;
            }
        }

        BoundingLength = FurthestPoint.Length();
    }

    private List<Vector2> GetAllPointsInCollisionBounds()
    {
        List<Vector2> Points = new();

        foreach (PhysicalEntityPart Part in Entity.Parts)
        {
            if (Part.CollisionBounds.Length == 0)
            {
                continue;
            }

            foreach (ICollisionBound Bound in Part.CollisionBounds)
            {
                Points.AddRange(GetCollisionBoundPoints(Bound, Part.Position));
            }
        }

        return Points;
    }

    private Vector2[] GetCollisionBoundPoints(ICollisionBound bound, Vector2 partPosition)
    {
        switch (bound.Type)
        {
            case CollisionBoundType.Rectangle:
                return ((RectangleCollisionBound)bound).GetVertices(partPosition, 0f);

            case CollisionBoundType.Ball:
                BallCollisionBound BallBound = (BallCollisionBound)bound;
                Vector2 FinalBoundPosition = partPosition + bound.Offset;

                Vector2 NormalToBallPoint = FinalBoundPosition - Entity.Position;
                if (NormalToBallPoint.LengthSquared() == 0)
                {
                    NormalToBallPoint = -Vector2.UnitY;
                }
                NormalToBallPoint.Normalize();

                return new Vector2[]
                {
                    NormalToBallPoint * (BallBound.Radius + Vector2.Distance(FinalBoundPosition, Entity.Position))
                };


            default:
                throw new NotImplementedException("Getting collision bound points is not supported for bound type" +
                    $"of \"{bound.Type}\" (int value of {(int)bound.Type})");
        }
    }

    


    // Private methods.
    private void OnCollisionIgnorableEntityDeleteEvent(object? sender, EventArgs args)
    {
        PhysicalEntity DeletedEntity = (PhysicalEntity)sender!;
        RemoveCollisionIgnorable(DeletedEntity);
        DeletedEntity.EntityDelete -= OnCollisionIgnorableEntityDeleteEvent;
    }
}