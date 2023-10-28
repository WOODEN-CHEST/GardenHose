using GardenHoseEngine.Frame;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;
using GardenHoseEngine.Frame.Animation;
using GardenHose.Game.World;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Planet;


internal class TestPlanet : WorldPlanet
{
    // Private fields.
    private SpriteItem _ballSprite;
    private Vector2 _ballSpriteScaling;


    // Constructors.
    internal TestPlanet(GameWorld? world) : base(world, 200f, 0f) { }

    internal TestPlanet() : this(null) { }


    // Inherited methods.
    public override void Draw()
    {
        _ballSprite.Position.Vector = World.ToViewportPosition(Position);
        _ballSprite.Scale.Vector = _ballSpriteScaling;
        _ballSprite.Scale.Vector.X *= World.Zoom;
        _ballSprite.Scale.Vector.Y *= World.Zoom;
        _ballSprite.Draw();
    }

    public override void Load(GHGameAssetManager assetManager)
    {
        _ballSprite = new SpriteItem(assetManager.TestPlanet);
        _ballSpriteScaling = new Vector2(Radius * 2, Radius * 2) / _ballSprite.TextureSize;
    }
}
