using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHose.UI.Buttons.Connector;


internal class ConnectorAssetCollection
{
    // Internal fields.
    internal AnimationInstance NormalPanel { get; private set; }
    internal AnimationInstance NormalGlow { get; private set; }
    internal AnimationInstance SquarePanel { get; private set; }
    internal AnimationInstance SquareGlow { get; private set; }
    internal AnimationInstance NarrowPanel { get; private set; }
    internal AnimationInstance NarrowGlow { get; private set; }
    internal AnimationInstance WidePanel { get; private set; }
    internal AnimationInstance WideGlow { get; private set; }

    internal AnimationInstance ToggleLight { get; private set; }

    internal AnimationInstance SliderPanel { get; private set; }
    internal AnimationInstance SliderGlow { get; private set; }
    internal AnimationInstance SliderPointer { get; private set; }

    internal AnimationInstance Connector { get; private set; }
    internal AnimationInstance Receiver { get; private set; }
    internal AnimationInstance ReceiverLights { get; private set; }

    internal SpriteFont TextFont { get; private set; }




    // Internal methods.
    internal void LoadAllAssets(IGameFrame? owner)
    {
        LoadFont(owner);
        LoadNormalButton(owner);
        LoadSquareButton(owner);
        LoadNarrowButton(owner);
        LoadWideButton(owner);
        LoadToggle(owner);
        LoadSlider(owner);
        LoadConnectorAndReceiver(owner);
    }

    internal void LoadFont(IGameFrame? owner)
    {
        TextFont = AssetManager.GetFont(owner, "geeich_large");
    }

    internal void LoadNormalButton(IGameFrame? owner)
    {
        NormalPanel = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/normal/panel").CreateInstance();
        NormalGlow = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/normal/glow").CreateInstance();
    }

    internal void LoadSquareButton(IGameFrame? owner)
    {
        SquarePanel = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/square/panel").CreateInstance();
        SquareGlow = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/square/glow").CreateInstance();
    }

    internal void LoadNarrowButton(IGameFrame? owner)
    {
        NarrowPanel = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/narrow/panel").CreateInstance();
        NarrowGlow = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/narrow/glow").CreateInstance();
    }

    internal void LoadWideButton(IGameFrame? owner)
    {
        WidePanel = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/wide/panel").CreateInstance();
        WideGlow = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/wide/glow").CreateInstance();
    }

    internal void LoadConnectorAndReceiver(IGameFrame? owner)
    {
        Connector = new SpriteAnimation(0f, owner, Origin.CenterLeft, "ui/buttons/connector/connector").CreateInstance();
        Receiver = new SpriteAnimation(0f, owner, Origin.CenterLeft, "ui/buttons/connector/receiver").CreateInstance();
        ReceiverLights = new SpriteAnimation(0f, owner, Origin.CenterLeft, "ui/buttons/connector/receiver_lights").CreateInstance();
    }

    internal void LoadToggle(IGameFrame? owner)
    {
        ToggleLight = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/toggle/light").CreateInstance();
    }

    internal void LoadSlider(IGameFrame? owner)
    {
        SliderPointer = new SpriteAnimation(0f, owner, Origin.TopMiddle, "ui/buttons/connector/slider/pointer").CreateInstance();
        SliderPanel = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/slider/panel").CreateInstance();
        SliderGlow = new SpriteAnimation(0f, owner, Origin.Center, "ui/buttons/connector/slider/glow").CreateInstance();
    }
}