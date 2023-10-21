using System;

namespace GardenHoseEngine.Frame.Animation;

public sealed partial class SpriteAnimation
{
    // Fields.
    public AnimationFrame[] Frames => _frames.ToArray();

    public int MaxFrameIndex { get; init; }

    public float DefaultFPS
    {
        get => _defaultFPS;
        set
        {
            if (!float.IsFinite(value))
            {
                throw new ArgumentException($"Invalid default animation speed: {value}", nameof(value));
            }
                
            _defaultFPS = value;
        }
    }


    // Private fields.
    private float _defaultFPS;
    private readonly AnimationFrame[] _frames;


    // Constructors.
    public SpriteAnimation(float fps, IGameFrame owner, Origin? textureOrigin, params string[] relativePaths)
    {
        DefaultFPS = fps;

        ArgumentNullException.ThrowIfNull(relativePaths, nameof(relativePaths));
        if (relativePaths.Length == 0)
        {
            throw new ArgumentException("At least one animation frame is required.", nameof(relativePaths));
        }

        _frames = new AnimationFrame[relativePaths.Length];
        for (int i = 0; i < _frames.Length; i++)
        {
            _frames[i] = new AnimationFrame(textureOrigin ?? Origin.TopLeft, AssetManager.GetTexture(owner, relativePaths[i]));
        }
        MaxFrameIndex = _frames.Length - 1;
    }


    // Methods.
    public AnimationInstance CreateInstance()
    {
        return new(this);
    }


    // Operators.
    public AnimationFrame this[int index]
    {
        get => Frames[index];
    }
}