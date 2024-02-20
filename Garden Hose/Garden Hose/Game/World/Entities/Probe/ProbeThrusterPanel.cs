using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeThrusterPanel
{
    // Internal fields.
    internal float ObservedMTValue { get; private set; }
    internal float ObservedRTValue { get; private set; }
    internal float ObservedLTValue { get; private set; }


    // Private fields.
    

    private SpriteItem[] _mainThrusterLights;
    private SpriteItem[] _rightThrusterLights;
    private SpriteItem[] _leftThrusterLights;



    // Constructors.
    internal ProbeThrusterPanel()
    {

    }


    // Internal methods.
    internal void Tick(ProbeEntity probe)
    {

    }

    internal void Load(GHGameAssetManager assetManager)
    {

    }

    internal void Draw(IDrawInfo info)
    {

    }


    // Private methods.
    private void SetLights(float thrustAmount, )
}