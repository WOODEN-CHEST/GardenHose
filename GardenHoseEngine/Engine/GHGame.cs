using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GardenHose;

public class GHGame : Game
{


    // Constructors.
    public GHGame()
    {
        Content.RootDirectory = "Content";
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