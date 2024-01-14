using GardenHose.Game.AssetManager;
using GardenHoseEngine.Frame.Item.Text;
using GardenHoseEngine.Frame.Item;
using System;
using GardenHose.Frames.Global;
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
    private SimpleTextBox _rollPanelText;

    /* Altitude. */


    /* Speed. */



    // Constructors.
    internal ProbeDashboard(ProbeSystem system)
    {
        _system = system ?? throw new ArgumentNullException(nameof(system));

        _rollPanelText = new(GlobalFrame.GeEichFont, "");
        _rollPanelText.Origin = Origin.Center;
        _rollPanelText.Scale = new(0.6f);
    }



    // Methods.
    internal void Load(GHGameAssetManager assetManager)
    {
        _rollPanel = new(assetManager.GetAnimation("ship_probe_rollpanel")!);
        _rollPanelIndicator = new(assetManager.GetAnimation("ship_probe_rollpanelindicator")!);

        _rollPanel.TargetTextureSize = ROLL_PANEL_SIZE;
        _rollPanelIndicator.TargetTextureSize = ROLL_PANEL_INDICATOR_SIZE;

        Vector2 RollPanelPosition = Display.VirtualSize - new Vector2(10f, 10f) - (_rollPanel.TextureSize * 0.5f);
        _rollPanel.Position.Vector = RollPanelPosition;
        _rollPanelIndicator.Position.Vector = RollPanelPosition + ROLL_PANEL_INDICATOR_OFFSET;
        _rollPanelText.Position.Vector = RollPanelPosition + ROLL_PANEL_TEXT_OFFSET;
    }

    internal void Draw()
    {
        float Roll = _system.RollRelativeToGround;
        _rollPanel.Rotation = Roll;
        _rollPanel.Draw();
        _rollPanelIndicator.Draw();
        _rollPanelText.Text = MathHelper.ToDegrees(Roll).ToString("0") + " deg";
        _rollPanelText.Draw();
    }
}