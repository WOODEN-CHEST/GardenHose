using GardenHose.Game;
using GardenHose.Game.World;
using GardenHose.Game.World.Planet;
using GardenHoseEngine.Frame;
using GardenHoseEngine.IO;
using GardenHose.Game.World.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


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
            Planet = new TestPlanet(),
            StartingEntities = new Entity[]
            {
                new TestEntity() { Position = new Vector2(250f, 0f), Motion = new Vector2(0f, 100f), AngularMotion = 1f }
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
                - (UserInput.VirtualMousePosition.Current - _startPosition);
            }, MouseButton.Right));

        UserInput.AddListener(MouseListenerCreator.Scroll(this, true, ScrollDirection.Up,
            (sender, args) => Game.World.Zoom *= 1.2f));

        UserInput.AddListener(MouseListenerCreator.Scroll(this, true, ScrollDirection.Down,
            (sender, args) => Game.World.Zoom /= 1.2f));

        UserInput.AddListener(KeyboardListenerCreator.SingleKey(this, KeyCondition.OnPress,
            (sender, args) =>
            {
                PhysicalEntity Entity = Game.World.GetEntity<PhysicalEntity>(1ul)!;
                Entity.Position = new Vector2(200, 200f);
                Entity.Motion = new Vector2(0f, 0f);
                Entity.Rotation = 0f;
            }, Keys.R));

        UserInput.AddListener(MouseListenerCreator.SingleButton(this, true, MouseCondition.OnClick,
            (sender, args) => {
                Game.World.GetEntity<PhysicalEntity>(1)!.Position = 
                     Game.World.ObjectVisualOffset - UserInput.VirtualMousePosition.Current;
            },
            MouseButton.Left));
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
