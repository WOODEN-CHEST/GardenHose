using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHose.Frames.Global;


internal class GlobalFrame : GameFrame
{
    // Internal static fields.
    internal static SpriteFont GeEichFont;
    internal static SpriteFont GeEichFontLarge;


    // Constructors.
    public GlobalFrame(string? name) : base(name) { }


    // Inherited methods.
    public override void Load()
    {
        base.Load();

        GeEichFont = AssetManager.GetFont(this, "geeich");
        GeEichFontLarge = AssetManager.GetFont(this, "geeich_large");
    }
}