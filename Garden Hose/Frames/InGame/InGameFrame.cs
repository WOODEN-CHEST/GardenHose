using GardenHose.Game;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public override void Load(AssetManager assetManager)
    {
        base.Load(assetManager);

        Game = new(this);
        Game.Load(assetManager);
    }

    public override void OnStart()
    {
        base.OnStart();
        Game.OnStart();

        GH.Engine.UserInput.AddListener(MouseListenerCreator.SingleButton(GH.Engine.UserInput, this, this, true, MouseCondition.OnClick,
            (sender, args) => {
                _startPosition = GH.Engine.UserInput.VirtualMousePosition.Current;
                _currentCameraCenter = Game.World.CameraCenter;
            },
            MouseButton.Right));

        GH.Engine.UserInput.AddListener(MouseListenerCreator.SingleButton(GH.Engine.UserInput, this, this, true, MouseCondition.WhileDown,
            (sender, args) =>
            {
                Game.World.CameraCenter = _currentCameraCenter 
                - (GH.Engine.UserInput.VirtualMousePosition.Current -_startPosition);
            }, MouseButton.Right));

        GH.Engine.UserInput.AddListener(MouseListenerCreator.Scroll(GH.Engine.UserInput, this, this, true, ScrollDirection.Up,
            (sender, args) => Game.World.Zoom *= 1.2f));

        GH.Engine.UserInput.AddListener(MouseListenerCreator.Scroll(GH.Engine.UserInput, this, this, true, ScrollDirection.Down,
            (sender, args) => Game.World.Zoom /= 1.2f));
    }

    public override void Update(float passedTimeSeconds)
    {
        base.Update(passedTimeSeconds);
        Game.Update(passedTimeSeconds);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Game.OnEnd(); 
    }

    public override void Draw(float passedTimeSeconds, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D layerPixelBuffer, RenderTarget2D framePixelBuffer)
    {
        base.Draw(passedTimeSeconds, graphicsDevice, spriteBatch, layerPixelBuffer, framePixelBuffer);
    }
}
