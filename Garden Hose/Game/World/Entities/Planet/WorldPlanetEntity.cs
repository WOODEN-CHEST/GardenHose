using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GardenHose.Game.World.Entities.Planet;


internal partial class WorldPlanetEntity : PhysicalEntity
{
    // Static fields.
    /* Preset planets. */
    public static WorldPlanetEntity TestPlanet => new WorldPlanetEntity(512f,
        WorldMaterial.PlanetTest,
        new PlanetTexture[]
        {
            new PlanetTexture(PlanetTextureType.WaterSurface1, new Color(22, 28, 102), 1f),
            new PlanetTexture(PlanetTextureType.RockSurface1, new Color(110, 140, 83), 1f),
            new PlanetTexture(PlanetTextureType.Clouds4, Color.White, 0.8f),
            new PlanetTexture(PlanetTextureType.DefaultAtmosphere, new Color(161, 155, 130), 0.6f, 1.08f),
        },
        null);

    // Fields.
    internal float Radius { get; init; }

    internal float AtmosphereThickness { get; init; } = 20f;

    internal override float Mass => 5.972e24f;


    // Constructors.
    public WorldPlanetEntity(float radius,
        WorldMaterial material,
        PlanetTexture[] textures,
        GameWorld? world = null) 
        : base(EntityType.Planet, world)
    {
        Radius = Math.Max(0f, radius);
        DrawLayer = DrawLayer.Bottom;

        MainPart = new PhysicalEntityPart(new ICollisionBound[] { new BallCollisionBound(Radius) }, material, this);

        IsCollisionReactionEnabled = false;
        IsInvulnerable = true;
        IsAttractable = false;

        if (textures == null)
        {
            throw new ArgumentNullException(nameof(textures));
        }

        foreach (PlanetTexture Texture in textures)
        {
            MainPart.AddSprite(Texture);
        }
    }


    // Internal methods.
    internal override void Load(GHGameAssetManager assetManager)
    {
        base.Load(assetManager);

        foreach (PhysicalEntityPartSprite Sprite in MainPart.PartSprites)
        {
            Sprite.Scale = new Vector2(Radius) / Sprite.Sprite.TextureSize * Sprite.Scale;
        }
    }

    // Inherited methods.
    internal override void SequentialTick()
    {
        base.SequentialTick();
    }

    internal override void ParallelTick()
    {

    }
}