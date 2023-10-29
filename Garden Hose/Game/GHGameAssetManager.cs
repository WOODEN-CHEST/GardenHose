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



    // Private fields.
    private IGameFrame _parentFrame;

    /* Planets. */
    private SpriteAnimation? _planetAtmosphere1Texture;
    private SpriteAnimation? _planetGas1Texture;


    // Constructors.
    internal GHGameAssetManager(IGameFrame parentFrame)
    {
        _parentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
    }
}