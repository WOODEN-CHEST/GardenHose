using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;

namespace GardenHose;

public class MainGame : Game
{
    // Static fields.
    public static MainGame Instance { get; private set; }
    public static GraphicsDeviceManager GraphicsManager { get; private set; }
    SpriteBatch Batch;
    Texture2D Ball;
    Color Mask = Color.White;
    public (double R, double G, double B) RealMask = (1d, 1d, 1d);
    Effect Effect;


    // Constructors.
    public MainGame()
    {
        GraphicsManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Instance = this;
        IsFixedTimeStep = false;
        GraphicsManager.SynchronizeWithVerticalRetrace = false;
        Window.AllowUserResizing = true;

        IsMouseVisible = true;
        GraphicsManager.PreferredBackBufferWidth = 1280;
        GraphicsManager.PreferredBackBufferHeight = 720;
    }


    // Inherited methods.
    protected override void LoadContent()
    {
        Ball = Content.Load<Texture2D>(@"ball");
        Effect = Content.Load<Effect>(@"glow");

        Batch = new(GraphicsDevice);
    }


    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {

            RealMask.R = 1d;
            RealMask.G = 0.1d;
            RealMask.B = 0.1d;
        }

        RealMask.R = Math.Min(1d, RealMask.R + gameTime.ElapsedGameTime.TotalSeconds * 4d);
        RealMask.G = Math.Min(1d, RealMask.G + gameTime.ElapsedGameTime.TotalSeconds * 4d);
        RealMask.B = Math.Min(1d, RealMask.B + gameTime.ElapsedGameTime.TotalSeconds * 4d);

        Mask.R = (byte)(byte.MaxValue * RealMask.R);
        Mask.G = (byte)(byte.MaxValue * RealMask.G);
        Mask.B = (byte)(byte.MaxValue * RealMask.B);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Brown);
        Vector4 MaskColor = Vector4.Zero;

        Batch.Begin(effect: Effect, blendState: BlendState.AlphaBlend);
        Vector2 Pos = Vector2.Zero;
        for (int i = 0; i < 1000; i++)
        {
            Effect.Parameters["Color"].SetValue(MaskColor);


            Batch.Draw(Ball, Pos, Mask);
            Pos.X += 1f;

            MaskColor.X = (MaskColor.X + 0.01f) % 1f;
            MaskColor.Y = (MaskColor.Y + 0.01f) % 1f;
            MaskColor.Z = (MaskColor.Z + 0.01f) % 1f;
            MaskColor.W = (MaskColor.W + 0.01f) % 1f;

            
        }
        Batch.End();
    }
}