using System;

namespace GardenHoseEngine.Frame.UI.Animation;

public sealed partial class SpriteAnimation : IDisposable
{
    // Fields.
    public AnimationFrame[] Frames;
    public int MaxFrameIndex { get; init; }
    public float DefaultFPS
    {
        get => _defaultFPS;
        set
        {
            if (!float.IsFinite(value))
                throw new ArgumentException($"Abnormal default animation speed: {value}");
            _defaultFPS = value;
        }
    }


    // Private fields.
    private float _defaultFPS = 60f;


    // Constructors.
    public SpriteAnimation(float fps, params string[] framePaths)
    {
        if (framePaths == null) throw new ArgumentNullException(nameof(framePaths));
        if (framePaths.Length == 0) throw new ArgumentException("At least one animation frame is required.");

        DefaultFPS = fps;

        Frames = new AnimationFrame[framePaths.Length];
        MaxFrameIndex = Frames.Length - 1;

        for (int i = 0; i < Frames.Length; i++)
        {
            Frames[i] = new AnimationFrame(null, framePaths[i]);
        }
    }


    // Inherited methods.
    public void Dispose()
    {
        foreach (var Frame in Frames) Frame.Dispose();
        Frames = null;
    }
}