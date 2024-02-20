using GardenHose.Game.GameAssetManager;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Buttons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeThrusterIndicator
{
    // Internal static fields.
    private const int SWITCH_OFF_ANIM_INDEX = 0;
    private const int SWITCH_ON_ANIM_INDEX = 1;

    // Internal fields.
    internal const int INDICATOR_LIGHT_COUNT = 4;
    internal const float MIN_VALUE = 0f;
    internal const float MAX_VALUE = 1f;

    internal float Value
    {
        get => _value;
        set => _value = Math.Clamp(value, MIN_VALUE, MAX_VALUE);
    }

    internal Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
        }
    }


    // Private fields.
    private Vector2 _position;
    private float _value = 0f;

    private SpriteItem _switch;
    private SpriteButton _switchPanel;

    private SpriteItem[] _lightDisplays;

    


    // Constructors.
    internal ProbeThrusterIndicator()
    {

    }

    internal void Load(GHGameAssetManager assetManager)
    {
        _lightDisplays = new SpriteItem[INDICATOR_LIGHT_COUNT];
        for (int i = 0; i < INDICATOR_LIGHT_COUNT; i++)
        {
            _lightDisplays[i] = new(assetManager)
        }
    }

    internal void Draw(IDrawInfo info)
    {

    }

}