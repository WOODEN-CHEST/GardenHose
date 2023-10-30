using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game;

internal class GHGameAssetManager
{
    // Internal fields.
    /* Planets. */
    internal SpriteAnimation PlanetAtmosphereDefaultTexture
    {
        get => _planetAtmosphere1Texture ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/atmosphere/default");
    }

    internal SpriteAnimation PlanetGas1Texture
    {
        get => _planetGas1Texture ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/planets/surface/gas_1");
    }


    /* Background. */
    internal SpriteAnimation BackgroundDefault
    {
        get => _backgroundDefault ??= new SpriteAnimation(0f, _parentFrame, Origin.TopLeft, "game/backgrounds/default");
    }
    internal SpriteAnimation BGStarSmall
    {
        get => _bgStarSmall ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/stars/small");
    }

    internal SpriteAnimation BGStarMedium
    {
        get => _bgStarMedium ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/stars/medium");
    }

    internal SpriteAnimation BGStarBig
    {
        get => _bgStarBig ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "game/stars/big");
    }


    // Private fields.
    private IGameFrame _parentFrame;

    /* Planets. */
    private SpriteAnimation? _planetAtmosphere1Texture;
    private SpriteAnimation? _planetGas1Texture;

    /* Background. */
    private SpriteAnimation? _backgroundDefault;
    private SpriteAnimation? _bgStarSmall;
    private SpriteAnimation? _bgStarMedium;
    private SpriteAnimation? _bgStarBig;


    // Constructors.
    internal GHGameAssetManager(IGameFrame parentFrame)
    {
        _parentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
    }
}