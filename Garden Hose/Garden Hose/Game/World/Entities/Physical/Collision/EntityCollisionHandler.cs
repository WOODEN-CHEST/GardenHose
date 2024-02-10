using GardenHose.Game.World.Entities.Particle;
using GardenHose.Game.World.Entities.Stray;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GardenHose.Game.World.Entities.Physical.Collision;

internal class EntityCollisionHandler
{
    // Internal fields.
    internal PhysicalEntity Entity { get; private init; }
    internal bool IsCollisionEnabled { get; set; } = true;
    internal bool IsCollisionReactionEnabled { get; set; } = true;
    internal float BoundingRadius { get; private set; }

    internal event EventHandler<CollisionEventArgs>? Collision;
    internal event EventHandler<CollisionEventArgs>? PartDamage;
    internal event EventHandler<CollisionEventArgs>? PartDestroy;
    internal event EventHandler<CollisionEventArgs>? PartBreakOff;


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
        if (Vector2.Distance(Entity.Position, targetEntity.Position) > (BoundingRadius + targetEntity.CollisionHandler.BoundingRadius))
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

    internal virtual void OnCollision(CollisionCase collisionCase, GHGameTime time)
    {
        if (!IsCollisionReactionEnabled) return;

        if (collisionCase.SelfPart.MaterialInstance.State != WorldMaterialState.Solid
            || collisionCase.TargetPart.MaterialInstance.State != WorldMaterialState.Solid)
        {
            OnSoftCollision(collisionCase, time);
        }
        else
        {
            OnHardCollision(collisionCase);
        }
    }
    internal virtual void PushOutOfOtherEntity(CollisionCase collisionCase)
    {
        if ((collisionCase.TargetPart.MaterialInstance.State != WorldMaterialState.Solid)
            || (collisionCase.SelfPart.MaterialInstance.State != WorldMaterialState.Solid))
        {
            return;
        }


        // Prepare variables.
        Vector2 PushOutDirection = GetPushOutDirection(collisionCase.TargetEntity);
        const int STEP_COUNT = 8;
        float STEP_DISTANCE = 10f;
        int StepsTaken = 0;
        const int MAX_STEPS = 1000;

        // Push out of other entity.
        do
        {
            Entity.Position += PushOutDirection * STEP_DISTANCE;
            StepsTaken++;
        }
        while ((TestBoundAgainstBound(collisionCase.SelfBound, collisionCase.TargetBound, collisionCase.SelfPart.Position,
            collisionCase.TargetPart.Position, collisionCase.SelfPart.CombinedRotation, collisionCase.TargetPart.CombinedRotation).Length != 0)
            && (StepsTaken <= MAX_STEPS));

        if (StepsTaken > MAX_STEPS)
        {
            throw new InvalidOperationException("Collision algorithm failed to push out an entity after taking the maximum amount of steps allowed. " +
                "\nThis should not ever happen.\n Crashing game to avoid further issues. CollisionCase details:" +
                $"SelfPos: {collisionCase.SelfEntity.Position}; TargetPos: {collisionCase.TargetEntity.Position}");
        }

        // Push closer as much as possible.
        for (int Step = 0; Step < STEP_COUNT; Step++)
        {
            Vector2 PreviousPosition = Entity.Position;
            STEP_DISTANCE *= 0.5f;

            Entity.Position -= PushOutDirection * STEP_DISTANCE;

            if (TestBoundAgainstBound(collisionCase.SelfBound, collisionCase.TargetBound, collisionCase.SelfPart.Position,
            collisionCase.TargetPart.Position, collisionCase.SelfPart.CombinedRotation, collisionCase.TargetPart.CombinedRotation).Length != 0)
            {
                Entity.Position = PreviousPosition;
            }
        }
    }

    internal void SpacePartitionEntity()
    {
        _entitiesCollidedWith.Clear();
        Entity.World!.AddPhysicalEntityToWorldPart(Entity);
    }

    internal void CreateBoundingBox()
    {
        BoundingRadius = 0f;

        if (Entity.MainPart == null)
        {
            return;
        }

        List<Vector2> Points = GetAllPointsInCollisionBounds();
        if (Points.Count == 0)
        {
            return;
        }

        float FurthestDistance = 0f;
        foreach (Vector2 Point in Points)
        {
            if ((Entity.Position - Point).Length() > FurthestDistance)
            {
                FurthestDistance = (Entity.Position - Point).Length();
            }
        }

        BoundingRadius = FurthestDistance;
    }


