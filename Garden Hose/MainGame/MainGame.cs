using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GardenHose;

public class MainGame : Game
{
    // Static fields.
    public static MainGame Instance { get; private set; }
    public static GraphicsDeviceManager GraphicsManager { get; private set; }

    private Model thing;

    // Constructors.
    public MainGame()
    {
        GraphicsManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Instance = this;

        IsMouseVisible = true;
        GraphicsManager.PreferredBackBufferWidth = 1280;
        GraphicsManager.PreferredBackBufferHeight = 720;
    }


    // Inherited methods.
    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {

    }

    protected override void Draw(GameTime gameTime)
    {

    }


    // Private methods.
    private void LoadPersistentContent()
    {

    }
}