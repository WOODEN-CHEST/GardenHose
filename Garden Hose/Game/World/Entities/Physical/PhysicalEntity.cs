using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;
using GardenHoseEngine;


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

    internal virtual PhysicalEntityPart MainPart { get; set; }

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
            GetSubParts(MainPart);

            _cachedParts = EntityParts.ToArray();
            return _cachedParts;
        }
    }

    internal virtual DrawLayer DrawLayer { get; set; } = DrawLayer.Bottom;

    internal bool AreCollisionBoundsDrawn { get; set; } = false;

    internal bool IsMotionDrawn { get; set; } = false;

    internal bool IsCenterOfMassDrawnm { get; set; } = false;

    internal event EventHandler<Vector2> Collision;

    internal event EventHandler CollisionBoundChange;

    internal event EventHandler ParentChange;

    internal event EventHandler SubPartChange;


    // Private fields.
    private PhysicalEntityPart[]? _cachedParts = null;
    private float? _cachedMass = null;



    // Constructors.
    internal PhysicalEntity(EntityType type, GameWorld? world)
        : base(type, world) { }


    // Internal Methods.
    internal virtual void TestCollision()
    {
        //TestPlanetCollision();
    }

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


    /* Events from parts. */
    internal void OnPartCollision(PhysicalEntityPart part, Vector2 position)
    {

    }

    internal void OnPartCollisionBoundChange(PhysicalEntityPart part, ICollisionBound[]? bounds)
    {
        _cachedMass = null;
    }

    internal void OnPartParentChange(PhysicalEntityPart part, PartLink? parentLink)
    {
        _cachedParts = null;
        _cachedMass = null;
    }

    internal void OnPartSubPartChange(PhysicalEntityPart part, PartLink[]? subPartLinks)
    {
        _cachedParts = null;
        _cachedMass = null;
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


    /* Collision. */
    protected virtual void TestPlanetCollision()
    {
        
    }

    //protected void TestPlanetCollision()
    //{
    //    foreach (ICollisionBound Bound in CollisionBounds)
    //    {
    //        if (Bound.Type == CollisionBoundType.Rectangle)
    //        {
    //            TestColRectToBall((RectangleCollisionBound)Bound, World!.Planet.CollisionBound);
    //        }
    //    }
    //}

    //private void TestColRectToBall(RectangleCollisionBound rect, BallCollisionBound ball)
    //{
    //    Vector2[] Vertices = rect.GetVertices();
    //    List<Vector2> CollisionPoints = null!;

    //    // Find closest point so that a testable ray can be created.
    //    float ClosestDistance = float.PositiveInfinity;
    //    Vector2 ClosestVertex = Vector2.Zero;
    //    foreach (Vector2 Vertex in Vertices)
    //    {
    //        float Distance = Vector2.Distance(ball.Position, Vertex);
    //        if (Distance < ClosestDistance)
    //        {
    //            ClosestDistance = Distance;
    //            ClosestVertex = Vertex;
    //        }
    //    }
    //    Ray BallToRectRay = new(ball.Position, ClosestVertex);

    //    /* Find collision points. This is done by getting rays from the edge vertices,
    //     * then finding intersection points in said rays, then testing if the intersection 
    //     * point is inside of the edge's limits. If so, a collision has occurred.*/
    //    for (int EdgeIndex = 0; EdgeIndex < Vertices.Length; EdgeIndex++)
    //    {
    //        Edge Edge = new(Vertices[EdgeIndex], Vertices[(EdgeIndex + 1) % Vertices.Length]);
    //        Ray EdgeRay = new(Edge);

    //        Vector2 CollisionPoint = Ray.GetIntersection(EdgeRay, BallToRectRay);

    //        float MinX = Math.Min(Edge.StartVertex.X, Edge.EndVertex.X);
    //        float MaxX = Math.Max(Edge.StartVertex.X, Edge.EndVertex.X);
    //        float MinY = Math.Min(Edge.StartVertex.Y, Edge.EndVertex.Y);
    //        float MaxY = Math.Max(Edge.StartVertex.Y, Edge.EndVertex.Y);

    //        if (Vector2.Distance(ball.Position, CollisionPoint) <= ball.Radius
    //            && (MinX <= CollisionPoint.X) && (CollisionPoint.X <= MaxX)
    //            && (MinY <= CollisionPoint.Y) && (CollisionPoint.Y <= MaxY))
    //        {
    //            CollisionPoints ??= new();
    //            CollisionPoints.Add(CollisionPoint);
    //        }
    //    }

    //    if (CollisionPoints != null)
    //    {
    //        Vector2 SurfaceNormal = Vector2.Normalize(rect.Position - ball.Position);

    //        PushOutOfBall(CollisionPoints[0], ball);
    //        OnCollision(CollisionPoints[0], Vector2.Zero, SurfaceNormal);
    //    }
    //}

    //protected Vector2[] GetCollisionPointsRectToBall(Vector2[] rectVertices, RectangleCollisionBound rect, BallCollisionBound ball)
    //{
    //    List<Vector2> CollisionPoints = new(4);
    //    Ray BallRay = new(rect.Position, ball.Position);

    //    for (int vertexIndex = 0; vertexIndex < rectVertices.Length; vertexIndex++)
    //    {
    //        Edge RectEdge = new(rectVertices[vertexIndex], rectVertices[(vertexIndex + 1) % rectVertices.Length]);
    //        Vector2 Point = Ray.GetIntersection(BallRay, new Ray(RectEdge));

    //        float MinX = (Math.Min(RectEdge.StartVertex.X, RectEdge.EndVertex.X));
    //        float MaxX = (Math.Max(RectEdge.StartVertex.X, RectEdge.EndVertex.X));
    //        float MinY = (Math.Min(RectEdge.StartVertex.Y, RectEdge.EndVertex.Y));
    //        float MaxY = (Math.Max(RectEdge.StartVertex.Y, RectEdge.EndVertex.Y));

    //        if ((MinX <= Point.X && Point.X <= MaxX) && (MinY <= Point.Y && Point.Y <= MaxY)
    //            && Vector2.Distance(ball.Position, Point) <= ball.Radius)
    //        {
    //            CollisionPoints.Add(Point);
    //        }
    //    }

    //    return CollisionPoints.ToArray();
    //}

    //protected virtual void OnCollision(Vector2 collisionPoint, Vector2 surface, Vector2 surfaceNormal)
    //{
    //    CollisionEvent?.Invoke(this, collisionPoint);

    //    Vector2 CombinedMotionAtPoint = GetAngularMotionAtPoint(collisionPoint) + Motion;

    //    Motion = Vector2.Reflect(Motion, surfaceNormal) * 0.85f;
    //}

    ///* Other */
    //protected Vector2 GetAngularMotionAtPoint(Vector2 point)
    //{
    //    float SpeedAtPoint = Vector2.Distance(Position, point) * MathHelper.TwoPi * AngularMotion;
    //    float AngleOfPointToPosition = MathF.Atan2(point.Y - Position.Y, point.X - Position.X);

    //    Vector2 CalculatedAngularMotion = Vector2.Transform(Vector2.UnitY, Matrix.CreateRotationZ(AngleOfPointToPosition));
    //    CalculatedAngularMotion *= SpeedAtPoint; 
    //    return CalculatedAngularMotion;
    //}

    //protected void PushOutOfBall(Vector2 referencePoint, BallCollisionBound ball)
    //{
    //    Vector2 Unit = Vector2.Normalize(referencePoint - ball.Position);
    //    Vector2 NewPoint = Unit * ball.Radius;

    //    Vector2 Difference = new();
    //    Difference.X = NewPoint.X - referencePoint.X;
    //    Difference.Y = NewPoint.Y - referencePoint.Y;
    //    Position += Difference;
    //}


    // Private methods.
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
        MainPart.Draw(AreCollisionBoundsDrawn);

        if (IsMotionDrawn)
        {
            DrawMotion();
        }
        if (IsCenterOfMassDrawnm)
        {
            DrawCenterOfMass();
        }
    }
}