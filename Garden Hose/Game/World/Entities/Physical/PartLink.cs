using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities;

internal class PartLink
{
    // Fields.
    internal PhysicalEntityPart ParentPart { get; init; }

    internal PhysicalEntityPart LinkedPart { get; init; }

    internal PhysicalEntity Entity { get; init; }

    internal Vector2 LinkDistance
    {
        get => _linkDistance;
        set
        {
            _linkDistance = value;
            Entity.PartLinkDistanceChange();
        }
    }


    // Private fields.
    private Vector2 _linkDistance;


    // Constructors.
    public PartLink(PhysicalEntityPart mainPart, PhysicalEntityPart linkedPart, PhysicalEntity entity, Vector2 linkDistance)
    {
        ParentPart = mainPart ?? throw new ArgumentNullException(nameof(mainPart));
        LinkedPart = linkedPart ?? throw new ArgumentNullException(nameof(linkedPart));
        Entity = entity ?? throw new ArgumentNullException(nameof(entity));
        LinkDistance = linkDistance;
    }
}