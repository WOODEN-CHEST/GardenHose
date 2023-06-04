using GardenHose.Engine.Frame.UI.Animation;
using System;

namespace GardenHose.Engine.Frame;

public class FrameAnimation : IDisposable
{
    // Fields.
    public Animation Anim { get; private set; }


    // Private fields.
    private Func<Animation> _creator = null;


    // Constructors.
    public FrameAnimation(Func<Animation> creator)
    {
        _creator = creator ?? throw new ArgumentNullException(nameof(creator));
        Anim = null;
    }


    // Methods.
    public Animation Get()
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