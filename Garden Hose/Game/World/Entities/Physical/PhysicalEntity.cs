using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;
using GardenHoseEngine;
using GardenHoseEngine.Screen;
using GardenHose.Game.World.Material;
using GardenHose.Game.World.Entities.Physical.Events;
using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical.Collision;

namespace GardenHose.Game.World.Entities.Physical;


internal abstract class PhysicalEntity : Entity, IDrawableItem
{
    // Fields.
    public virtual bool IsVisible { get; set; } = true;

    public virtual Effect? Shader { get; set; }


    // Internal fields.
    internal sealed override bool IsPhysical => true;


    /* Collision. */
    internal bool IsCollisionEnabled { get; set; } = true; // Whether collision testing is even done.

    internal bool IsCollisionReactionEnabled { get; set; } = true; // Whether the object reacts to collisions.

    internal float BoundingLength { get; private set; }

    internal bool IsInvulnerable { get; set; } = false;


    /* Entity properties. */
    internal virtual Vector2 Position
    {
        get => SelfPosition;
        set
        {
            SelfPosition = value;
            SetPositionAndRotation(SelfPosition, Rotation);
        }
    }

    internal virtual Vector2 Motion
    {
        get => _motion;
        set
        {
            _motion = value;

            _motion.X = float.IsNaN(_motion.X) ? 0f : Math.Clamp(_motion.X, MIN_MOTION, MAX_MOTION);
            _motion.Y = float.IsNaN(_motion.Y) ? 0f : Math.Clamp(_motion.Y, MIN_MOTION, MAX_MOTION);
        }
    }

    internal virtual float Rotation
    {
        get => SelfRotation;
        set
        {
            SelfRotation = value;
            SetPositionAndRotation(Position, SelfRotation);
        }
    }

    internal virtual float AngularMotion

    {
        get => _angularMotion;
        set
        {
            _angularMotion = Math.Clamp(value, float.MinValue, float.MaxValue);
        }
    }

    internal virtual float Mass
    {
        get
        {
            if (_cachedMass != null)
            {
                return _cachedMass.Value;
            }

            float TotalMass = 0f;

            foreach (PhysicalEntityPart Part in Parts)
            {
                TotalMass += Part.Mass;
            }

            _cachedMass = TotalMass;
            return TotalMass;
        }
    }

    internal virtual Vector2 CenterOfMass
    {
        get
        {
            float TotalMass = Mass;
            Vector2 CenterPosition = Vector2.Zero;

            foreach (PhysicalEntityPart Part in Parts)
            {
                CenterPosition += Part.Position * (Part.Mass / TotalMass);
            }

            return CenterPosition;
        }
    }

    internal bool IsAttractable { get; set; } = true;

    internal bool IsConductable { get; set; } = true;


