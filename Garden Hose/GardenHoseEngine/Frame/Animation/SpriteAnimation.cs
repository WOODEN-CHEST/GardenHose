using System;

namespace GardenHoseEngine.Frame.Animation;

public sealed partial class SpriteAnimation
{
    // Fields.
    public ReadOnlySpan<AnimationFrame> Frames => _frames.AsSpan();

    public int MaxFrameIndex { get; init; }

    public float DefaultFPS { get; set; }


    // Private fields.
    private readonly AnimationFrame[] _frames;


    // Constructors.
    public SpriteAnimation(float fps, IGameFrame? owner, Origin? textureOrigin, params string[] relativePaths)
    {
        DefaultFPS = fps;

        if (relativePaths == null)
        {
            throw new ArgumentNullException(nameof(relativePaths));
        }
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