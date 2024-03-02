using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Ship;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities.Probe;


internal class ProbeThrusterIndicator : ProbeSystemComponent
{
    // Internal static fields.
    internal static Vector2 SWITCH_SIZE { get; } = new(20f, 40f);
    internal static Vector2 SWITCH_SHADOW_SIZE { get; } = new(23f, 41f);
    internal static Vector2 SWITCH_PANEL_SIZE{ get; } = new(31f, 50f);
    internal static Vector2 SWITCH_PANEL_SHADOW_SIZE{ get; } = new(38f, 59f);
    internal static Vector2 LIGHT_PANEL_SIZE{ get; } = new(36f, 68f);
    internal static Vector2 LIGHT_PANEL_SHADOW_SIZE{ get; } = new(37f, 72f);
    internal static Vector2 LIGHT_PANEL_DISPLAY_SIZE{ get; } = new(26f, 58f);
    internal static Vector2 LIGHT_PANEL_GLASS_SIZE{ get; } = new(30f, 62f);
    internal static FloatColor LIGHT_OFF_COLOR { get; } = new(0.4f, 0.4f, 0.4f, 1f);
    internal static FloatColor LIGHT_ON_COLOR { get; } = new(0.7f, 0.1f, 0.1f, 1f);
    internal static Color SWITCH_OFF_COLOR { get; } = new(0.9f, 0.9f, 0.9f, 1f);
    internal static Color SWITCH_ON_COLOR { get; } = new(0.4f, 1f, 0.4f, 1f);



    // Internal fields.
    internal const int INDICATOR_LIGHT_COUNT = 4;
    internal const float MIN_VALUE = 0f;
    internal const float MAX_VALUE = 1f;

    internal float Value
    {
        get => _value;
        set => _value = Math.Clamp(value, MIN_VALUE, MAX_VALUE);
    }

    internal bool SwitchState
    {
        get => _switchState;
        set
        {
            if (value == _switchState)
            {
                return;
            }

            _switchState = value;
            if (_switch != null)
            {
                OnSwitchStateChange();
            }
        }
    }

    internal override Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;

            _switch.Position = value;
            _switchPanel.Position = value;
            _switchPanelShadow.Position = value + (SWITCH_PANEL_SIZE * new Vector2(0.5f, -0.5f));
            _switchShadow.Position = value + (SWITCH_SIZE * new Vector2(0.5f, -0.5f));

