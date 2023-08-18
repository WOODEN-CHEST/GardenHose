using GardenHoseEngine.Frame.UI.Animation;
using System;

namespace GardenHoseEngine.Frame;

public class FrameAnimation : IDisposable
{
    // Fields.
    public SpriteAnimation Anim { get; private set; }


    // Private fields.
    private Func<SpriteAnimation> _creator = null;


    // Constructors.
    public FrameAnimation(Func<SpriteAnimation> creator)
    {
        _creator = creator ?? throw new ArgumentNullException(nameof(creator));
        Anim = null;
    }


    // Methods.
    public SpriteAnimation Get()
    {
        if (Anim == null) Load();
        return Anim;
    }

    public void Load()
    {
        Anim = _creator.Invoke();
    }


    // Inherited methods.
    public void Dispose()
    {
        Anim?.Dispose();
        Anim = null;
        _creator = null;
    }
}