    // Protected methods.
    /* Collision testing. */
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
                targetPartPosition, selfPartPosition, targetPartCombinedRotation, selfPartCombinedRotation);
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
        Edge[] TargetRectEdges = targetRect.GetEdges(targetPartPosition, targetPartCombinedRotation);
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
                    if (RectEdges[EdgeIndex].IsPointInEdgeArea(Point)
                        && (Vector2.Distance(Point, ballPartPosition + ball.Offset) <= ball.Radius))
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


    /* Parts. */
    protected void OnPartCollision(CollisionEventArgs collisionArgs)
    {
        collisionArgs.Case.SelfPart.MaterialInstance.HeatByCollision(collisionArgs.ForceApplied);

        const float SOUND_FORCE_DOWNSCALE = 0.7f;
        if (collisionArgs.ForceApplied >= (collisionArgs.Case.SelfPart.MaterialInstance.Material.Resistance * SOUND_FORCE_DOWNSCALE))
        {
            // TODO: implement sound.
        }

        if (!collisionArgs.Case.SelfEntity.IsInvulnerable)
        {
            DamagePart(collisionArgs);
        }
    }

    protected void DamagePart(CollisionEventArgs collisionArgs)
    {
        CreateDamageParticles(collisionArgs);

        if (collisionArgs.ForceApplied >= collisionArgs.Case.SelfPart.MaterialInstance.Material.Resistance)
        {
            collisionArgs.Case.SelfPart.MaterialInstance.CurrentStrength -= collisionArgs.ForceApplied;

            if (collisionArgs.Case.SelfPart.MaterialInstance.Stage == WorldMaterialStage.Destroyed)
            {
                OnPartDestroy(collisionArgs);
                PartDestroy?.Invoke(this, collisionArgs);
            }
            else
            {
                PartDamage?.Invoke(this, collisionArgs);
                foreach (PartSprite Sprite in collisionArgs.Case.SelfPart.Sprites)
                {
                    Sprite.SetActiveSprite(collisionArgs.Case.SelfPart.MaterialInstance.Stage);
                }
            }
        }

        if ((collisionArgs.Case.SelfPart.ParentLink != null)
            && (collisionArgs.ForceApplied >= collisionArgs.Case.SelfPart.ParentLink.LinkStrength))
        {
            OnPartBreakOff(collisionArgs);
            PartBreakOff?.Invoke(this, collisionArgs);
        }
    }

    protected void OnPartDestroy(CollisionEventArgs collisionArgs)
    {
        if (!collisionArgs.Case.SelfPart.IsMainPart)
        {
            collisionArgs.Case.SelfPart.ParentLink!.ParentPart.UnlinkPart(collisionArgs.Case.SelfPart);
            return;
        }

        foreach (PartLink Link in collisionArgs.Case.SelfPart.SubPartLinks)
        {
            StrayEntity Stray = StrayEntity.MovePartToStrayEntity(Link.LinkedPart);

            Entity.World!.AddEntity(Stray);
        }

        Entity.Delete();
    }

    protected void OnPartBreakOff(CollisionEventArgs collisionArgs)
    {
        PhysicalEntity OldEntity = Entity;

        StrayEntity Stray = StrayEntity.MovePartToStrayEntity(collisionArgs.Case.SelfPart);

        Stray.CollisionHandler.AddCollisionIgnorable(OldEntity);
        Entity.World!.AddEntity(Stray);
    }

    protected void CreateDamageParticles(CollisionEventArgs collisionArgs)
    {
        if (collisionArgs.Case.SelfPart.MaterialInstance.Material.DamageParticles == null)
        {
            return;
        }

        const int MAX_PARTICLES = 20;
        const float FORCE_MULTIPLIER = 1f / 7500f;
        const float MOTION_MAGNITUDE_RANDOMNESS = 0.5f;
        const float PARTICLE_SPREAD = MathF.PI / 1.75f;
        int ParticleCount = Math.Min(MAX_PARTICLES, (int)(collisionArgs.ForceApplied * FORCE_MULTIPLIER));

        if (ParticleCount == 0)
        {
            return;
        }
        
        ParticleEntity.CreateParticles(Entity.World!,
            collisionArgs.Case.SelfPart.MaterialInstance.Material.DamageParticles!,
            new Range(ParticleCount / 2, ParticleCount),
            collisionArgs.Case.AverageCollisionPoint,
            Entity.Motion,
            MOTION_MAGNITUDE_RANDOMNESS,
            PARTICLE_SPREAD,
            Entity);
    }


    /* Collisions. */
    private void OnSoftCollision(CollisionCase collisionCase, GHGameTime time)
    {
        // Theoretically this code can cause strange behavior of objects pushing out of each other,
        // but practically it is rare. In case of such a bug, search here.
        // Soft collisions do not damage entity parts.

        Vector2 MotionRelativeToOtherEntity = collisionCase.SelfMotion - collisionCase.TargetMotion;

        const float ARBITRARY_SPEED_CHANGE_VALUE = 11.195f;
        float Multiplier = Math.Max(0f, 1f - (ARBITRARY_SPEED_CHANGE_VALUE * time.WorldTime.PassedTimeSeconds));

        Vector2 ChangeInRelativeMotion = MotionRelativeToOtherEntity - (MotionRelativeToOtherEntity * Multiplier);
        Entity.Motion -= ChangeInRelativeMotion;

        Entity.AngularMotion *= Multiplier;

        Collision?.Invoke(this, new CollisionEventArgs(collisionCase, 0f));
    }

    private void OnHardCollision(CollisionCase collisionCase)
    {
        Vector2 MotionAtPoint = collisionCase.SelfMotion + collisionCase.SelfRotationalMotionAtPoint;
        Vector2 EntityBMotionAtPoint = collisionCase.TargetMotion + collisionCase.TargetRotationalMotionAtPoint;

        float CombinedBounciness = (collisionCase.SelfPart.MaterialInstance.Material.Bounciness
            + collisionCase.TargetPart.MaterialInstance.Material.Bounciness) * 0.5f;
        Vector2 ForceApplied = -collisionCase.SurfaceNormal * (MotionAtPoint - EntityBMotionAtPoint).Length() * Entity.Mass;

        float ForceX = Vector2.Dot(ForceApplied, )







        //ForceApplied += ForceApplied * CombinedBounciness;

        //Vector2 Surface = GHMath.PerpVectorClockwise(collisionCase.SurfaceNormal);
        //float AlignedYMotion = Vector2.Dot(MotionAtPoint, collisionCase.SurfaceNormal);
        //float AlignedXMotion = Vector2.Dot(MotionAtPoint, Surface);
        //float EntityBAlignedYMotion = Vector2.Dot(EntityBMotionAtPoint, collisionCase.SurfaceNormal);

        //float CombinedBounciness = (collisionCase.SelfPart.MaterialInstance.Material.Bounciness
        //    + collisionCase.TargetPart.MaterialInstance.Material.Bounciness) * 0.5f;
        //float CombinedFrictionCoef = (collisionCase.SelfPart.MaterialInstance.Material.Friction
        //    + collisionCase.TargetPart.MaterialInstance.Material.Friction) * 0.5f;

        //float MotionY = CalculateSpeedOnAxis(AlignedYMotion, Entity.Mass,
        //    EntityBAlignedYMotion, collisionCase.TargetEntity.Mass, CombinedBounciness);

        //Vector2 NewMotion = (collisionCase.SurfaceNormal * MotionY) + (Surface * AlignedXMotion * CombinedFrictionCoef);
        //Vector2 ForceApplied = Entity.Mass * (NewMotion - MotionAtPoint);


        // Apply forces.
        Entity.ApplyForce(ForceApplied, collisionCase.AverageCollisionPoint);

        // Notify parts and event handlers.
        CollisionEventArgs CollisionArgs = new(collisionCase, ForceApplied.Length());
        OnPartCollision(CollisionArgs);
        Collision?.Invoke(this, CollisionArgs);
    }

    protected float CalculateSpeedOnAxis(float v1, float m1, float v2, float m2, float cor)
    {
        // So there's supposed to be some stuff done here with momentum conservation v1m + v2m = v1`m + v2`m,
        // but it just doesn't seem to work and I cannot get the calculations to make sense no matter what.
        // So here I just stole some formula from the Internet and I don't know how it works (Since my calculations give different results)
        return (m1 * v1 + m2 * v2 + m2 * (v2 - v1)) / (m1 + m2) * cor;
    }


    // Private methods.
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

    private Vector2 GetPushOutDirection(PhysicalEntity otherEntity)
    {
        Vector2 Direction = Entity.Position - otherEntity.Position;

        if (Direction.Length() is 0f or -0f)
        {
            Direction = -Vector2.UnitY;
        }

        return Vector2.Normalize(Direction);
    }

    private void OnCollisionIgnorableEntityDeleteEvent(object? sender, EventArgs args)
    {
        PhysicalEntity DeletedEntity = (PhysicalEntity)sender!;
        RemoveCollisionIgnorable(DeletedEntity);
        DeletedEntity.EntityDelete -= OnCollisionIgnorableEntityDeleteEvent;
    }
}