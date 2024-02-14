using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Planet.Buildings;

internal class ResidentalBuilding : PlanetBuilding
{
    // Internal fields.
    internal int Population { get; set; }
    internal ResidentalBuildingType BuildingType { get; private init; }


    // Private static fields.
    private static readonly Dictionary<ResidentalBuildingType, ResidentalBuildingDefinition> s_buildingDefinitions = new()
    {
        { ResidentalBuildingType.SmallPrivate, new ResidentalBuildingDefinition()
            {
                Population = 3,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.MediumPrivate, new ResidentalBuildingDefinition()
            {
                Population = 10,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.LargePrivate, new ResidentalBuildingDefinition()
            {
                Population = 30,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.SmallApartment, new ResidentalBuildingDefinition()
            {
                Population = 44,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.MediumApartment, new ResidentalBuildingDefinition()
            {
                Population = 74,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.LargeApartment, new ResidentalBuildingDefinition()
            {
                Population = 131,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.Skyscraper1, new ResidentalBuildingDefinition()
            {
                Population = 499,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.Skyscraper2, new ResidentalBuildingDefinition()
            {
                Population = 702,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.Skyscraper3, new ResidentalBuildingDefinition()
            {
                Population = 985,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.Skyscraper4, new ResidentalBuildingDefinition()
            {
                Population = 1411,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.Skyscraper5, new ResidentalBuildingDefinition()
            {
                Population = 2294,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        },
        { ResidentalBuildingType.Skyscraper6, new ResidentalBuildingDefinition()
            {
                Population = 5103,
                Size = new Vector2(),
                AnimationName = GHGameAnimationName.Particle_Test
            }
        }
    };


    // Constructors.
    public ResidentalBuilding(ResidentalBuildingType type, PhysicalEntity? entity)
        : base(new ICollisionBound[] {
            new RectangleCollisionBound(s_buildingDefinitions[type].Size, new Vector2(0f, s_buildingDefinitions[type].Size.Y * 0.4f))
        }, WorldMaterial.Test, entity)
    {

    }
}