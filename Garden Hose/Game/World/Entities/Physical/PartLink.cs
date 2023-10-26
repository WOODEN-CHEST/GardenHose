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

    internal Vector2 LinkDistance { get; set; }


    // Constructors.
    public PartLink(PhysicalEntityPart mainPart, PhysicalEntityPart linkedPart, Vector2 linkDistance, bool isRotationLinked)
    {
        ParentPart = mainPart;
        LinkedPart = linkedPart;
        LinkDistance = linkDistance;
    }
}