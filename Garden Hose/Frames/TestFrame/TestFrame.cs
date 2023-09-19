using GardenHoseEngine;
using GardenHoseEngine.Audio;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using GardenHoseEngine.Frame.Item.Text;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Frames;

public class TestFrame : GameFrame
{
    // Fields.
    public Sound AmongSound;

    SpriteAnimation BackgroundAnim;
    SpriteItem Background;

    DynamicFont DefaultFont;
    TypeableSimpleTextBox Box;

    new ILayer TopLayer;


    // Constructors.
    public TestFrame(string? name) : base(name)
    {
        
    }


    // Inherited methods.
    public override void Update(TimeSpan passedTime)
    {
        base.Update(passedTime);
    }

    public override void Load(AssetManager assetManager)
    {
        base.Load(assetManager);
        AmongSound = GH.Engine.AssetManager.GetSoundEffect(this, "amongus yell");

        BackgroundAnim = new(0d, this, GH.Engine.AssetManager, Origin.TopLeft, "back");
        DefaultFont = new(this, GH.Engine.AssetManager, "default");
    }

    public override void OnStart()
    {
        base.OnStart();

        TopLayer = new Layer(GH.Engine.GraphicsManager, "lays chips");
        AddLayer(TopLayer);

        Background = new(this, GH.Engine.Display, TopLayer, BackgroundAnim);
        Background.Position.Vector = Vector2.Zero;

        Box = new(this, GH.Engine.Display, TopLayer, GH.Engine.SinglePixel, 
            GH.Engine.UserInput, AmongSound, DefaultFont, "Hello, World!  AAAAAAAAAAAAAAAAAAAAAAAAAAAA\naaaa\naaaa\ndfnaj");

        Box.Mask = Color.Red;
        Box.Position.Vector = GH.Engine.Display.VirtualSize * 0.5f;
        Box.SetOrigin(Origin.Middle);
        Box.IsTypeable = true;

        GH.Engine.UserInput.AddListener(MouseListenerCreator.Scroll(GH.Engine.UserInput, this, this, true,
            ScrollDirection.Up, 
            (sender, args) => DefaultFont.Scale = 
            (DynamicFontScale)Math.Min((int)DefaultFont.Scale + 1, (int)DynamicFontScale.Huge)));

        GH.Engine.UserInput.AddListener(MouseListenerCreator.Scroll(GH.Engine.UserInput, this, this, true,
            ScrollDirection.Down,
            (sender, args) => DefaultFont.Scale =
            (DynamicFontScale)Math.Max((int)DefaultFont.Scale - 1, (int)DynamicFontScale.Tiny)));
    }
}