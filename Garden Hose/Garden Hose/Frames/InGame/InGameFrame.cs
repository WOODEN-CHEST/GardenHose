using GardenHose.Game;
using GardenHose.Game.World;
using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework;
using System;
using GardenHose.Game.World.Entities.Probe;
using GardenHose.Game.World.Entities.Planet;
using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Material;
using System.Collections.Generic;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Planet.Buildings;
using GardenHose.Game.World.Entities.Test;

namespace GardenHose.Frames.InGame;


internal class InGameFrame : GameFrame
{
    // Internal fields.
    internal GHGame Game { get; private set; }


    // Constructors.
    internal InGameFrame(string? name) : base(name) { }


    // Inherited methods.
    public override void Load()
    {
        base.Load();

        WorldPlanetEntity Planet = WorldPlanetEntity.TestPlanet;
        Planet.TryPlaceBuilding(new BuildingPlaceholderEntity(new Starport())
        {
            Position = Planet.GetPositionAboveSurface(MathHelper.PiOver4, 0f),
            Rotation = MathHelper.PiOver4
        });

        //List<PhysicalEntity> Ents = new();
        //for (int i = 0; i < 30; i++)
        //{
        //    for (int j = 0; j < 30; j++)
        //    {
        //        Ents.Add(new TestEntity() { Position = new Vector2(i * 25, j * 25) });
        //    }
        //}

        GameWorldSettings StartupSettings = new()
        {
            Planet = Planet,
            PlayerShip = new ProbeEntity() { Position = new Vector2(800f, 0f), Rotation = MathF.PI * 0.5f },
            StartingEntities = Array.Empty<PhysicalEntity>(),
            Background = new(GHGameAnimationName.Background_Default)
            {
                SmallStarCount = 200,
                MediumStarCount = 30,
                BigStarCount = 8
            },
            AmbientMaterial = WorldMaterial.Void
        };
        Game = new(this, StartupSettings);
    }

    public override void OnStart()
    {
        base.OnStart();
        Game.OnStart();
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);
        Game.Update(time);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Game.OnEnd(); 
    }
}
