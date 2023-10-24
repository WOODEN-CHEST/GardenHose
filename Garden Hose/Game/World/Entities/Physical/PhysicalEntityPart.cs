using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;


namespace GardenHose.Game.World.Entities;

internal class PhysicalEntityPart
{
    // Fields.
    internal virtual Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;

            if (LinkedParts == null) return;

            Matrix RotationMatrix = Matrix.CreateRotationZ(FullRotation);

            foreach (PartLink Link in LinkedParts)
            {
                Link.LinkedPart.Position = Link.IsRotationLinked ?
                   Vector2.Transform(Link.LinkDistance, RotationMatrix) + Position
                   : Position + Link.LinkDistance;
            }
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

    internal virtual float LinkedRotation
    {
        get => _linkedRotation;
        set
        {
            _linkedRotation = value;

            if (LinkedParts != null)
            {
                foreach (PartLink Link in LinkedParts)
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

    internal virtual float Temperature { get; set; }

    [MemberNotNull(nameof(_collisionBounds))]
    internal virtual ICollisionBound[] CollisionBounds
    {
        get => _collisionBounds;
        set
        {
            _collisionBounds = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    internal virtual PartLink[]? LinkedParts { get; set; }

    internal virtual WorldMaterialInstance Material { get; set; }


    // Private fields.
    private ICollisionBound[] _collisionBounds;
    private Vector2 _position = Vector2.Zero;
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


    // Private methods.
    private void UpdateRotation()
    {
        if (LinkedParts != null)
        {
            foreach (PartLink Link in LinkedParts)
            {
                Link.LinkedPart.LinkedRotation = FullRotation;
            }
        }

        Position = _position; // Forces linked part positions to update (they are affected by rotation).
    }
}