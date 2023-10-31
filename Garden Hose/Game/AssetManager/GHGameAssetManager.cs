using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using System;
using System.Collections.Generic;


namespace GardenHose.Game.AssetManager;


internal class GHGameAssetManager
{
    // Internal fields.
    /* Planets. */
    internal SpriteAnimation PlanetAtmosphereDefault
    {
        get => _planetAtmosphereDefault ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/atmosphere/default");
    }

    internal SpriteAnimation PlanetGas1Surface
    {
        get => _planetGas1Surface ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/surface/gas_1");
    }

    internal SpriteAnimation PlanetGas1Overlay1
    {
        get => _planetGas1Overlay1 ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/overlays/gas_1_overlay_1");
    }
    internal SpriteAnimation PlanetGas1Overlay2
    {
        get => _planetGas1Overlay2 ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/overlays/gas_1_overlay_2");
    }

    internal SpriteAnimation PlanetRock1Land
    {
        get => _planetRock1Land ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/surface/rock1_land");
    }

    internal SpriteAnimation PlanetWater
    {
        get => _planetWater ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/surface/water");
    }

    internal SpriteAnimation PlanetClouds1
    {
        get => _planetClouds1 ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/clouds/clouds1");
    }

    internal SpriteAnimation PlanetClouds2
    {
        get => _planetClouds2 ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/clouds/clouds2");
    }

    internal SpriteAnimation PlanetClouds3
    {
        get => _planetClouds3 ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/clouds/clouds3");
    }

    internal SpriteAnimation PlanetClouds4
    {
        get => _planetClouds4 ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/clouds/clouds4");
    }

    internal SpriteAnimation PlanetClouds5
    {
        get => _planetClouds5 ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/clouds/clouds5");
    }


    /* Background. */
    internal SpriteAnimation BackgroundDefault
    {
        get => _backgroundDefault ??= new SpriteAnimation(0f, _parentFrame, Origin.TopLeft, "game/backgrounds/default");
    }
    internal SpriteAnimation BackgroundStarSmall
    {
        get => _backgroundStarSmall ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/stars/small");
    }

    internal SpriteAnimation BackgroundStarMedium
    {
        get => _backgroundStarMedium ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/stars/medium");
    }

    internal SpriteAnimation BackgroundStarBig
    {
        get => _backgroundStarBig ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/stars/big");
    }


    // Private fields.
    private IGameFrame _parentFrame;

    /* Planets. */
    private SpriteAnimation? _planetAtmosphereDefault;
    private SpriteAnimation? _planetGas1Surface;
    private SpriteAnimation? _planetGas1Overlay1;
    private SpriteAnimation? _planetGas1Overlay2;
    private SpriteAnimation? _planetRock1Land;
    private SpriteAnimation? _planetWater;
    private SpriteAnimation? _planetClouds1;
    private SpriteAnimation? _planetClouds2;
    private SpriteAnimation? _planetClouds3;
    private SpriteAnimation? _planetClouds4;
    private SpriteAnimation? _planetClouds5;

    /* Background. */
    private SpriteAnimation? _backgroundDefault;
    private SpriteAnimation? _backgroundStarSmall;
    private SpriteAnimation? _backgroundStarMedium;
    private SpriteAnimation? _backgroundStarBig;


    // Constructors.
    internal GHGameAssetManager(IGameFrame parentFrame)
    {
        _parentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
    }
}