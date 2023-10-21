using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHose.Game.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using GardenHose.Game.World.Entities;

namespace GardenHose.Game.World.Planet;

internal abstract class WorldPlanet : IDrawableItem
{
    // Internal static fields.


    // Fields.
    public bool IsVisible { get; set; } = true;

    public Effect? Shader { get; set; } = null;


    // Internal fields.
    internal GameWorld? World { get; set; }

    internal float Radius { get; private init; }

    internal float Attraction { get; private init; }

    internal Vector2 Position { get; private set; } = new Vector2(0f, 0f);

    internal BallCollisionBound CollisionBound { get; private set; }




    // Constructors.
    internal WorldPlanet(GameWorld? world, float radius, float attraction)
    {
        if (radius <= 0 || !float.IsFinite(radius))
        {
            throw new ArgumentException($"Invalid planet radius: {radius}", nameof(radius));
        }
        if (!float.IsFinite(attraction))
        {
            throw new ArgumentException($"Invalid planet attraction: {attraction}", nameof(attraction));
        }

        Radius = radius;
        Attraction = attraction;
        World = world;
        CollisionBound = new(Radius);
    }

    internal WorldPlanet(float radius, float attraction) : this(null, radius, attraction) { }


    // Inherited methods.
    public abstract void Draw();

    public abstract void Load(GHGameAssetManager assetManager);
}