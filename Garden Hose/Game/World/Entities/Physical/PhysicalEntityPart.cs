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
                    TotalMass += CBound.GetArea() * Material.Density;
                }
            }  

            return TotalMass;
        }
    }

    public virtual float Temperature => Material.Temperature;

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

    internal virtual WorldMaterialInstance Material { get; set; }

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
        Material = material;
    }


    // Internal methods.
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

        PartLink Link = new PartLink(this, part, distance);
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

    internal List<PhysicalEntityPart> GetPathFromMainPart()
    {
        List<PhysicalEntityPart> Parts = new();
        PartLink? Link = ParentLink;

        while (Link != null)
        {
            Parts.Insert(0, Link.ParentPart);
            Link = Link.ParentPart.ParentLink;
        }

        return Parts;
    }

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
                AngularMotionVector = Part.SelfRotation > 0f ? GHMath.PerpVectorClockwise(AngularMotionVector)
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
}