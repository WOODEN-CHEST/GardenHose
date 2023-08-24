using Microsoft.Xna.Framework.Audio;
using System;

namespace GardenHoseEngine.Frame;

public class FrameSound : IDisposable
{
    // Fields.
    public SoundEffect Sound { get; private set; }


    // Private fields.
    private string _relativePath;


    // Constructors.
    public FrameSound(string relativePath)
    {
        _relativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
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
        Sound = gfdgdfg.GetSound(_relativePath);
    }


    // Inherited methods.
    public void Dispose()
    {
        gfdgdfg.DisposeSound(_relativePath);
        Sound = null;
        _relativePath = null;
    }
}