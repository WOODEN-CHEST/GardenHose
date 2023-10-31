using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities;


internal partial class WorldPlanet : PhysicalEntity
{
    // Static fields.
    public static WorldPlanet TestPlanet => new WorldPlanet(512f, 120f,
        WorldMaterial.Test, PlanetSurfaceType.Gas1, PlanetAtmosphereType.None)
    {
        SurfaceColor = new(161, 155, 130),
        Overlays = new PlanetOverlay[]
        {
            new PlanetOverlay(PlanetOverlayType.Gas1Overlay1, new Color(199, 110, 102)),
            new PlanetOverlay(PlanetOverlayType.Gas1Overlay2, new Color(158, 145, 58))
        }
    };


    // Fields.
    internal float Radius { get; init; }

    internal float RadiusSquared { get; init; }

    internal float Attraction { get; set; }

    internal PlanetSurfaceType SurfaceType { get; private init; }

    internal PlanetAtmosphereType AtmosphereType { get; private init; }

    internal PlanetOverlay[]? Overlays { get; init; }

    internal Color SurfaceColor { get; set; } = Color.White;


    internal float AtmosphereThickness
    {
        get => _atmosphereThickness;
        set
        {
            _atmosphereThickness = Math.Max(0f, value);
            UpdateTextureScalings();
        }
    }

    internal Color AtmosphereColor { get; set; } = Color.White;

    internal float AtmosphereOpacity { get; set; } = 1f;


    // Private fields.
    private bool _isLoaded = false;
    private float _atmosphereThickness = 50f;

    private SpriteItem? _atmosphere;
    private SpriteItem _surface;
    private SpriteItem _overlay1;
    private SpriteItem _overlay2;
    private SpriteItem _overlay3;
    private SpriteItem _overlay4;
    private SpriteItem _overlay5;

    private Vector2 _surfaceScaling;
    private Vector2 _atmosphereScaling;


    // Constructors.
    public WorldPlanet(float radius,
        float attraction,
        WorldMaterial material,
        PlanetSurfaceType surfaceType,
        PlanetAtmosphereType atmosphereType,
        GameWorld? world = null) 
        : base(EntityType.Planet, world)
    {
        IsBoundingBoxDrawn = true;
        Radius = Math.Max(0, radius);
        RadiusSquared = Radius * Radius;
        Attraction = attraction;
        SurfaceType = surfaceType;
        AtmosphereType = atmosphereType;
        DrawLayer = DrawLayer.Bottom;

        MainPart = new PlanetPart(Radius, material, this);
    }


    // Internal methods.
    internal override void Delete()
    {
        throw new NotImplementedException();
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        _surface = SurfaceType switch
        {
            PlanetSurfaceType.Gas1 => new(assetManager.PlanetGas1Surface),

            _ => throw new EnumValueException(nameof(SurfaceType), nameof(PlanetSurfaceType),
                SurfaceType.ToString(), (int)SurfaceType)
        };

        if (Overlays != null)
        {
            foreach (PlanetOverlay Overlay in Overlays)
            {
                Overlay.Load(assetManager, Radius * 2f);
            }
        }

        _atmosphere = AtmosphereType switch
        {
            PlanetAtmosphereType.None => null,
            PlanetAtmosphereType.Default => new(assetManager.PlanetAtmosphereDefault),

            _ => throw new EnumValueException(nameof(AtmosphereType), nameof(PlanetAtmosphereType),
                AtmosphereType.ToString(), (int)AtmosphereType)
        };

        _isLoaded = true;
        UpdateTextureScalings();
    }


    // Private methods.
    private void UpdateTextureScalings()
    {
        if (!_isLoaded)
        {
            return;
        }

        float Diameter = Radius * 2f;

        _surfaceScaling = new Vector2(Diameter / _surface.TextureSize.X, Diameter / _surface.TextureSize.Y);

        if (AtmosphereType != PlanetAtmosphereType.None)
        {
            _atmosphereScaling = new Vector2((Diameter + AtmosphereThickness) / _atmosphere!.TextureSize.X,
                (Diameter + AtmosphereThickness) / _atmosphere.TextureSize.Y);
        }
    }

    private void DrawPlanet()
    {
        _surface.Position.Vector = World!.ToViewportPosition(Position);
        _surface.Scale.Vector = _surfaceScaling * World.Zoom;
        _surface.Mask = SurfaceColor;
        _surface.Draw();

        if (Overlays != null)
        {
            foreach (PlanetOverlay Overlay in Overlays)
            {
                Overlay.Draw(Position, World);
            }
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

        if (IsBoundingBoxDrawn)
        {
            DrawBoundingBox();
        }
    }

    internal override void ApplyForce(Vector2 force, Vector2 location) { }

    internal override void Tick() { }
}