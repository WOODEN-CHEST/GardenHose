using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GardenHoseEngine.Frame;

namespace GardenHose.UI.Buttons.Connector;

internal partial class ConnectorButton
{
    // Private static fields.
    /* Animations. */
    private static SpriteAnimation? s_normalPanelAnim;
    private static SpriteAnimation? s_normalGlowAnim;

    private static SpriteAnimation? s_connectorAnim;
    private static SpriteAnimation? s_receiverAnim;
    private static SpriteAnimation? s_receiverLightsAnim;

    /* SpriteAnimation instances. */
    private static AnimationInstance? s_normalPanelInstance;
    private static AnimationInstance? s_normalGlowInstance;

    private static AnimationInstance? s_connectorInstance;
    private static AnimationInstance? s_receiverInstance;
    private static AnimationInstance? s_receiverLightsInstance;

    /* Color. */
    private static readonly FloatColor s_unselectedColor = new(0.4f, 0.4f, 0.4f, 1f);
    private static readonly FloatColor s_selectedColor = new(0.737f, 0.08f, 0.768f, 1f);
    private static readonly FloatColor s_defaultClickedColor = new(0.458f, 0.631f, 1f, 1f);


    // Static methods.
    internal static void LoadAllAssets(AssetManager assetManager, GameFrame owner)
    {
        LoadNormalButton(assetManager, owner);
        LoadConnectorAndReceiver(assetManager, owner);
    }

    internal static void LoadNormalButton(AssetManager assetManager, GameFrame owner)
    {
        s_normalPanelAnim = new(0f, owner, assetManager, Origin.Center, "ui/buttons/connector/normal/panel");
        s_normalGlowAnim = new(0f, owner, assetManager, Origin.Center, "ui/buttons/connector/normal/glow");

        s_normalPanelInstance = s_normalPanelAnim.CreateInstance();
        s_normalGlowInstance = s_normalGlowAnim.CreateInstance();
    }

    internal static void LoadConnectorAndReceiver(AssetManager assetManager, GameFrame owner)
    {
        s_connectorAnim = new(0f, owner, assetManager, Origin.CenterLeft, "ui/buttons/connector/connector");
        s_receiverAnim = new(0f, owner, assetManager, Origin.CenterLeft, "ui/buttons/connector/receiver");
        s_receiverLightsAnim = new(0f, owner, assetManager, Origin.CenterLeft, "ui/buttons/connector/receiver_lights");

        s_connectorInstance = s_connectorAnim.CreateInstance();
        s_receiverInstance = s_receiverAnim.CreateInstance();
        s_receiverLightsInstance = s_receiverLightsAnim.CreateInstance();
    }
}
