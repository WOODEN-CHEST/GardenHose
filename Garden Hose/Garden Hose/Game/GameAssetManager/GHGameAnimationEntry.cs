using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.GameAssetManager;

internal class GHGameAnimationEntry
{
    // Private fields.
    private SpriteAnimation? _animation;
    private readonly string[] _assetNames;
    private readonly float _fps;
    private readonly Origin _origin;


    // Constructors.
    internal GHGameAnimationEntry(float fps, Origin origin, params string[] assetNames)
    {
        _fps = fps;
        _origin = origin;
        _assetNames = assetNames ?? throw new ArgumentNullException(nameof(assetNames));
    }


    // Internal methods.
    internal SpriteAnimation GetAnimation(IGameFrame owner)
    {
        if (_animation == null)
        {
            _animation = new(_fps, owner, _origin, _assetNames);
        }
        return _animation;
    }
}