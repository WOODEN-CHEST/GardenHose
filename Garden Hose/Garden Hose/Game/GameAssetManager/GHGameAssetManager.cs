using GardenHoseEngine;
using GardenHoseEngine.Audio;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using System;
using System.Collections.Generic;

namespace GardenHose.Game.GameAssetManager;


internal class GHGameAssetManager
{
    // Private fields.
    private IGameFrame _parentFrame;

    private Dictionary<GHGameAnimationName, GHGameAnimationEntry> _animations = new();
    private Dictionary<GHGameSoundName, GHGameSoundEntry> _sounds = new();


    // Constructors.
    internal GHGameAssetManager(IGameFrame parentFrame)
    {
        _parentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
        CreateAssetEntries();
    }


    // Internal methods.
    internal SpriteAnimation GetAnimation(GHGameAnimationName name)
    {
        return _animations[name].GetAnimation(_parentFrame);
    }

    internal Sound GetSound(GHGameSoundName name)
    {
        return _sounds[name].GetSound(_parentFrame);
    }


    // Private methods.
    private void CreateAssetEntries()
    {
        CreateAnimationEntries();
        CreateSoundEntries();
    }

    private void CreateAnimationEntries()
    {
        /* Planet. */
        _animations.Add(GHGameAnimationName.Planet_Atmosphere_Default, new(0f, Origin.Center, "game/planets/atmosphere/default"));

        _animations.Add(GHGameAnimationName.Planet_GasSurface_1, new(
            0f, Origin.Center, "game/planets/layers/gassurface1"));

        _animations.Add(GHGameAnimationName.Planet_GasOverlay_1, new(
            0f, Origin.Center, "game/planets/layers/gasoverlay1"));

        _animations.Add(GHGameAnimationName.Planet_GasOverlay_2, new(
            0f, Origin.Center, "game/planets/layers/gasoverlay2"));

        _animations.Add(GHGameAnimationName.Planet_RockSurface_1, new(
            0f, Origin.Center, "game/planets/layers/rocksurface1"));

        _animations.Add(GHGameAnimationName.Planet_WaterSurface_1, new(
            0f, Origin.Center, "game/planets/layers/watersurface"));

        _animations.Add(GHGameAnimationName.Planet_Clouds_1, new(
            0f, Origin.Center, "game/planets/layers/clouds1"));

        _animations.Add(GHGameAnimationName.Planet_Clouds_2, new(
            0f, Origin.Center, "game/planets/layers/clouds2"));

        _animations.Add(GHGameAnimationName.Planet_Clouds_3, new(
            0f, Origin.Center, "game/planets/layers/clouds3"));

        _animations.Add(GHGameAnimationName.Planet_Clouds_4, new(
            0f, Origin.Center, "game/planets/layers/clouds4"));

        _animations.Add(GHGameAnimationName.Planet_Clouds_5, new(
            0f, Origin.Center, "game/planets/layers/clouds5"));


        /* Particles. */
        _animations.Add(GHGameAnimationName.Particle_Test, new(
            4f, Origin.Center, "game/particles/test/0", "game/particles/test/1", "game/particles/test/2",
            "game/particles/test/3", "game/particles/test/4", "game/particles/test/5", "game/particles/test/6",
            "game/particles/test/7", "game/particles/test/8", "game/particles/test/9"));

        _animations.Add(GHGameAnimationName.Particle_Fuel1, new(0f, Origin.Center, "game/particles/fuel/1"));
        _animations.Add(GHGameAnimationName.Particle_Fuel2, new(0f, Origin.Center, "game/particles/fuel/2"));
        _animations.Add(GHGameAnimationName.Particle_Fuel3, new(0f, Origin.Center, "game/particles/fuel/3"));
        _animations.Add(GHGameAnimationName.Particle_Fuel4, new(0f, Origin.Center, "game/particles/fuel/4"));


        /* Background. */
        _animations.Add(GHGameAnimationName.Background_Default, new(
            0f, Origin.TopLeft, GameBackground.ASSETPATH_BACKGROUND_DEFAULT));

        _animations.Add(GHGameAnimationName.Background_Star_Small, new(
            0f, Origin.Center, GameBackground.ASSETPATH_STAR_SMALL));

        _animations.Add(GHGameAnimationName.Background_Star_Medium, new(
            0f, Origin.Center, GameBackground.ASSETPATH_STAR_MEDIUM));

        _animations.Add(GHGameAnimationName.Background_Star_Big, new(
            0f, Origin.Center, GameBackground.ASSETPATH_STAR_BIG));


        /* Ships. */
        /* Probe. */
        _animations.Add(GHGameAnimationName.Ship_Probe_Base, new(
            0f, Origin.Center, "game/ships/probe/base"));

        _animations.Add(GHGameAnimationName.Ship_Probe_Head, new(
            0f, Origin.Center, "game/ships/probe/head"));

        _animations.Add(GHGameAnimationName.Ship_Probe_MainThruster, new(
            0f, Origin.Center, "game/ships/probe/main_thruster"));

        _animations.Add(GHGameAnimationName.Ship_Probe_SideThruster, new(
            0f, Origin.Center, "game/ships/probe/side_thruster"));

        _animations.Add(GHGameAnimationName.Ship_Probe_RollPanel, new(
            0f, Origin.Center, "game/ships/probe/roll_panel"));

        _animations.Add(GHGameAnimationName.Ship_Probe_RollGlass, new(
           0f, Origin.Center, "game/ships/probe/roll_glass"));

        _animations.Add(GHGameAnimationName.Ship_Probe_RollDisplay, new(
           0f, Origin.Center, "game/ships/probe/roll_display"));

        _animations.Add(GHGameAnimationName.Ship_Probe_RollIndicator, new(
           0f, Origin.Center, "game/ships/probe/roll_indicator"));

        _animations.Add(GHGameAnimationName.Ship_Probe_RollIndicatorShadow, new(
           0f, Origin.TopRight, "game/ships/probe/roll_indicator_shadow"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ErrorPanel, new(
           0f, Origin.Center, "game/ships/probe/error_panel"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ErrorGlass, new(
           0f, Origin.Center, "game/ships/probe/error_panel_glass"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ErrorLT, new(
           0f, Origin.Center, "game/ships/probe/error_indicator_lt"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ErrorMT, new(
           0f, Origin.Center, "game/ships/probe/error_indicator_mt"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ErrorRT, new(
           0f, Origin.Center, "game/ships/probe/error_indicator_rt"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ErrorOxygen, new(
           0f, Origin.Center, "game/ships/probe/error_indicator_oxygen"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ErrorFuel, new(
           0f, Origin.Center, "game/ships/probe/error_indicator_fuel"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ErrorLeak, new(
          0f, Origin.Center, "game/ships/probe/error_indicator_leak"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ErrorSpin, new(
          0f, Origin.Center, "game/ships/probe/error_indicator_spin"));

        _animations.Add(GHGameAnimationName.Ship_Probe_Meter, new(
          0f, Origin.Center, "game/ships/probe/meter"));

        _animations.Add(GHGameAnimationName.Ship_Probe_MeterIndicator, new(
          0f, Origin.CenterRight, "game/ships/probe/meter_indicator"));

        _animations.Add(GHGameAnimationName.Ship_Probe_MeterMarkingA, new(
          0f, Origin.Center, "game/ships/probe/meter_marking_a"));

        _animations.Add(GHGameAnimationName.Ship_Probe_MeterMarkingS, new(
          0f, Origin.Center, "game/ships/probe/meter_marking_s"));

        _animations.Add(GHGameAnimationName.Ship_Probe_MeterDigitsA, new(
          0f, Origin.Center, "game/ships/probe/meter_digits_altitude"));

        _animations.Add(GHGameAnimationName.Ship_Probe_MeterDigitsS, new(
          0f, Origin.Center, "game/ships/probe/meter_digits_speed"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanel, new(
          0f, Origin.Center, "game/ships/probe/thruster_panel"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanelLightPanel, new(
          0f, Origin.Center, "game/ships/probe/thruster_panel_light_panel"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanelLightPanelShadow, new(
          0f, Origin.TopRight, "game/ships/probe/thruster_panel_light_panel_shadow"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanelLightDisplay, new(
          0f, Origin.Center, "game/ships/probe/thruster_panel_light_display"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanelLightGlass, new(
          0f, Origin.Center, "game/ships/probe/thruster_panel_light_glass"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanelSwitchPanel, new(
          0f, Origin.Center, "game/ships/probe/thruster_panel_switch_panel"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanelSwitchPanelShadow, new(
          0f, Origin.TopRight, "game/ships/probe/thruster_panel_switch_panel_shadow"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanelSwitch, new(
          0f, Origin.Center, "game/ships/probe/thruster_panel_switch_off", "game/ships/probe/thruster_panel_switch_on"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanelSwitchShadow, new(
          0f, Origin.TopRight, "game/ships/probe/thruster_panel_switch_off_shadow", "game/ships/probe/thruster_panel_switch_on_shadow"));

        _animations.Add(GHGameAnimationName.Ship_Probe_PowerPanel, new(
          0f, Origin.Center, "game/ships/probe/power_panel"));

        _animations.Add(GHGameAnimationName.Ship_Probe_PowerButton, new(
          0f, Origin.Center, "game/ships/probe/power_button"));

        _animations.Add(GHGameAnimationName.Ship_Probe_PowerButtonShadow, new(
          0f, Origin.TopRight, "game/ships/probe/power_button_shadow"));

        _animations.Add(GHGameAnimationName.Ship_Probe_AutopilotPanel, new(
         0f, Origin.Center, "game/ships/probe/autopilot_panel"));

        _animations.Add(GHGameAnimationName.Ship_Probe_AutopilotKnob, new(
         0f, Origin.Center, "game/ships/probe/autopilot_knob"));
    }

    private void CreateSoundEntries() { }
}