using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;
using GardenHoseEngine;
using GardenHose.Game.World.Entities.Physical;
using GardenHoseEngine.Screen;
using System.Runtime.CompilerServices;

namespace GardenHose.Game.World.Entities;


internal abstract class PhysicalEntity : Entity, IDrawableItem
{
    // Fields.
    public virtual bool IsVisible { get; set; } = true;

    public virtual Effect? Shader { get; set; }


    // Internal fields.
    internal sealed override bool IsPhysical => true;


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

                if (part.SubPartLinks != null)
                {
                    foreach (PartLink Link in part.SubPartLinks)
                    {
                        GetSubParts(Link.LinkedPart);
                    }
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

    internal virtual float BoundingLength { get; private set; }


    /* Drawing. */
    internal virtual DrawLayer DrawLayer { get; set; } = DrawLayer.Bottom;

    internal bool IsDebugInfoDrawn { get; set; } = false;


    /* Events. */
    internal event EventHandler<Vector2>? Collision;

    internal event EventHandler? CollisionBoundChange;

    internal event EventHandler? ParentChange;

    internal event EventHandler? SubPartChange;


    // Protected fields.
    protected Vector2 SelfPosition;
    protected float SelfRotation;

    protected const float MIN_MOTION = -1_000_000f;
    protected const float MAX_MOTION = 1_000_000f;
    protected const float MIN_POSITION = -100_000f;
    protected const float MAX_POSITION = 100_000f;


    // Private fields.
    private PhysicalEntityPart _mainpart;

    private PhysicalEntityPart[]? _cachedParts = null;
    private float? _cachedMass = null;

    private Vector2 _motion;
    private float _angularMotion;



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
    internal virtual void ApplyForce(Vector2 force, Vector2 location, PhysicalEntityPart? part = null)
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
    internal virtual bool TestCollisionAgainstEntity(PhysicalEntity entity, out CollisionCase[]? collisions)
    {
        List<CollisionCase> Collisions = null!;

        // Test bounding circles.
        if (Vector2.Distance(Position, entity.Position) > (BoundingLength + entity.BoundingLength))
        {
            collisions = null;
            return false;
        }

        // Test parts against entity.
        foreach (PhysicalEntityPart SelfPart in Parts)
        {
            CollisionCase[] CollisionsInPart = SelfPart.TestCollisionAgainstEntity(entity);

            if (CollisionsInPart != null)
            {
                Collisions ??= new();
                Collisions.AddRange(CollisionsInPart);
            }
        }

        collisions = Collisions?.ToArray();
        return collisions != null;
    }

    internal virtual void OnCollision(PhysicalEntity otherEntity,
        PhysicalEntityPart selfPart,
        PhysicalEntityPart otherPart,
        ICollisionBound selfBound,
        ICollisionBound otherBound,
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
        ApplyForce(ForceApplied, collisionPoint, selfPart);
        selfPart.OnCollision(collisionPoint, ForceApplied.Length());
    }

    internal virtual void PushOutOfOtherEntity(ICollisionBound selfBound,
        ICollisionBound otherbound,
        PhysicalEntity otherEntity,
        PhysicalEntityPart selfPart,
        PhysicalEntityPart otherPart)
    {
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


    /* Part events. */
    internal virtual void OnPartCollisionBoundChange()
    {
        _cachedMass = null;
        CreateBoundingBox();
    }

    internal virtual void OnPartChange()
    {
        _cachedParts = null;
        _cachedMass = null;
        CreateBoundingBox();
    }

    internal virtual void OnPartLinkDistanceChange()
    {
        CreateBoundingBox();
    }

    internal virtual void OnPartDamage() { }

    internal virtual void OnPartBreak() { }


    // Protected methods.
    /* Physics. */
    protected virtual void SimulatePhysicsPlanet()
    {
        float AttractionStrength = (World!.Planet!.Radius / Vector2.Distance(Position, World.Planet.Position))
            * World.Planet.Attraction * World!.PassedTimeSeconds;
        AttractionStrength = float.IsFinite(AttractionStrength) ? AttractionStrength : 0f;

        Vector2 AddedMotion = -Vector2.Normalize(Position - World.Planet.Position);
        AddedMotion.X = float.IsFinite(AddedMotion.X) ? AddedMotion.X : 0f;
        AddedMotion.Y = float.IsFinite(AddedMotion.Y) ? AddedMotion.Y : 0f;

        AddedMotion *= AttractionStrength;

        Motion += AddedMotion;
    }

    protected virtual void FinalizeTickSimulation()
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

    /* Other. */
    private void SetPositionAndRotation(Vector2 position, float rotation)
    {
        SelfPosition = position;
        SelfPosition.X = float.IsNaN(SelfPosition.X) ? 0f : Math.Clamp(SelfPosition.X, MIN_POSITION, MAX_POSITION);
        SelfPosition.Y = float.IsNaN(SelfPosition.Y) ? 0f : Math.Clamp(SelfPosition.Y, MIN_POSITION, MAX_POSITION);
        SelfRotation = Math.Clamp(rotation, float.MinValue, float.MaxValue);

        _mainpart?.SetPositionAndRotation(SelfPosition, SelfRotation);
    }


    // Inherited methods.
    internal override void Tick()
    {
        if (World!.Planet != null)
        {
            SimulatePhysicsPlanet();
        }
        FinalizeTickSimulation();

        MainPart.Tick();
        World!.AddPhysicalEntityToWorldPart(this);
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