using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities;


internal class WorldPlanet : PhysicalEntity
{
    // Static fields.
    public static WorldPlanet TestPlanet => new WorldPlanet(1024f, 120f,
        WorldMaterial.Test, PlanetSurfaceType.Gas1, PlanetAtmosphereType.Default)
    {
        Color1 = new(186, 139, 102),
        AtmosphereColor = new Color(209, 173, 65),
        AtmosphereOpacity = 0.6f,
        AtmosphereThickness = 50f
    };


    // Fields.
    internal float Radius { get; init; }

    internal float RadiusSquared { get; init; }

    internal float Attraction { get; set; }

    internal PlanetSurfaceType SurfaceType { get; private init; }

    internal PlanetAtmosphereType AtmosphereType { get; private init; }

    internal Color Color1 { get; set; } = Color.White;

    internal Color Color2 { get; set; } = Color.White;

    internal Color Color3 { get; set; } = Color.White;

    internal Color Color4 { get; set; } = Color.White;

    internal Color Color5 { get; set; } = Color.White;

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
    private float _atmosphereThickness = 50f;

    private SpriteItem? _atmosphere;
    private SpriteItem _surface;

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
            PlanetSurfaceType.Gas1 => new(assetManager.PlanetGas1Texture),

            _ => throw new EnumValueException(nameof(SurfaceType), nameof(PlanetSurfaceType),
                SurfaceType.ToString(), (int)SurfaceType)
        };

        _atmosphere = AtmosphereType switch
        {
            PlanetAtmosphereType.None => null,
            PlanetAtmosphereType.Default => new(assetManager.PlanetAtmosphereDefaultTexture),

            _ => throw new EnumValueException(nameof(AtmosphereType), nameof(PlanetAtmosphereType),
                AtmosphereType.ToString(), (int)AtmosphereType)
        };

        UpdateTextureScalings();
    }


    // Private methods.
    private void UpdateTextureScalings()
    {
        if (_surface != null)
        {
            _surfaceScaling = new Vector2(Radius / _surface.TextureSize.X, Radius / _surface.TextureSize.Y);
        }

        if (_atmosphere != null && AtmosphereType != PlanetAtmosphereType.None)
        {
            _atmosphereScaling = new Vector2((Radius + AtmosphereThickness) / _atmosphere!.TextureSize.X,
                (Radius + AtmosphereThickness) / _atmosphere.TextureSize.Y);
        }
    }


    // Inherited methods.
    public override void Draw()
    {
        if (!IsVisible) return;
        
        _surface.Position.Vector = World!.ToViewportPosition(Position);
        _surface.Scale.Vector = _surfaceScaling * World.Zoom;
        _surface.Mask = Color1;
        _surface.Draw();


        if (AtmosphereType == PlanetAtmosphereType.None) return;

        _atmosphere!.Position.Vector = World!.ToViewportPosition(Position);
        _atmosphere.Scale.Vector = _atmosphereScaling * World.Zoom;
        _atmosphere.Mask = AtmosphereColor;
        _atmosphere.Opacity = AtmosphereOpacity;
        _atmosphere.Draw();
    }

    internal override void ApplyForce(Vector2 force, Vector2 location) { }

    internal override void Tick() { }
}