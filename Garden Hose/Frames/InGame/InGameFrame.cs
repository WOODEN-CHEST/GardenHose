using GardenHose.Game;
using GardenHose.Game.World;
using GardenHoseEngine.Frame;
using GardenHoseEngine.IO;
using GardenHose.Game.World.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using GardenHose.Game.Background;
using System.Collections.Generic;
using GardenHose.Game.World.Entities.Probe;
using GardenHose.Game.World.Entities.Planet;
using GardenHose.Game.World.Entities.Physical;

namespace GardenHose.Frames.InGame;


internal class InGameFrame : GameFrame
{
    // Internal fields.
    internal GHGame Game { get; private set; }


    // Private fields.
    private Vector2 _startPosition;
    private Vector2 _currentCameraCenter;


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
                SmallStarCount = 80,
                MediumStarCount = 15,
                BigStarCount = 2
            },
            AmbientMaterial = GardenHose.Game.World.Material.WorldMaterial.Void
        };
        Game = new(this, StartupSettings);
    }

    public override void OnStart()
    {
        base.OnStart();
        Game.OnStart();

        UserInput.AddListener(MouseListenerCreator.SingleButton(this, true, MouseCondition.OnClick,
            (sender, args) => {
                _startPosition = UserInput.VirtualMousePosition.Current;
                _currentCameraCenter = Game.World.CameraCenter;
            },
            MouseButton.Right));

        UserInput.AddListener(MouseListenerCreator.SingleButton(this, true, MouseCondition.WhileDown,
            (sender, args) =>
            {
                Game.World.CameraCenter = _currentCameraCenter
                - (UserInput.VirtualMousePosition.Current - _startPosition) * (1f / Game.World.Zoom);
            }, MouseButton.Right));

        UserInput.AddListener(MouseListenerCreator.Scroll(this, true, ScrollDirection.Up,
            (sender, args) => Game.World.Zoom *= 1.2f));

        UserInput.AddListener(MouseListenerCreator.Scroll(this, true, ScrollDirection.Down,
            (sender, args) => Game.World.Zoom /= 1.2f));
    }

    public override void Update()
    {
        base.Update();
        Game.Update();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Game.OnEnd(); 
    }

    public override void Draw()
    {
        base.Draw();
    }
}
