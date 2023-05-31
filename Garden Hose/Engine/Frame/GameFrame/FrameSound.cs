using System;
using System.Collections.Generic;
using GardenHose.Engine.Frame.UI.Animation;
using Microsoft.Xna.Framework.Audio;

namespace GardenHose.Engine.Frame;

public class FrameSound : IDisposable
{
    // Fields.
    public SoundEffect Sound { get; private set; }


    // Private fields.
    private string _relativePath;


    // Constructors.
    public FrameSound(string relativePath)
    {
        _relativePath = _relativePath ?? throw new ArgumentNullException(nameof(_relativePath));
        Sound = null;
    }


    // Methods.
    public SoundEffect Get()
    {
        if (Sound == null) Load();
        return Sound;
    }

    public void Load()
    {
        Sound = AssetManager.GetSound(_relativePath);
    }


    // Inherited methods.
    public void Dispose()
    {
        AssetManager.DisposeSound(_relativePath);
        Sound = null;
        _relativePath = null;
    }
}