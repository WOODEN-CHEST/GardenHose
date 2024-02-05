using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item.Text;
using GardenHoseEngine.Frame.Item;
using System;
using GardenHoseEngine;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeDashboard
{
    // Private static fields.
    private static readonly Vector2 ROLL_PANEL_SIZE = new Vector2(150f, 150f);
    private static readonly Vector2 ROLL_PANEL_INDICATOR_SIZE = new Vector2(18f, 18f);
    private static readonly Vector2 ROLL_PANEL_INDICATOR_OFFSET = new Vector2(0f, -10f);
    private static readonly Vector2 ROLL_PANEL_TEXT_OFFSET = new Vector2(0f, 20f);


    // Private fields.
    private ProbeSystem _system;

    /* Roll. */
    private SpriteItem _rollPanel;
    private SpriteItem _rollPanelIndicator;
    private FittedText _rollPanelText;

    /* Altitude. */


    /* Speed. */



    // Constructors.
    internal ProbeDashboard(ProbeSystem system)
    {
        _system = system ?? throw new ArgumentNullException(nameof(system));

        _rollPanelText = new("", GH.GeeichFont);
        _rollPanelText.TextOrigin = Origin.Center;
        _rollPanelText.FittingSizePixels = new(100f, 50f);
    }



    // Methods.
    internal void Load(GHGameAssetManager assetManager)
    {
        _rollPanel = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_RollPanel).CreateInstance(), ROLL_PANEL_SIZE);
        _rollPanelIndicator = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_RollPanelIndicator).CreateInstance(), ROLL_PANEL_INDICATOR_SIZE);

        Vector2 RollPanelPosition = Display.VirtualSize - new Vector2(10f, 10f) - (_rollPanel.TextureSize * 0.5f);
        _rollPanel.Position = RollPanelPosition;
        _rollPanelIndicator.Position = RollPanelPosition + ROLL_PANEL_INDICATOR_OFFSET;
        _rollPanelText.Position = RollPanelPosition + ROLL_PANEL_TEXT_OFFSET;
    }

    internal void Draw(IDrawInfo info)
    {
        float Roll = _system.RollRelativeToGround;
        _rollPanel.Rotation = Roll;
        _rollPanel.Draw(info);
        _rollPanelIndicator.Draw(info);
        _rollPanelText.Text = MathHelper.ToDegrees(Roll).ToString("0") + " deg";
        _rollPanelText.Draw(info);
    }
}