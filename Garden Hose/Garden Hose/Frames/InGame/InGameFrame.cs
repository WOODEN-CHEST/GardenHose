﻿using GardenHose.Game;
using GardenHose.Game.World;
using GardenHoseEngine.Frame;
using GardenHose.Game.World.Entities;
using Microsoft.Xna.Framework;
using System;
using GardenHose.Game.Background;
using GardenHose.Game.World.Entities.Probe;
using GardenHose.Game.World.Entities.Planet;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;

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

        GameWorldSettings StartupSettings = new()
        {
            Planet = Planet,
            PlayerShip = new ProbeEntity() { Position = new Vector2(0f, 550f), Rotation = MathF.PI },
            StartingEntities = new Entity[] { },
            Background = new(BackgroundImage.Default)
            {
                SmallStarCount = 200,
                MediumStarCount = 30,
                BigStarCount = 8
            },
            AmbientMaterial = GardenHose.Game.World.Material.WorldMaterial.Void
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
