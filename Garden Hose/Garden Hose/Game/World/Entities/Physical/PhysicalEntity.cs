using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;
using GardenHoseEngine;
using GardenHoseEngine.Screen;
using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical.Collision;

namespace GardenHose.Game.World.Entities.Physical;


internal abstract class PhysicalEntity : Entity, IDrawableItem
{
    // Fields.
    public virtual bool IsVisible { get; set; } = true;
    public virtual Effect? Shader { get; set; }


    // Internal static fields.
    internal const float ZINDEX_PLANET = 0.1f;
    internal const float ZINDEX_PROJECTILE = 0.3f;
    internal const float ZINDEX_STRAY = 0.45f;
    internal const float ZINDEX_DEFAULT_OBJECT = 0.5f;
    internal const float ZINDEX_PARTICLE = 0.75f;


    // Internal fields.
    internal sealed override bool IsPhysical => true;
    internal EntityCollisionHandler CollisionHandler { get; init; }
    internal bool IsInvulnerable { get; set; } = false;


    /* Entity properties. */
    internal virtual Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            SetPositionAndRotation(_position, Rotation);
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
        get => _rotation;
        set
        {
            _rotation = value;
            SetPositionAndRotation(Position, _rotation);
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
            float TotalMass = 0f;

            foreach (PhysicalEntityPart Part in Parts)
            {
                TotalMass += Part.Mass;
            }

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
    internal bool IsForceApplicable { get; set; } = true;
    internal bool IsPositionLocked { get; set; } = false;
    internal bool IsRotationLocked { get; set; } = false;


    /* Common math. */
    internal CommonEntityMath CommonMath { get; init; }


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
            _mainpart.Entity = this;
            UpdateAllPositionsAndRotations();
            ResetPartInfo();
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
    internal virtual float ZIndex { get; init; } = ZINDEX_DEFAULT_OBJECT;
    internal bool IsDebugInfoDrawn { get; set; } = false;


    // Protected fields.
    protected const float MIN_MOTION = -1_000_000f;
    protected const float MAX_MOTION = 1_000_000f;
    protected const float MIN_POSITION = -100_000f;
    protected const float MAX_POSITION = 100_000f;


    // Private fields.
    private PhysicalEntityPart _mainpart;

    private PhysicalEntityPart[]? _cachedParts = null;

    private Vector2 _position;
    private float _rotation;
    private Vector2 _motion;
    private float _angularMotion;

   
    

    // Constructors.
    internal PhysicalEntity(EntityType type)
        : this(type, Vector2.Zero) { }

    internal PhysicalEntity(EntityType type, Vector2 position)
        : this(type, position, 0f) { }

    internal PhysicalEntity(EntityType type, Vector2 position, float rotation)
        : base(type)
    {
        Rotation = rotation;
        Position = position;

        CommonMath = new(this);
        CollisionHandler = new(this);
    }


    // Internal Methods.
    /* Physics. */
    internal virtual void ApplyForce(Vector2 force, Vector2 location)
    {
        if (!IsForceApplicable)
        {
            return;
        }

        // Linear.
        Vector2 LinearAcceleration = force / Mass;

        // Rotational.
        float AngularAcceleration = 0f;
        Vector2 LeverArm = location - Position;
        if (!(LeverArm.LengthSquared() is 0f or -0f))
        {
            LeverArm.Normalize();
            Vector2 LeverArmNormal = GHMath.PerpVectorClockwise(LeverArm);
            float DistanceFromCenter = Vector2.Distance(CenterOfMass, location);
            float AppliedTorque = Vector2.Dot(force, LeverArmNormal) * DistanceFromCenter;
            float MomentOfInertia = Vector2.DistanceSquared(Position, location) * Mass;
            AngularAcceleration = AppliedTorque / MomentOfInertia;
        }

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

    internal void UpdateAllPositionsAndRotations()
    {
        SetPositionAndRotation(Position, Rotation);
    }

    internal virtual void OnCollision(CollisionEventArgs args) { }


    /* Part events. */
    internal void ResetPartInfo()
    {
        _cachedParts = null;
        CollisionHandler.CreateBoundingBox();
    }


    // Protected methods.
    /* Physics. */
    [TickedFunction(false)]
    protected virtual void StepMotion(GHGameTime time)
    {
        Vector2 NewPosition = _position;
        if (!IsPositionLocked)
        {
            NewPosition += (Motion * time.WorldTime.PassedTimeSeconds);
        }

        float NewRotation = Rotation;
        if (!IsRotationLocked)
        {
            NewRotation += AngularMotion * time.WorldTime.PassedTimeSeconds;
        }
        SetPositionAndRotation(NewPosition, NewRotation);
    }


    // Private methods.
    private void SetPositionAndRotation(Vector2 position, float rotation)
    {
        _position = position;
        _position.X = float.IsNaN(_position.X) ? 0f : Math.Clamp(_position.X, MIN_POSITION, MAX_POSITION);
        _position.Y = float.IsNaN(_position.Y) ? 0f : Math.Clamp(_position.Y, MIN_POSITION, MAX_POSITION);
        _rotation = Math.Clamp(rotation, float.MinValue, float.MaxValue);

        _mainpart?.SetPositionAndRotation(_position, _rotation);
    }


    /* Drawing */
    private void DrawHitboxes(IDrawInfo info)
    {
        if (MainPart == null) return;

        PhysicalEntityPart[] EntityParts = Parts;

        foreach (PhysicalEntityPart Part in EntityParts)
        {
            Part.DrawCollisionBounds(info);
        }
    }

    private void DrawMotion(IDrawInfo info)
    {
        Display.SharedLine.Thickness = 5f * World!.Player.Camera.Zoom;
        Display.SharedLine.Mask = Color.Green;
        Display.SharedLine.Set(Position * World.Player.Camera.Zoom + World.Player.Camera.ObjectVisualOffset,
            (Position + Motion / 2f) * World!.Player.Camera.Zoom + World.Player.Camera.ObjectVisualOffset);
        Display.SharedLine.Draw(info);
    }

    private void DrawCenterOfMass(IDrawInfo info)
    {
        Display.SharedLine.Thickness = 10f * World!.Player.Camera.Zoom;
        Display.SharedLine.Mask = Color.Yellow;
        Display.SharedLine.Set((CenterOfMass - new Vector2(5f, 0f)) * World!.Player.Camera.Zoom + World!.Player.Camera.ObjectVisualOffset,
            Display.SharedLine.Thickness, 0f);
        Display.SharedLine.Draw(info);
    }

    private void DrawBoundingBox(IDrawInfo info)
    {
        Display.SharedLine.Thickness = 5f * World!.Player.Camera.Zoom;
        Display.SharedLine.Mask = Color.Khaki;

        Display.SharedLine.Set(
            World!.Player.Camera.ToViewportPosition(Position + new Vector2(-CollisionHandler.BoundingRadius, 0f)),
            World.Player.Camera.ToViewportPosition(Position + new Vector2(CollisionHandler.BoundingRadius, 0f)));
        Display.SharedLine.Draw(info);

        Display.SharedLine.Set(
            World.Player.Camera.ToViewportPosition(Position + new Vector2(0f, -CollisionHandler.BoundingRadius)),
            World.Player.Camera.ToViewportPosition(Position + new Vector2(0f, CollisionHandler.BoundingRadius)));
        Display.SharedLine.Draw(info);
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
    internal override void Tick(GHGameTime time)
    {
        CommonMath.Calculate();

        StepMotion(time);
        CollisionHandler.SpacePartitionEntity();

        MainPart.Tick(time);
    }

    public virtual void Draw(IDrawInfo info)
    {
        if (IsVisible)
        {
            MainPart.Draw(info);
        }

        if (IsDebugInfoDrawn)
        {
            DrawBoundingBox(info);
            DrawHitboxes(info);
            DrawMotion(info);
            DrawCenterOfMass(info);
        }
    }
    internal override Entity CloneDataToObject(Entity newEntity)
    {
        base.CloneDataToObject(newEntity);

        PhysicalEntity Physical = (PhysicalEntity)newEntity;

        Physical.IsVisible = IsVisible;
        Physical.Shader = Shader;

        Physical.IsInvulnerable = IsInvulnerable;
        Physical.IsForceApplicable = IsForceApplicable;
        Physical.IsAttractable = IsAttractable;
        Physical.IsPositionLocked = IsPositionLocked;
        Physical.IsRotationLocked = IsRotationLocked;

        Physical._position = Position;
        Physical._rotation = Rotation;
        Physical._motion = Motion;
        Physical._angularMotion = AngularMotion;

        Physical._cachedParts = null;

        Physical.MainPart = MainPart.CreateClone(Physical);

        return newEntity;
    }

    internal override void OnAddToWorld()
    {
        base.OnAddToWorld();

        MainPart.OnAddToWorld();
    }

    internal override void Delete()
    {
        base.Delete();

        MainPart.OnDeleteFromWorld();
    }
}