            Vector2 FirstLightPadding = new Vector2((SWITCH_PANEL_SIZE.X * 0.5f) + (LIGHT_PANEL_SIZE.X * 0.5f) + 3f, 0f);
            Vector2 SubsequentLightPadding = new Vector2(LIGHT_PANEL_SIZE.X + 3f, 0f);
            for (int i = 0; i < INDICATOR_LIGHT_COUNT; i++)
            {
                _lightDisplays[i].Position = value + FirstLightPadding + (SubsequentLightPadding * i);
                _lightPanelGlasses[i].Position = value + FirstLightPadding + (SubsequentLightPadding * i);
                _lightPanels[i].Position = value + FirstLightPadding + (SubsequentLightPadding * i);
                _lightPanelsShdaows[i].Position = value + FirstLightPadding + (SubsequentLightPadding * i)
                    + (LIGHT_PANEL_SIZE * new Vector2(0.5f, -0.5f));
            }
        }
    }


    // Private fields.
    private const int SWITCH_OFF_ANIM_INDEX = 0;
    private const int SWITCH_ON_ANIM_INDEX = 1;

    private readonly EventHandler<bool> _switchSetHandler;
    private bool _switchState;

    private float _value = 0f;

    private SpriteItem _switch;
    private SpriteItem _switchShadow;
    private SpriteButton _switchPanel;
    private SpriteItem _switchPanelShadow;

    private SpriteItem[] _lightDisplays;
    private SpriteItem[] _lightPanels;
    private SpriteItem[] _lightPanelsShdaows;
    private SpriteItem[] _lightPanelGlasses;


    // Constructors.
    internal ProbeThrusterIndicator(EventHandler<bool> switchSetHandler, bool initialState)
    {
        _switchSetHandler = switchSetHandler ?? throw new ArgumentNullException(nameof(switchSetHandler));
        SwitchState = initialState;
    }



    // Internal methods.
    internal override void Load(GHGameAssetManager assetManager)
    {
        _lightDisplays = new SpriteItem[INDICATOR_LIGHT_COUNT];
        _lightPanels = new SpriteItem[INDICATOR_LIGHT_COUNT];
        _lightPanelsShdaows = new SpriteItem[INDICATOR_LIGHT_COUNT];
        _lightPanelGlasses = new SpriteItem[INDICATOR_LIGHT_COUNT];

        for (int i = 0; i < INDICATOR_LIGHT_COUNT; i++)
        {
            _lightDisplays[i] = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelLightDisplay).CreateInstance(),
                LIGHT_PANEL_DISPLAY_SIZE);

            _lightPanels[i] = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelLightPanel).CreateInstance(),
                LIGHT_PANEL_SIZE);

            _lightPanelsShdaows[i] = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelLightPanelShadow).CreateInstance(),
                            LIGHT_PANEL_SHADOW_SIZE);

            _lightPanelGlasses[i] = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelLightGlass).CreateInstance(),
                            LIGHT_PANEL_GLASS_SIZE);
        }

        _switch = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelSwitch).CreateInstance(),
            SWITCH_SIZE);

        _switchShadow = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelSwitchShadow).CreateInstance(),
            SWITCH_SHADOW_SIZE);

        _switchPanel = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelSwitchPanel).CreateInstance(),
            SWITCH_PANEL_SIZE);

        _switchPanel.SetHandler(ButtonEvent.RightClick, (sender, args) =>
        {
            SwitchState = !SwitchState;
            _switchSetHandler.Invoke(this, SwitchState);
        });

        _switchPanelShadow = new(assetManager.GetAnimation(GHGameAnimationName.Ship_Probe_ThrusterPanelSwitchPanelShadow).CreateInstance(),
            SWITCH_PANEL_SHADOW_SIZE);

        OnSwitchStateChange();
    }

    internal override void Draw(IDrawInfo info)
    {
        _switchPanelShadow.Draw(info);
        _switchPanel.Draw(info);
        _switchShadow.Draw(info);
        _switch.Draw(info);

        for (int i = 0; i < INDICATOR_LIGHT_COUNT; i++)
        {
            _lightPanelsShdaows[i].Draw(info);

            _lightDisplays[i].Mask = FloatColor.InterpolateRGB(LIGHT_OFF_COLOR, LIGHT_ON_COLOR, 
                Math.Clamp((Value * INDICATOR_LIGHT_COUNT) - i, 0f, 1f));
            _lightDisplays[i].Draw(info);

            _lightPanels[i].Draw(info);
            _lightPanelGlasses[i].Draw(info);
        }
    }

    internal override void Tick(GHGameTime time, ProbeEntity probe, bool isComponentPowered)
    {
        if (probe.Pilot == SpaceshipPilot.Player)
        {
            _switchPanel.Update(time.WorldTime);
        }

        _switch.Mask = (SwitchState && isComponentPowered) ? SWITCH_ON_COLOR : SWITCH_OFF_COLOR;
    }


    // Private methods.
    private void OnSwitchStateChange()
    {
        _switch.ActiveAnimation.FrameIndex = SwitchState ? SWITCH_ON_ANIM_INDEX : SWITCH_OFF_ANIM_INDEX;
        _switchShadow.ActiveAnimation.FrameIndex = SwitchState ? SWITCH_ON_ANIM_INDEX : SWITCH_OFF_ANIM_INDEX;
    }
}