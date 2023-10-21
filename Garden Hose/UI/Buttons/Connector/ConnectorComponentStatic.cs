using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using System;
using Microsoft.Xna.Framework;

namespace GardenHose.UI.Buttons.Connector;

internal abstract partial class ConnectorElement
{
    // Protected static fields.
    /* Color. */
    protected static readonly FloatColor UnselectedColor = new(0.4f, 0.4f, 0.4f, 1f);
    protected static readonly FloatColor SelectedColor = new(0.737f, 0.08f, 0.768f, 1f);
    protected static readonly FloatColor DefaultClickedColor = new(0.458f, 0.631f, 1f, 1f);


    /* Text shadow. */
    protected static readonly Color TextShadowColor = new(50, 50, 50, 255);
    protected static readonly float TextShadowOffset = 0.08f;


    /* Animations. */
    protected static SpriteAnimation? NormalPanelAnim;
    protected static SpriteAnimation? NormalGlowAnim;
    protected static SpriteAnimation? SquarePanelAnim;
    protected static SpriteAnimation? SquareGlowAnim;
    protected static SpriteAnimation? NarrowPanelAnim;
    protected static SpriteAnimation? NarrowGlowAnim;
    protected static SpriteAnimation? WidePanelAnim;
    protected static SpriteAnimation? WideGlowAnim;

    protected static SpriteAnimation? ToggleLightAnim;

    protected static SpriteAnimation? SliderPanelAnim;
    protected static SpriteAnimation? SliderGlowAnim;
    protected static SpriteAnimation? SliderPointerAnim;


    protected static SpriteAnimation? ConnectorAnim;
    protected static SpriteAnimation? ReceiverAnim;
    protected static SpriteAnimation? ReceiverLightsAnim;


    /* SpriteAnimation instances. */
    protected static AnimationInstance? NormalPanelInstance;
    protected static AnimationInstance? NormalGlowInstance;
    protected static AnimationInstance? SquarePanelInstance;
    protected static AnimationInstance? SquareGlowInstance;
    protected static AnimationInstance? NarrowPanelInstance;
    protected static AnimationInstance? NarrowGlowInstance;
    protected static AnimationInstance? WidePanelInstance;
    protected static AnimationInstance? WideGlowInstance;

    protected static AnimationInstance? ToggleLightInstance;

    protected static AnimationInstance? SliderPanelInstance;
    protected static AnimationInstance? SliderGlowInstance;
    protected static AnimationInstance? SliderPointerInstance;

    protected static AnimationInstance? ConnectorInstance;
    protected static AnimationInstance? ReceiverInstance;
    protected static AnimationInstance? ReceiverLightsInstance;


    // Static methods.
    internal static void LoadAllAssets(GameFrame owner)
    {
        LoadNormalButton(owner);
        LoadSquareButton(owner);
        LoadNarrowButton(owner);
        LoadWideButton(owner);
        LoadToggle(owner);
        LoadSlider(owner);
        LoadConnectorAndReceiver(owner);
    }

    internal static void LoadNormalButton(GameFrame owner)
    {
        NormalPanelAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/normal/panel");
        NormalGlowAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/normal/glow");

        NormalPanelInstance = NormalPanelAnim.CreateInstance();
        NormalGlowInstance = NormalGlowAnim.CreateInstance();
    }

    internal static void LoadSquareButton(GameFrame owner)
    {
        SquarePanelAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/square/panel");
        SquareGlowAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/square/glow");

        SquarePanelInstance = SquarePanelAnim.CreateInstance();
        SquareGlowInstance = SquareGlowAnim.CreateInstance();
    }

    internal static void LoadNarrowButton(GameFrame owner)
    {
        NarrowPanelAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/narrow/panel");
        NarrowGlowAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/narrow/glow");

        NarrowPanelInstance = NarrowPanelAnim.CreateInstance();
        NarrowGlowInstance = NarrowGlowAnim.CreateInstance();
    }

    internal static void LoadWideButton(GameFrame owner)
    {
        WidePanelAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/wide/panel");
        WideGlowAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/wide/glow");

        WidePanelInstance = WidePanelAnim.CreateInstance();
        WideGlowInstance = WideGlowAnim.CreateInstance();
    }

    internal static void LoadConnectorAndReceiver(GameFrame owner)
    {
        ConnectorAnim = new(0f, owner, Origin.CenterLeft, "ui/buttons/connector/connector");
        ReceiverAnim = new(0f, owner, Origin.CenterLeft, "ui/buttons/connector/receiver");
        ReceiverLightsAnim = new(0f, owner, Origin.CenterLeft, "ui/buttons/connector/receiver_lights");

        ConnectorInstance = ConnectorAnim.CreateInstance();
        ReceiverInstance = ReceiverAnim.CreateInstance();
        ReceiverLightsInstance = ReceiverLightsAnim.CreateInstance();
    }

    internal static void LoadToggle(GameFrame owner)
    {
        ToggleLightAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/toggle/light");
        ToggleLightInstance = ToggleLightAnim.CreateInstance();
    }

    internal static void LoadSlider(GameFrame owner)
    {
        SliderPointerAnim = new(0f, owner, Origin.TopMiddle, "ui/buttons/connector/slider/pointer");
        SliderPanelAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/slider/panel");
        SliderGlowAnim = new(0f, owner, Origin.Center, "ui/buttons/connector/slider/glow");

        SliderPointerInstance = SliderPointerAnim.CreateInstance();
        SliderPanelInstance = SliderPanelAnim.CreateInstance();
        SliderGlowInstance = SliderGlowAnim.CreateInstance();
    }

    internal static float GetConnectorAngle(Direction direction)
    {
        return direction switch
        {
            Direction.Right => 0f,
            Direction.Left => MathF.PI,
            Direction.Up => -(MathF.PI / 2),
            Direction.Down => MathF.PI / 2f,
            _ => throw new EnumValueException(nameof(direction), nameof(Direction),
                    direction.ToString(), (int)direction)
        };
    }

    internal static Vector2 GetDirectionVector(Direction direction)
    {
        return direction switch
        {
            Direction.Left => new(-1f, 0f),
            Direction.Right => new(1f, 0f),
            Direction.Up => new(0f, -1f),
            Direction.Down => new(0f, 1f),
            _ => throw new EnumValueException(nameof(direction), nameof(Direction),
            direction, (int)direction),
        };
    }
}
