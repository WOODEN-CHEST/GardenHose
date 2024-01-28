using GardenHoseEngine.Frame.Animation;

namespace GardenHoseEngine.Frame.Item;

public class PropertyAnimation : ITimeUpdatable
{
    // Internal fields.
    public SpriteItem Item { get; private init; }
    public KeyframeCollection KeyFrames { get; } = new();
    public float Speed { get; set; } = 1f;
    public bool IsLooped { get; set; } = false;
    public bool IsPlaying { get; set; } = true;
    public float AnimationTime { get; set; } = 0f;

    public event EventHandler<AnimFinishEventArgs>? AnimationFinish;


    // Constructors.
    public PropertyAnimation(SpriteItem item)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
    }

    public void Update(IProgramTime time)
    {
        if (!IsPlaying) return;

        AnimationTime += time.PassedTimeSeconds * Speed;
        if (AnimationTime > KeyFrames.Duration)
        {
            AnimationFinish?.Invoke(this, new(FinishLocation.End));
            if (IsLooped)
            {
                AnimationTime = 0f;
            }
            else
            {
                IsPlaying = false;
            }
        }
        else if (AnimationTime < 0f)
        {
            AnimationFinish?.Invoke(this, new(FinishLocation.Start));
            if (IsLooped)
            {
                AnimationTime = KeyFrames.Duration;
            }
            else
            {
                IsPlaying = false;
            }
        }

        PropertyKeyframe PreviousFrame = KeyFrames.GetPrevious(AnimationTime);
        PropertyKeyframe NextFrame = KeyFrames.GetNext(AnimationTime);

        float Progress = (NextFrame.Time - PreviousFrame.Time) / (NextFrame.Time - AnimationTime);

        Item.Position = GHMath.Interpolate(NextFrame.Interpolation, PreviousFrame.Position, NextFrame.Position, Progress);
        Item.Size = GHMath.Interpolate(NextFrame.Interpolation, PreviousFrame.Size, NextFrame.Size, Progress);
        Item.Rotation = GHMath.Interpolate(NextFrame.Interpolation, PreviousFrame.Rotation, NextFrame.Rotation, Progress);
    }
}