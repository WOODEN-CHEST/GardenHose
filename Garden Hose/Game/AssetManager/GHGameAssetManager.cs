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