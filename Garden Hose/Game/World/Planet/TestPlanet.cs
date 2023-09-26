using GardenHoseEngine.Frame;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;
using GardenHoseEngine.Frame.Animation;
using GardenHoseServer.World;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Planet;


internal class TestPlanet : WorldPlanet
{
    // Private fields.
    private SpriteItem _ballSprite;


    // Constructors.
    internal TestPlanet(GameWorld world, IGameFrame ownerFrame, AssetManager assetManager, float radius, float attraction) 
        : base(world, ownerFrame, assetManager, radius, attraction)
    {
        _ballSprite = new SpriteItem(GH.Engine.Display,
            new SpriteAnimation(0f, ownerFrame, assetManager, Origin.Center, "test/ball"));
        _ballSprite.Scale.Vector = new Vector2(Radius, Radius) / _ballSprite.TextureSize;
    }


    // Inherited methods.
    public override void Draw(float passedTimeSeconds, SpriteBatch spriteBatch)
    {
        _ballSprite.Position.Vector = World.ObjectVisualOffset;
    }
}