    /* Parts. */
    internal virtual PhysicalEntityPart MainPart
    {
        get => _mainpart;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _mainpart = value;
            _cachedParts = null;
            _cachedMass = null;
            CreateBoundingBox();
            SetPositionAndRotation(Position, Rotation);
        }
    }

    internal virtual PhysicalEntityPart[] Parts
    {
        get
        {
            if (_cachedParts != null)
            {
                return _cachedParts;
            }

            List<PhysicalEntityPart> EntityParts = new();
            
            void GetSubParts(PhysicalEntityPart part)
            {
                EntityParts.Add(part);

                foreach (PartLink Link in part.SubPartLinks)
                {
                    GetSubParts(Link.LinkedPart);
                }
            }

            if (MainPart != null)
            {
                GetSubParts(MainPart);
            }

            _cachedParts = EntityParts.ToArray();
            return _cachedParts;
        }
    }


    /* Drawing. */
    internal virtual DrawLayer DrawLayer { get; set; } = DrawLayer.Bottom;

    internal bool IsDebugInfoDrawn { get; set; } = false;

    


    /* Events. */
    internal event EventHandler<CollisionEventArgs>? Collision;

    internal event EventHandler? CollisionBoundChange;

    internal event EventHandler? PartChange;

    internal event EventHandler<PartDamageEventArgs>? PartDamage;

    internal event EventHandler<PartDamageEventArgs>? PartDesotry;

    internal event EventHandler<PartDamageEventArgs>? PartBreakOff;


    // Protected fields.
    protected Vector2 SelfPosition;
    protected float SelfRotation;

    protected const float MIN_MOTION = -1_000_000f;
    protected const float MAX_MOTION = 1_000_000f;
    protected const float MIN_POSITION = -100_000f;
    protected const float MAX_POSITION = 100_000f;

    protected readonly HashSet<PhysicalEntity> EntitiesCollidedWith = new();


    // Private fields.
    private PhysicalEntityPart _mainpart;

    private PhysicalEntityPart[]? _cachedParts = null;
    private float? _cachedMass = null;

    private Vector2 _motion;
    private float _angularMotion;

    private readonly HashSet<PhysicalEntity> _collisionIgnorableEntities = new();
    

    // Constructors.
    internal PhysicalEntity(EntityType type, GameWorld? world)
        : this(type, world, Vector2.Zero) { }

    internal PhysicalEntity(EntityType type, GameWorld? world, Vector2 position)
        : this(type, world, position, 0f) { }

    internal PhysicalEntity(EntityType type, GameWorld? world, Vector2 position, float rotation)
        : base(type, world)
    {
        Rotation = rotation;
        Position = position;
    }


    // Internal Methods.
    /* Physics. */
    internal virtual void ApplyForce(Vector2 force, Vector2 location)
    {
        // Linear.
        Vector2 LinearAcceleration = force / Mass;

        // Rotational.
        Vector2 LeverArm = Vector2.Normalize(location - Position);
        Vector2 LeverArmNormal = GHMath.PerpVectorClockwise(LeverArm);
        float DistanceFromCenter = Vector2.Distance(CenterOfMass, location);
        float AppliedTorque = Vector2.Dot(force, LeverArmNormal) * DistanceFromCenter;
        float MomentOfInertia = Vector2.DistanceSquared(Position, location) * Mass;
        float AngularAcceleration = AppliedTorque / MomentOfInertia;

        // Apply.
        Motion += LinearAcceleration;
        AngularMotion += AngularAcceleration; 
    }

    internal Vector2 GetAngularMotionAtPoint(Vector2 point)
    {
        Vector2 PositionToPoint = point - Position;

        float Circumference = PositionToPoint.Length() * 2f * MathF.PI;
        float AngularSpeed = Circumference * (AngularMotion / (MathHelper.TwoPi));

        Vector2 NormalizedMotionDirection = Vector2.Normalize(GHMath.PerpVectorClockwise(PositionToPoint));

        return NormalizedMotionDirection * AngularSpeed;
    }

    /* Collision. */
    internal virtual bool TestCollisionAgainstEntity(PhysicalEntity entity, out CollisionCase[] collisions)
    {
        collisions = Array.Empty<CollisionCase>();

        // Early exits and special cases.
        if (!IsCollisionEnabled || !entity.IsCollisionEnabled ||
            _collisionIgnorableEntities.Contains(entity) || entity._collisionIgnorableEntities.Contains(this)
            || EntitiesCollidedWith.Contains(entity) || entity.EntitiesCollidedWith.Contains(this))
        {
            return false;
        }

        // Test bounding circles.
        if (Vector2.Distance(Position, entity.Position) > (BoundingLength + entity.BoundingLength))
        {
            return false;
        }

        // Test parts against entity.
        List<CollisionCase> Collisions = null!;

        foreach (PhysicalEntityPart SelfPart in Parts)
        {
            CollisionCase[] CollisionsInPart = SelfPart.TestCollisionAgainstEntity(entity);
            
            if (CollisionsInPart.Length != 0)
            {
                Collisions ??= new();
                Collisions.AddRange(CollisionsInPart);
            }
        }

        collisions = Collisions?.ToArray() ?? Array.Empty<CollisionCase>();

        if (collisions.Length != 0)
        {
            EntitiesCollidedWith.Add(entity);

            entity.EntitiesCollidedWith.Add(this);
            return true;
        }
        return false;
    }

    internal virtual void OnCollision(PhysicalEntity otherEntity,
        PhysicalEntityPart selfPart,
        PhysicalEntityPart otherPart,
        Vector2 surfaceNormal,
        Vector2 collisionPoint)
    {
        if (!IsCollisionReactionEnabled) return;

        if (selfPart.MaterialInstance.State != WorldMaterialState.Solid
            || otherPart.MaterialInstance.State != WorldMaterialState.Solid)
        {
            OnSoftCollision(otherEntity, collisionPoint);
        }
        else
        {
            OnHardCollision(otherEntity, selfPart, otherPart, surfaceNormal, collisionPoint);
        }
    }

    internal virtual void PushOutOfOtherEntity(ICollisionBound selfBound,
        ICollisionBound otherbound,
        PhysicalEntity otherEntity,
        PhysicalEntityPart selfPart,
        PhysicalEntityPart otherPart)
    {
        if (otherPart.MaterialInstance.State != WorldMaterialState.Solid
            || selfPart.MaterialInstance.State != WorldMaterialState.Solid)
        {
            return;
        }

        // Prepare variables.
        Vector2 PushOutDirection = GetPushOutDirection(otherEntity);
        const int StepCount = 8;
        float StepDistance = 10f;

        // First step.
        SelfPosition += StepDistance * PushOutDirection;

        // Consequent steps.
        Vector2 ClosestPushOutPosition = Position;
        bool IsColliding = false;

        for (int Step = 0; Step < StepCount; Step++)
        {
            StepDistance *= 0.5f;
            Position += IsColliding ? (PushOutDirection * StepDistance) : (-PushOutDirection * StepDistance);

            var CollisionData = selfPart.TestBoundAgainstBound(
                selfBound, otherbound, otherPart);
            IsColliding = CollisionData != null;

            if (!IsColliding)
            {
                ClosestPushOutPosition = Position;
            }
        }

        // Update position.
        Position = ClosestPushOutPosition;
    }

    internal void AddCollisionIgnorable(PhysicalEntity entity)
    {
        if  (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (_collisionIgnorableEntities.Add(entity))
        {
            entity.EntityDelete += OnCollisionIgnorableEntityDeleteEvent;
        }
    }

    internal void RemoveCollisionIgnorable(PhysicalEntity entity)
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

    internal bool IsCollisionIgnored(PhysicalEntity targetEntity)
    {
        return _collisionIgnorableEntities.Contains(targetEntity);
    }


    /* Part events. */
    internal virtual void OnPartCollisionBoundChange()
    {
        _cachedMass = null;
        CreateBoundingBox();
        CollisionBoundChange?.Invoke(this, EventArgs.Empty);
    }

    internal virtual void OnPartChange()
    {
        _cachedParts = null;
        _cachedMass = null;
        CreateBoundingBox();
        PartChange?.Invoke(this, EventArgs.Empty);
    }

    internal virtual void OnPartLinkDistanceChange()
    {
        CreateBoundingBox();
    }

    internal virtual void OnPartDamage(PartDamageEventArgs args)
    {
        PartDamage?.Invoke(this, args);
    }

    internal virtual void OnPartDestroy(PartDamageEventArgs args)
    {
        PartDesotry?.Invoke(this, args);
    }

    internal virtual void OnPartBreakOff(PartDamageEventArgs args)
    {
        PartBreakOff?.Invoke(this, args);
    }


    // Protected methods.
    /* Physics. */
    [TickedFunction(false)]
    protected virtual void StepMotion()
    {
        Vector2 NewPosition = SelfPosition + (Motion * World!.PassedTimeSeconds);
        float NewRotation = SelfRotation + (AngularMotion * World!.PassedTimeSeconds);
        SetPositionAndRotation(NewPosition, NewRotation);
    }

    /* Collision. */
    protected virtual Vector2 GetPushOutDirection(PhysicalEntity otherEntity)
    {
        // Try self motion.
        Vector2 Direction = Position - otherEntity.Position;

        if (Direction.Length() is 0f or -0f)
        {
            Direction = Vector2.UnitX;
        }

        return Vector2.Normalize(Direction);
    }

    protected float CalculateSpeedOnAxis(float v1, float m1, float v2, float m2, float cor)
    {
        return (m1 * v1 + m2 * v2 + m2*(v2 - v1)) / (m1 + m2) * cor;
    }

    protected virtual void CollisionTick()
    {
        EntitiesCollidedWith.Clear();
        World!.AddPhysicalEntityToWorldPart(this);
    }


    /* Other. */
    protected void SetPositionAndRotation(Vector2 position, float rotation)
    {
        SelfPosition = position;
        SelfPosition.X = float.IsNaN(SelfPosition.X) ? 0f : Math.Clamp(SelfPosition.X, MIN_POSITION, MAX_POSITION);
        SelfPosition.Y = float.IsNaN(SelfPosition.Y) ? 0f : Math.Clamp(SelfPosition.Y, MIN_POSITION, MAX_POSITION);
        SelfRotation = Math.Clamp(rotation, float.MinValue, float.MaxValue);

        _mainpart?.SetPositionAndRotation(SelfPosition, SelfRotation);
    }


    // Private methods.
    /* Drawing */
    private void DrawHitboxes()
    {
        if (MainPart == null) return;

        PhysicalEntityPart[] EntityParts = Parts;

        foreach (PhysicalEntityPart Part in EntityParts)
        {
            Part.DrawCollisionBounds();
        }
    }

    private void DrawMotion()
    {
        Display.SharedLine.Thickness = 5f * World!.Zoom;
        Display.SharedLine.Mask = Color.Green;
        Display.SharedLine.Set(Position * World!.Zoom + World.ObjectVisualOffset,
            (Position + Motion / 2f) * World.Zoom + World.ObjectVisualOffset);
        Display.SharedLine.Draw();
    }

    private void DrawCenterOfMass()
    {
        Display.SharedLine.Thickness = 10f * World!.Zoom;
        Display.SharedLine.Mask = Color.Yellow;
        Display.SharedLine.Set((CenterOfMass - new Vector2(5f, 0f)) * World.Zoom + World.ObjectVisualOffset,
            Display.SharedLine.Thickness, 0f);
        Display.SharedLine.Draw();
    }

    private void DrawBoundingBox()
    {
        Display.SharedLine.Thickness = 5f * World!.Zoom;
        Display.SharedLine.Mask = Color.Khaki;

        Display.SharedLine.Set(
            World.ToViewportPosition(Position + new Vector2(-BoundingLength, 0f)),
            World.ToViewportPosition(Position + new Vector2(BoundingLength, 0f)));
        Display.SharedLine.Draw();

        Display.SharedLine.Set(
            World.ToViewportPosition(Position + new Vector2(0f, -BoundingLength)),
            World.ToViewportPosition(Position + new Vector2(0f, BoundingLength)));
        Display.SharedLine.Draw();
    }


    /* Collision. */
    private void CreateBoundingBox()
    {
        if (MainPart == null)
        {
            return;
        }

        List<Vector2> Points = GetAllPointsInCollisionBounds();

        if (Points.Count == 0)
        {
            return;
        }

        Vector2? FurthestPoint = null;
        foreach (Vector2 point in Points)
        {
            if ((FurthestPoint == null) || (point.LengthSquared() > FurthestPoint.Value.LengthSquared()))
            {
                FurthestPoint = point;
            }
        }

        BoundingLength = FurthestPoint!.Value.Length();
    }

    private List<Vector2> GetAllPointsInCollisionBounds()
    {
        List<Vector2> Points = new();

        foreach (PhysicalEntityPart Part in Parts)
        {
            if (Part.CollisionBounds == null)
            {
                continue;
            }

            Vector2 PartPosition = Vector2.Zero;
            foreach (PartLink Link in Part.GetPathFromMainPart())
            {
                PartPosition += Link.LinkDistance;
            }

            foreach (ICollisionBound Bound in Part.CollisionBounds)
            {
                Points.AddRange(GetCollisionBoundPoints(Bound, PartPosition));
            }
        }

        return Points;
    }

    private Vector2[] GetCollisionBoundPoints(ICollisionBound bound, Vector2 partPosition)
    {
        Vector2 BoundPosition = partPosition + bound.Offset;

        switch (bound.Type)
        {
            case CollisionBoundType.Rectangle:
                RectangleCollisionBound RectBound = (RectangleCollisionBound)bound;
                float LongestEdgeLength = Math.Max(RectBound.HalfSize.X, RectBound.HalfSize.Y);

                return new Vector2[]
                {
                BoundPosition + new Vector2(LongestEdgeLength, LongestEdgeLength),
                BoundPosition + new Vector2(-LongestEdgeLength, -LongestEdgeLength),
                BoundPosition + new Vector2(-LongestEdgeLength, LongestEdgeLength),
                BoundPosition + new Vector2(LongestEdgeLength, -LongestEdgeLength),
                };


            case CollisionBoundType.Ball:
                BallCollisionBound BallBound = (BallCollisionBound)bound;

                Vector2 NormalToBallPoint = BoundPosition -partPosition;
                if (NormalToBallPoint.LengthSquared() == 0)
                {
                    NormalToBallPoint = Vector2.UnitX;
                }
                else
                {
                    NormalToBallPoint = Vector2.Normalize(NormalToBallPoint);
                }

                return new Vector2[]
                {
                    BoundPosition + NormalToBallPoint * BallBound.Radius
                };


            default:
                throw new NotImplementedException("Getting collision bound points is not supported for bound type" +
                    $"of \"{bound.Type}\" (int value of {(int)bound.Type})");
        }
    }

    private void OnCollisionIgnorableEntityDeleteEvent(object? sender, EventArgs args)
    {
        if (sender != null)
        {
            _collisionIgnorableEntities.Remove((PhysicalEntity)sender);
        }
    }

    private void OnSoftCollision(PhysicalEntity otherEntity, Vector2 collisionPoint)
    {
        // Theoretically this code can cause strange behavior of objects pushing out of each other,
        // but practically it is rare. In case of such a bug, search here.

        Vector2 MotionRelativeToOtherEntity = Motion - otherEntity.Motion;

        const float ARBITRARY_SPEED_CHANGE_VALUE = 11.195f;
        float Multiplier = Math.Max(0f, 1f - (ARBITRARY_SPEED_CHANGE_VALUE * World!.PassedTimeSeconds));

        Vector2 ChangeInRelativeMotion = MotionRelativeToOtherEntity - (MotionRelativeToOtherEntity * Multiplier);
        Motion -= ChangeInRelativeMotion;

        AngularMotion *= Multiplier;

        Collision?.Invoke(this, new(collisionPoint, 0f));
    }

    private void OnHardCollision(PhysicalEntity otherEntity,
        PhysicalEntityPart selfPart,
        PhysicalEntityPart otherPart,
        Vector2 surfaceNormal,
        Vector2 collisionPoint)
    {
        // Multiply by 0.5 so physics are more stable.
        Vector2 MotionAtPoint = GetAngularMotionAtPoint(collisionPoint) * 0.5f + Motion;
        Vector2 EntityBMotionAtPoint = otherEntity.GetAngularMotionAtPoint(collisionPoint) * 0.5f + otherEntity.Motion;

        // So there's supposed to be some stuff done here with momentum conservation v1m + v2m = v1`m + v2`m,
        // but it just doesn't seem to work and I cannot get the calculations to make sense no matter what.
        // So here I just stole some formula from online and I don't know how it works (Since my calculations give different results)
        Vector2 Surface = GHMath.PerpVectorClockwise(surfaceNormal);
        float AlignedYMotion = Vector2.Dot(MotionAtPoint, surfaceNormal);
        float AlignedXMotion = Vector2.Dot(MotionAtPoint, Surface);
        float EntityBAlignedYMotion = Vector2.Dot(EntityBMotionAtPoint, surfaceNormal);

        float CombinedBounciness = (selfPart.MaterialInstance.Material.Bounciness
            + otherPart.MaterialInstance.Material.Bounciness) * 0.5f;
        float CombinedFrictionCoef = (selfPart.MaterialInstance.Material.Friction
            + otherPart.MaterialInstance.Material.Friction) * 0.5f;


        float MotionY = CalculateSpeedOnAxis(AlignedYMotion, Mass,
            EntityBAlignedYMotion, otherEntity.Mass, CombinedBounciness);

        Vector2 NewMotion = (surfaceNormal * MotionY) + (Surface * AlignedXMotion * CombinedFrictionCoef);
        Vector2 ForceApplied = Mass * (NewMotion - MotionAtPoint);


        // Apply forces.
        ApplyForce(ForceApplied, collisionPoint);
        selfPart.OnCollision(collisionPoint, ForceApplied.Length());
        Collision?.Invoke(this, new(collisionPoint, ForceApplied.Length()));
    }


    // Inherited methods.
    internal override void Load(GHGameAssetManager assetManager)
    {
        foreach (PhysicalEntityPart Part in Parts)
        {
            Part.Load(assetManager);
        }
    }

    [TickedFunction(false)]
    internal override void ParallelTick()
    {
        StepMotion();
        MainPart.ParallelTick();
    }

    [TickedFunction(false)]
    internal override void SequentialTick()
    {
        CollisionTick();
        MainPart.SequentialTick();
    }

    public virtual void Draw()
    {
        if (IsVisible)
        {
            MainPart.Draw();
        }
        if (IsDebugInfoDrawn)
        {
            DrawBoundingBox();
            DrawHitboxes();
            DrawMotion();
            DrawCenterOfMass();
        }
    }
}