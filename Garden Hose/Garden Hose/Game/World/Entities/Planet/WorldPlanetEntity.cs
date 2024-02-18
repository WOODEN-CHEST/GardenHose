using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Entities.Planet.Buildings;
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
    public static WorldPlanetEntity TestPlanet => new WorldPlanetEntity(500f,
        WorldMaterial.PlanetTest,
        new PartSprite[]
        {
            new PartSprite(GHGameAnimationName.Planet_WaterSurface_1) { ColorMask = new Color(22, 28, 102) },
            new PartSprite(GHGameAnimationName.Planet_RockSurface_1) { ColorMask = new Color(110, 140, 83) },
            new PartSprite(GHGameAnimationName.Planet_Clouds_4) { ColorMask = Color.White, Opacity = 0.8f },
        },
        new PartSprite(GHGameAnimationName.Planet_Atmosphere_Default) { ColorMask = new Color(161, 155, 130), Opacity = 0.6f });


    internal const float DEFAULT_RESOURCES_PER_PLANET = 1000f;

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





    /* Buildings. */
    internal float TotalResources { get; }
    internal float TotalPlanetTokens { get; }
    internal float ResourcesPerPlanetToken { get; set; } = DEFAULT_RESOURCES_PER_PLANET;
    internal float TotalShipTokens { get; }
    internal float SavedResources { get; }
    internal float PartOfResourcesToSave { get; set; } = 0.1f;
    internal float PartOfResourcesForPlanet { get; set; } = 0.5f;
    internal float PartOfResourcesForShip => 1.0f - PartOfResourcesForPlanet;
    internal float Population { get; }


    // Private fields.
    private PartSprite? _atmosphere;
    private float _atmosphereThickness;
    private float _radius;


    // Constructors.
    internal WorldPlanetEntity(float radius,
        WorldMaterial material,
        PartSprite[] sprites,
        PartSprite? atmosphere) 
        : base(EntityType.Planet)
    {
        CollisionHandler = new PlanetCollisionHandler(this);

        Radius = radius;
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
        if (_atmosphere != null)
        {
            MainPart.AddSprite(_atmosphere);
        }
        AtmosphereThickness = 20f;

        IsAttractable = false;
        IsForceApplicable = false;
        IsPositionLocked = true;
        IsRotationLocked = true;

        ZIndex = ZINDEX_PLANET;
    }


    // Internal methods.
    internal float GetPositionAsRotation(Vector2 position)
    {
        Vector2 RelativePosition = position - Position;
        return MathF.Atan2(RelativePosition.X, RelativePosition.Y);
    }

    internal Vector2 GetPositionAboveSurface(Vector2 position, float height)
    {
        return GHMath.NormalizeOrDefault(position - Position) * (Radius + height);
    }

    internal Vector2 GetPositionAboveSurface(float rotation, float height)
    {
        return Vector2.TransformNormal(new Vector2(0f, -1f), Matrix.CreateRotationZ(rotation)) * (Radius + height);
    }

    internal bool IsBuildingPlaceable(BuildingPlaceholderEntity buildingEntity)
    {
        List<CollisionCase> Cases = new();
        foreach (PartLink Link in MainPart.SubPartLinks)
        {
            PhysicalEntityPart Part = Link.LinkedPart;
            foreach (ICollisionBound Bound in Part.CollisionBounds)
            {
                CollisionHandler.TestBoundAgainstPart(Cases, Part, Bound, buildingEntity.MainPart, buildingEntity);
                if (Cases.Count > 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    internal bool TryPlaceBuilding(BuildingPlaceholderEntity buildingEntity)
    {
        if (!IsBuildingPlaceable(buildingEntity))
        {
            return false;
        }

        PlanetBuilding Building = (PlanetBuilding)buildingEntity.MainPart;
        MainPart.LinkPart(Building, buildingEntity.Position - Position);
        Building.SelfRotation = buildingEntity.Rotation;

        return true;
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

    internal override Entity CreateClone()
    {
        throw new NotImplementedException();
    }
}