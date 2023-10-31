using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;
using GardenHoseEngine;
using System.Runtime.CompilerServices;
using GardenHose.Game.World.Entities.Physical;

namespace GardenHose.Game.World.Entities;


internal abstract class PhysicalEntity : Entity, IDrawableItem
{
    // Fields.
    public virtual bool IsVisible { get; set; } = true;

    public virtual Effect? Shader { get; set; }


    // Internal fields.
    internal sealed override bool IsPhysical => true;

    internal virtual Vector2 Position { get; set; }

    internal virtual Vector2 Motion { get; set; }

    internal virtual float Rotation { get; set; }

    internal virtual float AngularMotion { get; set; }

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

    internal virtual PhysicalEntityPart MainPart
    {
        get => _mainpart;
        set
        {
            _mainpart = value;
            _cachedParts = null;
            _cachedMass = null;
            CreateBoundingBox();
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

    internal virtual DrawLayer DrawLayer { get; set; } = DrawLayer.Bottom;

    internal bool AreCollisionBoundsDrawn { get; set; } = false;

    internal bool IsMotionDrawn { get; set; } = false;

    internal bool IsCenterOfMassDrawn { get; set; } = false;

    internal bool IsBoundingBoxDrawn { get; set; } = false;

    internal event EventHandler<Vector2> Collision;

    internal event EventHandler CollisionBoundChange;

    internal event EventHandler ParentChange;

    internal event EventHandler SubPartChange;


    // Private fields.
    private PhysicalEntityPart _mainpart;
    private PhysicalEntityPart[]? _cachedParts = null;
    private float? _cachedMass = null;



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
        // Scuffed code that weirdly calculates force and straight up discards some of it.
        float CurrentMass = Mass;
        const float ROTATION_SANITY_MULTIPLIER = 0.02f; // Keep rotation from becoming ridiculous.

        // Linear motion.
        Motion += force / CurrentMass;

        // Rotation.
        Vector2 LeverArm = location - Position;
        Vector2 LeverArmNorm = Vector2.Normalize(GHMath.PerpVectorClockwise(LeverArm));
        float AppliedForce = Vector2.Dot(force, LeverArmNorm) * ROTATION_SANITY_MULTIPLIER;

        AngularMotion += AppliedForce / CurrentMass;
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
    internal virtual void TestCollisionAgainstEntity(PhysicalEntity entity)
    {
        // Test bounding circles.
        if (Vector2.Distance(Position, entity.Position) > (BoundingLength + entity.BoundingLength))
        {
            return;
        }

        // Test parts against entity.
        foreach (PhysicalEntityPart SelfPart in Parts)
        {
            SelfPart.TestCollisionAgainstEntity(entity);
        }
    }


    /* Part related. */
    internal void PartCollision(CollisionCase collisionCase) 
    {
        Vector2 MotionHitAt = Motion - collisionCase.EntityB.Motion;

        PushOutOfOtherEntity(collisionCase);
        collisionCase.EntityB.Motion = Vector2.Reflect(Motion, collisionCase.SurfaceNormal);
    }

    internal void PartCollisionBoundChange()
    {
        _cachedMass = null;
        CreateBoundingBox();
    }

    internal void PartChange()
    {
        _cachedParts = null;
        _cachedMass = null;
        CreateBoundingBox();
    }

    internal void PartLinkDistanceChange()
    {
        CreateBoundingBox();
    }


    // Protected methods.
    /* Physics. */
    protected virtual void SimulatePhysicsPlanet()
    {
        float AttractionStrength = (World!.Planet.Radius / Vector2.Distance(Position, World.Planet.Position))
            * World.Planet.Attraction * World!.PassedTimeSeconds;
        AttractionStrength = float.IsFinite(AttractionStrength) ? AttractionStrength : 0f;

        Vector2 AddedMotion = -Vector2.Normalize(Position - World.Planet.Position);
        AddedMotion *= AttractionStrength;

        Motion += AddedMotion;
    }

    protected virtual void FinalizeTickSimulation()
    {
        Rotation += AngularMotion * World!.PassedTimeSeconds;
        Position += Motion * World!.PassedTimeSeconds;
    }

    protected virtual void PushOutOfOtherEntity(CollisionCase collisionCase)
    {
        if (collisionCase.BoundA.Type == CollisionBoundType.Rectangle
            && collisionCase.BoundB.Type == CollisionBoundType.Ball)
        {
            //PushOutOfBall(collisionCase, ())
        }
    }

    protected virtual void PushOutOfBall(CollisionCase collisionCase, BallCollisionBound ball)
    {

    }


    /* Other */
    protected virtual void DrawCollisionBounds()
    {
        if (MainPart == null) return;

        PhysicalEntityPart[] EntityParts = Parts;

        foreach (PhysicalEntityPart Part in EntityParts)
        {
            Part.DrawCollisionBounds();
        }
    }

    protected void DrawMotion()
    {
        ICollisionBound.VisualLine.Thickness = 5f * World!.Zoom;
        ICollisionBound.VisualLine.Mask = Color.Green;
        ICollisionBound.VisualLine.Set(Position * World!.Zoom + World.ObjectVisualOffset,
            (Position + Motion / 2f) * World.Zoom + World.ObjectVisualOffset);
        ICollisionBound.VisualLine.Draw();
    }

    protected void DrawCenterOfMass()
    {
        ICollisionBound.VisualLine.Thickness = 10f * World!.Zoom;
        ICollisionBound.VisualLine.Mask = Color.Yellow;
        ICollisionBound.VisualLine.Set((CenterOfMass - new Vector2(5f, 0f)) * World.Zoom + World.ObjectVisualOffset, 
            ICollisionBound.VisualLine.Thickness, 0f);
        ICollisionBound.VisualLine.Draw();
    }

    protected void DrawBoundingBox()
    {
        ICollisionBound.VisualLine.Thickness = 5f * World!.Zoom;
        ICollisionBound.VisualLine.Mask = Color.Khaki;

        ICollisionBound.VisualLine.Set(
            World.ToViewportPosition(Position + new Vector2(-BoundingLength, 0f)),
            World.ToViewportPosition(Position + new Vector2(BoundingLength, 0f)));
        ICollisionBound.VisualLine.Draw();

        ICollisionBound.VisualLine.Set(
            World.ToViewportPosition(Position + new Vector2(0f, -BoundingLength)),
            World.ToViewportPosition(Position + new Vector2(0f, BoundingLength)));
        ICollisionBound.VisualLine.Draw();
    }

    /* Bounding box. */
    protected void CreateBoundingBox()
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


    // Private methods.
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
                Points.AddRange(GetCollisionBoundPoints(Part, Bound, PartPosition));
            }
        }

        return Points;
    }

    private Vector2[] GetCollisionBoundPoints(PhysicalEntityPart part, ICollisionBound bound, Vector2 partPosition)
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


    // Inherited methods.
    internal override void Tick()
    {
        SimulatePhysicsPlanet();
        FinalizeTickSimulation();

        MainPart.SetPositionAndRotation(Position, Rotation);
        MainPart.Tick();
    }

    public virtual void Draw()
    {
        if (IsVisible)
        {
            MainPart.Draw(AreCollisionBoundsDrawn);
        }
        if (IsMotionDrawn)
        {
            DrawMotion();
        }
        if (IsCenterOfMassDrawn)
        {
            DrawCenterOfMass();
        }
        if (IsBoundingBoxDrawn)
        {
            DrawBoundingBox();
        }
    }
}