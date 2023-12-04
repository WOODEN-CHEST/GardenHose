using GardenHoseEngine;
using GardenHoseEngine.Audio;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GardenHose.Game.AssetManager;


internal class GHGameAssetManager
{
    // Internal fields.

    // Private fields.
    private IGameFrame _parentFrame;

    private Dictionary<string, (SpriteAnimation? Animation, Func<SpriteAnimation> LoadFunction)> _animations = new();
    private Dictionary<string, (Sound? Sound, Func<Sound> LoadFunction)> _sounds = new();


    // Constructors.
    internal GHGameAssetManager(IGameFrame parentFrame)
    {
        _parentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
        CreateAssetEntries();
    }


    // Internal methods.
    internal SpriteAnimation? GetAnimation(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (!_animations.TryGetValue(name, out var Value))
        {
            return null;
        }

        if (Value.Animation == null)
        {
            Value.Animation = Value.LoadFunction.Invoke();
            _animations[name] = Value;
        }

        return Value.Animation;
    }

    internal Sound? GetSound(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (!_sounds.TryGetValue(name, out var Value))
        {
            return null;
        }

        if (Value.Sound == null)
        {
            Value.Sound = Value.LoadFunction.Invoke();
            _sounds[name] = Value;
        }

        return Value.Sound;
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
        _animations.Add("planet_atmosphere_default", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/atmosphere/default")));

        _animations.Add("planet_layer_gassurface1", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/layers/gassurface1")));

        _animations.Add("planet_layer_gasoverlay1", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/layers/gasoverlay1")));

        _animations.Add("planet_layer_gasoverlay2", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/layers/gasoverlay2")));

        _animations.Add("planet_layer_rocksurface1", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/layers/rocksurface1")));

        _animations.Add("planet_layer_watersurface1", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/layers/watersurface")));

        _animations.Add("planet_layer_clouds1", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/layers/clouds1")));

        _animations.Add("planet_layer_clouds2", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/layers/clouds2")));

        _animations.Add("planet_layer_clouds3", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/layers/clouds3")));

        _animations.Add("planet_layer_clouds4", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/layers/clouds4")));

        _animations.Add("planet_layer_clouds5", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/planets/layers/clouds5")));  


        /* Particles. */
        _animations.Add("particle_test", (null, () => new SpriteAnimation(
            4f, _parentFrame, Origin.Center, "game/particles/test/0", "game/particles/test/1", "game/particles/test/2", 
            "game/particles/test/3","game/particles/test/4", "game/particles/test/5", "game/particles/test/6", 
            "game/particles/test/7", "game/particles/test/8", "game/particles/test/9")));


        /* Background. */
        _animations.Add("background_default", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.TopLeft, "game/backgrounds/default")));

        _animations.Add("background_star_small", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/stars/small")));

        _animations.Add("background_star_medium", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/stars/medium")));

        _animations.Add("background_star_big", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/stars/big")));


        /* Ships. */
        /* Probe. */
        _animations.Add("ship_probe_base", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/ships/probe/base")));

        _animations.Add("ship_probe_head", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/ships/probe/head")));

        _animations.Add("ship_probe_mainthruster", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/ships/probe/main_thruster")));

        _animations.Add("ship_probe_sidethruster", (null, () => new SpriteAnimation(
            0f, _parentFrame, Origin.Center, "game/ships/probe/side_thruster")));
    }

    private void CreateSoundEntries()
    {

    }
}