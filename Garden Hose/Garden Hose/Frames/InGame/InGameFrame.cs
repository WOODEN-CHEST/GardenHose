using GardenHose.Game;
using GardenHose.Game.World;
using GardenHoseEngine.Frame;
using GardenHose.Game.World.Entities;
using Microsoft.Xna.Framework;
using System;
using GardenHose.Game.World.Entities.Probe;
using GardenHose.Game.World.Entities.Planet;
using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Material;
using GardenHose.Game.World.Entities.Test;
using System.Collections.Generic;
using GardenHose.Game.World.Entities.Physical;

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

        List<PhysicalEntity> Ents = new();
        //for (int i = 0; i < 20; i++)
        //{
        //    for (int j = 0; j < 20; j++)
        //    {
        //        Ents.Add(new TestEntity() { Position = new Vector2(i * 20, j * 20) });
        //    }
        //}

        GameWorldSettings StartupSettings = new()
        {
            Planet = Planet,
            PlayerShip = new ProbeEntity() { Position = new Vector2(-100f, 550f), Rotation = MathF.PI },
            StartingEntities = Ents.ToArray(),
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
