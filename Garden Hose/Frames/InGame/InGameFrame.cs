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

        WorldPlanet Planet = WorldPlanet.TestPlanet;

        //List<Entity> Entities = new();

        //for (int i = -10; i < 10; i++)
        //{
        //    for (int j = -10; j < 10; j++)
        //    {
        //        Vector2 Position = new(150f * i, 150f * j);
        //        if (Vector2.Distance(Planet.Position, Position) < Planet.Radius)
        //        {
        //            continue;
        //        }

        //        Entities.Add(new TestEntity() { Position = Position });
        //    }
        //}

        GameWorldSettings StartupSettings = new()
        {

            Planet = Planet,
            StartingEntities = new Entity[]
            {
                new TestEntity() { Position = new Vector2(0f, 700f) },

            },
            Background = new(BackgroundImage.Default)
            {
                SmallStarCount = 80,
                MediumStarCount = 15,
                BigStarCount = 2
            }
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

        UserInput.AddListener(MouseListenerCreator.SingleButton(this, true, MouseCondition.OnClick,
            (sender, args) => {
                Game.World.GetEntity<PhysicalEntity>(2uL)!.Position = 
                     (UserInput.VirtualMousePosition.Current - Game.World.ObjectVisualOffset) * (1f / Game.World.Zoom);
            },
            MouseButton.Left));


        UserInput.AddListener(KeyboardListenerCreator.SingleKey(this, KeyCondition.OnPress,
            (sender, args) =>
            {
                PhysicalEntity Entity = Game.World.GetEntity<PhysicalEntity>(2uL)!;
                Entity.Position = new(700f, 0f);
                Entity.Motion = Vector2.Zero;
                Entity.Rotation = 0f;
                Entity.AngularMotion = 0f;
                Game.World.CameraCenter = Vector2.Zero;
                Game.World.Zoom = 1f;
            },
            Keys.R));

        UserInput.AddListener(KeyboardListenerCreator.SingleKey(this, KeyCondition.WhileDown,
            (sender, args) =>
            {
                Game.World.CameraCenter = Game.World.GetEntity<PhysicalEntity>(2uL)!.Position;
            },
            Keys.Decimal));

        UserInput.AddListener(KeyboardListenerCreator.SingleKey(this, KeyCondition.WhileDown,
            (sender, args) => 
            {
                PhysicalEntity Entity = Game.World.GetEntity<PhysicalEntity>(2uL)!;

                Matrix RotationMatrix = Matrix.CreateRotationZ(Entity.Rotation);
                Vector2 ForceLocation = Vector2.Transform(new Vector2(0f, -20f), RotationMatrix) + Entity.Position;

                Entity.ApplyForce(Vector2.Transform(new(0f, -50_000f), RotationMatrix) * GameFrameManager.PassedTimeSeconds,
                    ForceLocation);
            }, Keys.Up));

        UserInput.AddListener(KeyboardListenerCreator.SingleKey(this, KeyCondition.WhileDown,
            (sender, args) =>
            {
                PhysicalEntity Entity = Game.World.GetEntity<PhysicalEntity>(2uL)!;

                Matrix RotationMatrix = Matrix.CreateRotationZ(Entity.Rotation);
                Vector2 ForceLocation = Vector2.Transform(new Vector2(0f, 20f), RotationMatrix) + Entity.Position;

                Entity.ApplyForce(Vector2.Transform(new(0f, 50_000f), RotationMatrix) * GameFrameManager.PassedTimeSeconds,
                    ForceLocation);
            }, Keys.Down));

        UserInput.AddListener(KeyboardListenerCreator.SingleKey(this, KeyCondition.WhileDown,
            (sender, args) =>
            {
                PhysicalEntity Entity = Game.World.GetEntity<PhysicalEntity>(2uL)!;

                Matrix RotationMatrix = Matrix.CreateRotationZ(Entity.Rotation);
                Vector2 ForceLocation = Vector2.Transform(new Vector2(-30f, 20f), RotationMatrix) + Entity.Position;

                Entity.ApplyForce(Vector2.Transform(new(0f, -50_000f), RotationMatrix) * GameFrameManager.PassedTimeSeconds,
                    ForceLocation);
            }, Keys.Left));

        UserInput.AddListener(KeyboardListenerCreator.SingleKey(this, KeyCondition.WhileDown,
            (sender, args) =>
            {
                PhysicalEntity Entity = Game.World.GetEntity<PhysicalEntity>(2uL)!;

                Matrix RotationMatrix = Matrix.CreateRotationZ(Entity.Rotation);
                Vector2 ForceLocation = Vector2.Transform(new Vector2(30f, 20f), RotationMatrix) + Entity.Position;

                Entity.ApplyForce(Vector2.Transform(new(0f, -50_000f), RotationMatrix) * GameFrameManager.PassedTimeSeconds,
                    ForceLocation);
            }, Keys.Right));

        UserInput.AddListener(KeyboardListenerCreator.SingleKey(this, KeyCondition.WhileDown, DoTheThing, Keys.B));
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

    private void DoTheThing(object? sender, EventArgs args)
    {

    }
}
