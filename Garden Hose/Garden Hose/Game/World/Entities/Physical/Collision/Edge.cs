using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Physical.Collision;

internal struct Edge
{
    // Fields.
    internal Vector2 StartVertex { get; set; }
    internal Vector2 EndVertex { get; set; }
    internal Vector2 Step => EndVertex - StartVertex;
    internal Vector2 MiddleVertex => StartVertex + (Step * 0.5f);
    internal float Length => Vector2.Distance(StartVertex, EndVertex);


    // Constructors.
    internal Edge(Vector2 startVertex, Vector2 endVertex)
    {
        StartVertex = startVertex;
        EndVertex = endVertex;
    }


    // Internal methods.
    internal bool IsPointInEdgeArea(Vector2 point)
    {
        float MinX = Math.Min(StartVertex.X, EndVertex.X);
        float MaxX = Math.Max(StartVertex.X, EndVertex.X);
        float MinY = Math.Min(StartVertex.Y, EndVertex.Y);
        float MaxY = Math.Max(StartVertex.Y, EndVertex.Y);

        return (MinX <= point.X && point.X <= MaxX)
            && (MinY <= point.Y && point.Y <= MaxY);
    }


    // Inherited methods.
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Edge)
        {
            Edge ObjEdge = (Edge)obj;
            return (ObjEdge.StartVertex == StartVertex) && (ObjEdge.EndVertex == EndVertex);
        }
        return false;
        
    }

    public override int GetHashCode()
    {
        return StartVertex.GetHashCode() + EndVertex.GetHashCode();
    }


    // Operators.
    public static bool operator ==(Edge edge1, Edge edge2)
    {
        return (edge1.StartVertex == edge2.StartVertex)
            && (edge1.EndVertex == edge2.EndVertex);
    }

    public static bool operator !=(Edge edge1, Edge edge2)
    {
        return (edge1.StartVertex != edge2.StartVertex)
            || (edge1.EndVertex != edge2.EndVertex);
    }
}