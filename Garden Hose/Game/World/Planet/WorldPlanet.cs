using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHoseServer.World;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GardenHose.Game.World.Planet;

internal abstract class WorldPlanet : IDrawableItem
{
    // Internal static fields.


    // Fields.
    public bool IsVisible { get; set; } = true;

    public Effect? Shader { get; set; } = null;


    // Internal fields.
    internal GameWorld World { get; set; }

    internal float Radius { get; private init; }

    internal float Attraction { get; private init; }




    // Constructors.
    internal WorldPlanet(GameWorld world, IGameFrame ownerFrame, AssetManager assetManager, float radius, float attraction)
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
        World = world ?? throw new ArgumentNullException(nameof(world));
    }


    // Inherited methods.
    public abstract void Draw(float passedTimeSeconds, SpriteBatch spriteBatch);
}