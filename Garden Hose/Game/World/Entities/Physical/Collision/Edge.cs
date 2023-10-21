using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities;

internal struct Edge
{
    // Fields.
    internal Vector2 StartVertex { get; set; }

    internal Vector2 EndVertex { get; set; }

    internal Vector2 Length => EndVertex - StartVertex;

    internal Vector2 MiddleVertex => StartVertex + (Length * 0.5f);

    internal float Magnitude => Vector2.Distance(StartVertex, EndVertex);


    // Constructors.
    internal Edge(Vector2 startVertex, Vector2 endVertex)
    {
        StartVertex = startVertex;
        EndVertex = endVertex;
    }


    // Inherited methods.
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not Edge)
        {
            return false;
        }

        Edge ObjEdge = (Edge)obj;
        return (ObjEdge.StartVertex == StartVertex && ObjEdge.EndVertex == EndVertex);
    }
}