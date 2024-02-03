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

        _animations.Add(GHGameAnimationName.Planet_GasSurface_1,  new(
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
            "game/particles/test/3","game/particles/test/4", "game/particles/test/5", "game/particles/test/6", 
            "game/particles/test/7", "game/particles/test/8", "game/particles/test/9"));


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

        _animations.Add(GHGameAnimationName.Ship_Probe_SideThruster,new (
            0f, Origin.Center, "game/ships/probe/side_thruster"));

        _animations.Add(GHGameAnimationName.Ship_Probe_RollPanel, new(
            0f, Origin.Center, "game/ships/probe/roll_panel"));

        _animations.Add(GHGameAnimationName.Ship_Probe_RollPanelIndicator, new(
           0f, Origin.Center, "game/ships/probe/roll_panel_indicator"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanel, new(
           0f, Origin.Center, "game/ships/probe/thruster_panel"));

        _animations.Add(GHGameAnimationName.Ship_Probe_ThrusterPanelButton, new(
           0f, Origin.Center, "game/ships/probe/thruster_panel_button_m",
           "game/ships/probe/thruster_panel_button_s", "game/ships/probe/thruster_panel_button_f"));
    }

    private void CreateSoundEntries() { }
}