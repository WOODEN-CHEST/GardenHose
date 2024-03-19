using GardenHoseEngine.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Player.Sound;

internal class SoundManager
{
    // Private fields.
    private readonly List<WorldSound> _sounds = new();


    // Constructors.
    internal SoundManager()
    {

    }


    // Internal methods.
    internal void Tick(GHGameTime time, WorldPlayer player)
    {
        foreach (WorldSound Sound in _sounds)
        {
            Sound.Tick(time, player);
        }
    }

    internal void AddSound(WorldSound sound)
    {
        _sounds.Add(sound ?? throw new ArgumentNullException(nameof(sound)));
        sound.Play();
        sound.SoundFinished += OnSoundFinishedEvent;
    }

    internal void RemoveSound(WorldSound sound)
    {
        _sounds.Remove(sound ?? throw new ArgumentNullException(nameof(sound)));
        sound.Stop();
        sound.SoundFinished -= OnSoundFinishedEvent;
    }

    internal void ClearSounds()
    {
        foreach (WorldSound Sound in _sounds)
        {
            Sound.Stop();
        }
        _sounds.Clear();
    }


    // Private methods.
    private void OnSoundFinishedEvent(object? sender, EventArgs args)
    {
        lock (_sounds)
        {
            RemoveSound((WorldSound)sender!);
        }
    }
}