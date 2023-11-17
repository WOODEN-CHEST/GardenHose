using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities;


internal partial class WorldPlanet : PhysicalEntity
{
    // Static fields.
    public static WorldPlanet TestPlanet => new WorldPlanet(512f, 120f,
        WorldMaterial.Test, PlanetAtmosphereType.None)
    {
        AtmosphereColor = Color.CornflowerBlue,
        AtmosphereOpacity = 0.6f,
        AtmosphereThickness = 50f,
        AtmosphereType = PlanetAtmosphereType.Default,
        SurfaceColor = new(161, 155, 130),
        Textures = new PlanetTexture[]
        {
            new PlanetTexture(PlanetTextureType.Water, new Color(22, 28, 102)),
            new PlanetTexture(PlanetTextureType.Rock1Land, new Color(110, 140, 83)),
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
    private SpriteItem _surface;
    private SpriteItem _overlay1;
    private SpriteItem _overlay2;
    private SpriteItem _overlay3;
    private SpriteItem _overlay4;
    private SpriteItem _overlay5;

    private Vector2 _atmosphereScaling;


    // Constructors.
    public WorldPlanet(float radius,
        float attraction,
        WorldMaterial material,
        PlanetAtmosphereType atmosphereType,
        GameWorld? world = null) 
        : base(EntityType.Planet, world)
    {
        IsBoundingBoxDrawn = true;
        Radius = Math.Max(0, radius);
        RadiusSquared = Radius * Radius;
        Attraction = attraction;
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
        foreach (PlanetTexture Overlay in Textures)
        {
            Overlay.Load(assetManager, Radius * 2f);
        }

        _atmosphere = AtmosphereType switch
        {
            PlanetAtmosphereType.None => null,
            PlanetAtmosphereType.Default => new(assetManager.PlanetAtmosphereDefault),

            _ => throw new EnumValueException(nameof(AtmosphereType), nameof(PlanetAtmosphereType),
                AtmosphereType.ToString(), (int)AtmosphereType)
        };

        _isLoaded = true;
        UpdateAtmosphereScaling();
    }

    // Protected methods.
    protected override void PushOutOfOtherEntity(CollisionCase collisionCase) // Overrides to push other entity rather than self.
    {
        // Prepare variables.
        Vector2 PushOutDirection = -GetPushOutDirection(collisionCase);
        const int StepCount = 8;
        float StepDistance = 10f;

        // First step.
        collisionCase.EntityB.Position += StepDistance * PushOutDirection;

        // Consequent steps.
        Vector2 ClosestPushOutPosition = collisionCase.EntityB.Position;
        bool IsColliding = false;

        for (int Step = 0; Step < StepCount; Step++)
        {
            StepDistance *= 0.5f;
            collisionCase.EntityB.Position += IsColliding ? (PushOutDirection * StepDistance) : (-PushOutDirection * StepDistance);

            var CollisionData = collisionCase.PartA.TestBoundAgainstBound(
                collisionCase.BoundA, collisionCase.BoundB, collisionCase.PartB);
            IsColliding = CollisionData != null;

            if (!IsColliding)
            {
                ClosestPushOutPosition = collisionCase.EntityB.Position;
            }
        }

        // Update position.
        collisionCase.EntityB.Position = ClosestPushOutPosition;
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

        if (IsBoundingBoxDrawn)
        {
            DrawBoundingBox();
        }
    }

    internal override void ApplyForce(Vector2 force, Vector2 location) { }

    internal override void Tick() { }
}