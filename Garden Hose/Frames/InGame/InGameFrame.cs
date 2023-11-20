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

        GameWorldSettings StartupSettings = new()
        {
            Planet = WorldPlanet.TestPlanet,
            StartingEntities = new Entity[]
            {
                new TestEntity() { Position = new Vector2(0f, 700f) },
                //new TestEntity() { Position = new Vector2(0f, 800f) },
                //new TestEntity() { Position = new Vector2(0f, 900f) },
                //new TestEntity() { Position = new Vector2(0f, 1000f) },
                //new TestEntity() { Position = new Vector2(0f, 1100f) },
                //new TestEntity() { Position = new Vector2(0f, 1200f) },
                //new TestEntity() { Position = new Vector2(700f, 0f) },
                //new TestEntity() { Position = new Vector2(800f, 700f) },
                //new TestEntity() { Position = new Vector2(900f, 700f) },
                //new TestEntity() { Position = new Vector2(1000f, 700f) },
                //new TestEntity() { Position = new Vector2(1100f, 700f) },
                //new TestEntity() { Position = new Vector2(1200f, 700f) },
                //new TestEntity() { Position = new Vector2(700f, 700f) },
                //new TestEntity() { Position = new Vector2(700f, 800f) },
                //new TestEntity() { Position = new Vector2(700f, 900f) },
                //new TestEntity() { Position = new Vector2(700f, 1000f) },
                //new TestEntity() { Position = new Vector2(700f, 1100f) },
                //new TestEntity() { Position = new Vector2(700f, 1200f) },
                //new TestEntity() { Position = new Vector2(-700f, 700f) },
                //new TestEntity() { Position = new Vector2(-700f, 800f) },
                //new TestEntity() { Position = new Vector2(-700f, 900f) },
                //new TestEntity() { Position = new Vector2(-700f, 1000f) },
                //new TestEntity() { Position = new Vector2(-700f, 1100f) },
                //new TestEntity() { Position = new Vector2(-700f, 1200f) },
                //new TestEntity() { Position = new Vector2(-800f, 700f) },
                //new TestEntity() { Position = new Vector2(-800f, 800f) },
                //new TestEntity() { Position = new Vector2(-800f, 900f) },
                //new TestEntity() { Position = new Vector2(-800f, 1000f) },
                //new TestEntity() { Position = new Vector2(-800f, 1100f) },
                //new TestEntity() { Position = new Vector2(-800f, 1200f) },
                //new TestEntity() { Position = new Vector2(-900f, 700f) },
                //new TestEntity() { Position = new Vector2(-900f, 800f) },
                //new TestEntity() { Position = new Vector2(-900f, 900f) },
                //new TestEntity() { Position = new Vector2(-900f, 1000f) },
                //new TestEntity() { Position = new Vector2(-900f, 1100f) },
                //new TestEntity() { Position = new Vector2(-900f, 1200f) },
                //new TestEntity() { Position = new Vector2(-1000f, 700f) },
                //new TestEntity() { Position = new Vector2(-1000f, 800f) },
                //new TestEntity() { Position = new Vector2(-1000f, 900f) },
                //new TestEntity() { Position = new Vector2(-1000f, 1000f) },
                //new TestEntity() { Position = new Vector2(-1000f, 1100f) },
                //new TestEntity() { Position = new Vector2(-1000f, 1200f) },
                //new TestEntity() { Position = new Vector2(-1100f, 700f) },
                //new TestEntity() { Position = new Vector2(-1100f, 800f) },
                //new TestEntity() { Position = new Vector2(-1100f, 900f) },
                //new TestEntity() { Position = new Vector2(-1100f, 1000f) },
                //new TestEntity() { Position = new Vector2(-1100f, 1100f) },
                //new TestEntity() { Position = new Vector2(-11000f, 1200f) },
                //new TestEntity() { Position = new Vector2(-1200f, 700f) },
                //new TestEntity() { Position = new Vector2(-1200f, 800f) },
                //new TestEntity() { Position = new Vector2(-1200f, 900f) },
                //new TestEntity() { Position = new Vector2(-1200f, 1000f) },
                //new TestEntity() { Position = new Vector2(-1200f, 1100f) },
                //new TestEntity() { Position = new Vector2(-1200f, 1200f) }

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
