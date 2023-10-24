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
    internal PhysicalEntityPart MainPart { get; init; }

    internal PhysicalEntityPart LinkedPart { get; init; }

    internal Vector2 LinkDistance { get; init; }

    internal bool IsRotationLinked { get; set; } = true;



    // Constructors.
    public PartLink(PhysicalEntityPart mainPart, PhysicalEntityPart linkedPart, Vector2 linkDistance, bool isRotationLinked)
    {
        MainPart = mainPart ?? throw new ArgumentNullException(nameof(mainPart));
        LinkedPart = linkedPart ?? throw new ArgumentNullException(nameof(linkedPart));
        LinkDistance = linkDistance;
        IsRotationLinked = isRotationLinked;
    }
}