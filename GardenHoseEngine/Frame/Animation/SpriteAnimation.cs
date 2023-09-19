using System;

namespace GardenHoseEngine.Frame.Animation;

public sealed partial class SpriteAnimation
{
    // Fields.
    public AnimationFrame[] Frames => _frames.ToArray();

    public int MaxFrameIndex { get; init; }

    public double DefaultFPS
    {
        get => _defaultFPS;
        set
        {
            if (!double.IsFinite(value))
            {
                throw new ArgumentException($"Invalid default animation speed: {value}", nameof(value));
            }
                
            _defaultFPS = value;
        }
    }


    // Private fields.
    private double _defaultFPS;
    private readonly AnimationFrame[] _frames;


    // Constructors.
    public SpriteAnimation(double fps, GameFrame owner, AssetManager assetManager,
        Origin? textureOrigin, params string[] relativePaths)
    {
        ArgumentNullException.ThrowIfNull(assetManager, nameof(assetManager));
        DefaultFPS = fps;

        ArgumentNullException.ThrowIfNull(relativePaths, nameof(relativePaths));
        if (relativePaths.Length == 0)
        {
            throw new ArgumentException("At least one animation frame is required.", nameof(relativePaths));
        }

        _frames = new AnimationFrame[relativePaths.Length];
        for (int i = 0; i < _frames.Length; i++)
        {
            _frames[i] = new AnimationFrame(textureOrigin ?? Origin.TopLeft, assetManager.GetTexture(owner, relativePaths[i]));
        }
        MaxFrameIndex = _frames.Length - 1;
    }


    // Methods.
    public AnimationInstance CreateInstance(ITimeUpdater updater)
    {
        return new(this, updater);
    }


    // Operators.
    public AnimationFrame this[int index]
    {
        get => Frames[index];
    }
}