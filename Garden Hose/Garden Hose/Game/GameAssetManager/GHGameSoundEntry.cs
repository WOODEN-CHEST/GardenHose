using GardenHoseEngine;
using GardenHoseEngine.Audio;
using GardenHoseEngine.Frame;
using System;

namespace GardenHose.Game.GameAssetManager;

internal class GHGameSoundEntry
{
    // Private fields.
    private string _soundName;
    private Sound? _sound;


    // Constructors.
    internal GHGameSoundEntry(string soundName)
    {
        _soundName = soundName ?? throw new ArgumentNullException(nameof(soundName));
    }


    // Internal methods.
    internal Sound GetSound(IGameFrame? owner)
    {
        if (_sound == null)
        {
            _sound = AssetManager.GetSoundEffect(owner, _soundName);
        }
        return _sound;
    }
}