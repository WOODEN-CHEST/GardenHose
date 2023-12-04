using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Events;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities.Planet;


internal partial class WorldPlanetEntity : PhysicalEntity
{
    // Static fields.
    /* Preset planets. */
    public static WorldPlanetEntity TestPlanet => new WorldPlanetEntity(512f, 120f,
        WorldMaterial.PlanetTest, PlanetAtmosphereType.None)
    {
        AtmosphereColor = Color.CornflowerBlue,
        AtmosphereOpacity = 0.6f,
        AtmosphereThickness = 50f,
        AtmosphereType = PlanetAtmosphereType.Default,
        SurfaceColor = new(161, 155, 130),
        Textures = new PlanetTexture[]
        {
            new PlanetTexture(PlanetTextureType.WaterSurface1, new Color(22, 28, 102)),
            new PlanetTexture(PlanetTextureType.RockSurface1, new Color(110, 140, 83)),
            new PlanetTexture(PlanetTextureType.Clouds4, Color.White),
        }
    };


    // Fields.
    internal float Radius { get; init; }

    internal float RadiusSquared { get; init; }

    internal float Attraction { get; set; }

    internal PlanetAtmosphereType AtmosphereType { get; private init; }

    internal PlanetTexture[] Textures { get; init; }

    internal Color SurfaceColor { get; set; } = Color.White;


    internal float AtmosphereThickness
    {
        get => _atmosphereThickness;
        set
        {
            _atmosphereThickness = Math.Max(0f, value);
            UpdateAtmosphereScaling();
        }
    }

    internal Color AtmosphereColor { get; set; } = Color.White;

    internal float AtmosphereOpacity { get; set; } = 1f;

    internal override float Mass => 5.972e24f;


    // Private fields.
    private bool _isLoaded = false;
    private float _atmosphereThickness = 5f;

    private SpriteItem? _atmosphere;
    private Vector2 _atmosphereScaling;


    // Constructors.
    public WorldPlanetEntity(float radius,
        float attraction,
        WorldMaterial material,
        PlanetAtmosphereType atmosphereType,
        GameWorld? world = null) 
        : base(EntityType.Planet, world)
    {
        Radius = Math.Max(0, radius);
        RadiusSquared = Radius * Radius;
        Attraction = attraction;
        AtmosphereType = atmosphereType;
        DrawLayer = DrawLayer.Bottom;

        MainPart = new WorldPlanetPart(Radius, material, this);

        IsCollisionReactionEnabled = false;
        IsInvulnerable = true;
        IsAttractable = false;
    }


    // Internal methods.
    internal override void Load(GHGameAssetManager assetManager)
    {
        base.Load(assetManager);

        foreach (PlanetTexture Overlay in Textures)
        {
            Overlay.Load(assetManager, Radius * 2f);
        }

        _atmosphere = AtmosphereType switch
        {
            PlanetAtmosphereType.None => null,
            PlanetAtmosphereType.Default => new(assetManager.GetAnimation("planet_atmosphere_default")!),

            _ => throw new EnumValueException(nameof(AtmosphereType), nameof(PlanetAtmosphereType),
                AtmosphereType.ToString(), (int)AtmosphereType)
        };

        _isLoaded = true;
        UpdateAtmosphereScaling();
    }

    // Private methods.
    private void UpdateAtmosphereScaling()
    {
        if (!_isLoaded || AtmosphereType == PlanetAtmosphereType.None)
        {
            return;
        }

        float Diameter = Radius * 2f;
        _atmosphereScaling = new Vector2((Diameter + AtmosphereThickness) / _atmosphere!.TextureSize.X,
            (Diameter + AtmosphereThickness) / _atmosphere.TextureSize.Y);
    }

    private void DrawPlanet()
    {
        foreach (PlanetTexture Texture in Textures)
        {
            Texture.Draw(Position, World!);
        }

        if (AtmosphereType == PlanetAtmosphereType.None)
        {
            return;
        }

        _atmosphere!.Position.Vector = World!.ToViewportPosition(Position);
        _atmosphere.Scale.Vector = _atmosphereScaling * World.Zoom;
        _atmosphere.Mask = AtmosphereColor;
        _atmosphere.Opacity = AtmosphereOpacity;
        _atmosphere.Draw();
    }


    // Inherited methods.
    public override void Draw()
    {
        if (IsVisible)
        {
            DrawPlanet();
        }

        base.Draw();
    }

    internal override void SequentialTick()
    {
        base.SequentialTick();
    }

    internal override void ParallelTick()
    {

    }

    internal override void OnPartDamage(PartDamageEventArgs args) { }

    internal override void OnPartDestroy(PartDamageEventArgs args) { }
}