using GardenHose.Game.World.Material;
using GardenHoseEngine;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;


namespace GardenHose.Game.World.Entities;

internal class PhysicalEntityPart
{
    // Fields.
    internal bool IsMainPart => ParentLink == null;

    internal virtual Vector2 Position
    {
        get
        {
            Vector2 Position = Entity.Position;
            PartLink? Link = ParentLink;

            while (Link != null)
            {
                Position += Vector2.Transform(Link.LinkDistance, Matrix.CreateRotationZ(Link.ParentPart.Rotation));
                Link = Link.ParentPart.ParentLink;
            }

            return Position;
        }
    }

    internal virtual float SelfRotation
    {
        get => _selfRotation;
        set
        {
            _selfRotation = value;
            UpdateRotation();
        }
    }

    internal virtual float Rotation { get; set; }

    internal virtual float LinkedRotation
    {
        get => _linkedRotation;
        set
        {
            _linkedRotation = value;

            if (SubPartLinks != null)
            {
                foreach (PartLink Link in SubPartLinks)
                {
                    Link.LinkedPart.LinkedRotation = FullRotation;
                }
            }

            Position = _position; // Forces linked part positions to update (they are affected by rotation).
        }
    }

    internal virtual float FullRotation => SelfRotation + LinkedRotation;

    internal virtual float AngularMotion { get; set; }

    public virtual float Mass
    {
        get
        {
            float TotalMass = 0f;

            foreach (ICollisionBound CBound in CollisionBounds)
            {
                TotalMass += CBound.GetArea() * Material.Density;
            }

            return TotalMass;
        }
    }

    public virtual float Temperature => Material.Temperature;

    [MemberNotNull(nameof(_collisionBounds))]
    internal virtual ICollisionBound[] CollisionBounds
    {
        get => _collisionBounds;
        set
        {
            _collisionBounds = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    internal virtual PartLink[] SubPartLinks { get; private set; }

    internal virtual PartLink? ParentLink { get; set; }

    internal virtual PhysicalEntity Entity { get; set; }

    internal virtual WorldMaterialInstance Material { get; set; }


    // Private fields.
    private ICollisionBound[] _collisionBounds;
    private float _selfRotation = 0f;
    private float _linkedRotation = 0f;


    // Constructors.
    internal PhysicalEntityPart(ICollisionBound[] collisionBounds, WorldMaterial material)
        : this(collisionBounds, material.CreateInstance()) { }

    internal PhysicalEntityPart(ICollisionBound[] collisionBounds, WorldMaterialInstance material)
    {
        CollisionBounds = collisionBounds;
        Material = material;
    }


    // Internal methods.
    internal void LinkPart(PhysicalEntityPart part, Vector2 distance, bool isRotationLinked)
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
        NewPartLinks[^1] = new PartLink(this, part, distance, isRotationLinked);
    }

    internal void LinkEntityAsPart(PhysicalEntity entity, Vector2 distance, bool isRotationLinked)
    {
        LinkPart(entity.MainPart, distance, isRotationLinked);
    }

    internal Vector2 GetCombinedMotionAtPoint(Vector2 point)
    {
        Vector2 Motion = Entity.Motion;
        PhysicalEntityPart? Part = this;

        do
        {
            if (Part.AngularMotion != 0f)
            {
                /* Get vector from point to part, rotate it 90 or -90 degrees depending on the angular motion,
                 * normalize it, then multiply it by speed at location, finally add it to the motion. */
                Vector2 AngularMotionVector = point - Part.Position;
                AngularMotionVector = Part.FullRotation > 0f ? GHMath.PerpVectorClockwise(AngularMotionVector)
                    : GHMath.PerpVectorCounterClockwise(AngularMotionVector);
                AngularMotionVector = Vector2.Normalize(AngularMotionVector);
                AngularMotionVector *= (MathF.PI * 2f * (point - Part.Position).Length())
                    * (Part.AngularMotion / (2f * MathF.PI));

                Motion += AngularMotionVector;
            }

            Part = Part.ParentLink?.ParentPart;
        }
        while (Part != null);

        return Motion;
    }

    internal void Tick()
    {
        SelfRotation += AngularMotion;
    }

    internal void Draw()
    {

    }

    internal void DrawCollisionBounds()
    {
        foreach (ICollisionBound Bound in CollisionBounds)
        {
            Bound.Draw(Position, FullRotation, Entity.World);
        }
    }


    // Private methods.
    private void UpdateRotation()
    {
        if (SubPartLinks != null)
        {
            foreach (PartLink Link in SubPartLinks)
            {
                Link.LinkedPart.LinkedRotation = FullRotation;
            }
        }

        Position = _position; // Forces linked part positions to update (they are affected by rotation).
    }
}