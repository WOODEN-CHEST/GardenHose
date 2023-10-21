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
    internal SpriteAnimation TestPlanet
    {
        get => _testPlanet ??= new SpriteAnimation(0f, _parentFrame, Origin.Center, "test/ball");
    }



    // Private fields.
    private IGameFrame _parentFrame;

    private SpriteAnimation? _testPlanet;



    // Constructors.
    internal GHGameAssetManager(IGameFrame parentFrame)
    {
        _parentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
    }
}