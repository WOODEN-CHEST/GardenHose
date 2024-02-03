using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Planet;


internal partial class WorldPlanetEntity : PhysicalEntity
{
    // Static fields.
    /* Preset planets. */
    public static WorldPlanetEntity TestPlanet => new WorldPlanetEntity(500f,
        WorldMaterial.PlanetTest,
        new PartSprite[]
        {
            new PartSprite(GHGameAnimationName.Planet_WaterSurface_1) { ColorMask = new Color(22, 28, 102) },
            new PartSprite(GHGameAnimationName.Planet_RockSurface_1) { ColorMask = new Color(110, 140, 83) },
            new PartSprite(GHGameAnimationName.Planet_Clouds_4) { ColorMask = Color.White, Opacity = 0.8f },
        },
        new PartSprite(GHGameAnimationName.Planet_Clouds_4) { ColorMask = new Color(161, 155, 130), Opacity = 0.6f });


    // Fields.
    internal float Radius
    {
        get => _radius;
        set
        {
            _radius = Math.Max(0f, value);
            ApplyAtmosphereSize();
            ApplySpriteSizes();
        }
    }

    internal float AtmosphereThickness
    {
        get => _atmosphereThickness;
        set
        {
            _atmosphereThickness = value;
            ApplyAtmosphereSize();
        }
    }
    internal override float Mass => 5.972e24f;


    // Private fields.
    private PartSprite? _atmosphere;
    private float _atmosphereThickness;
    private float _radius;


    // Constructors.
    public WorldPlanetEntity(float radius,
        WorldMaterial material,
        PartSprite[] sprites,
        PartSprite? atmosphere) 
        : base(EntityType.Planet)
    {
        Radius = radius;
        DrawLayer = DrawLayer.Bottom;
        MainPart = new PhysicalEntityPart(new ICollisionBound[] { new BallCollisionBound(Radius) }, material, this);

        if (sprites == null)
        {
            throw new ArgumentNullException(nameof(sprites));
        }

        foreach (PartSprite Sprite in sprites)
        {
            Sprite.Size = new(Radius * 2f);
            base.MainPart.AddSprite(Sprite);
        }

        _atmosphere = atmosphere;
        AtmosphereThickness = 20f;

        CollisionHandler.IsCollisionReactionEnabled = false;
        IsInvulnerable = true;
        IsAttractable = false;
    }


    // Private methods.
    private void ApplyAtmosphereSize()
    {
        if (_atmosphere != null)
        {
            _atmosphere.Size = new(Radius * 2f + _atmosphereThickness);
        }
    }

    private void ApplySpriteSizes()
    {
        if (MainPart == null)
        {
            return;
        }

        foreach (PartSprite Sprite in MainPart.Sprites)
        {
            Sprite.Size = new(Radius * 2f);
        }
    }
}