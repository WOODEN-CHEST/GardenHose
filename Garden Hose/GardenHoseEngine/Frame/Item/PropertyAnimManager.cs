using GardenHoseEngine.Frame.Animation;

namespace GardenHoseEngine.Frame.Item;

public class PropertyAnimManager : ITimeUpdatable
{
    // Private fields.
    private readonly List<PropertyAnimation> _animations = new();
    private readonly List<PropertyAnimation> _animationsToRemove = new();
    

    // Constructors.
    internal PropertyAnimManager() { }

    // Methods.
    public void AddAnimation(PropertyAnimation animation)
    {
        _animations.Add(animation);
        animation.AnimationFinish += OnAnimationFinishEvent;
    }

    public void RemoveAnimation(PropertyAnimation animation)
    {
        _animations.Remove(animation);
    }


    // Private methods.
    private void OnAnimationFinishEvent(object? sender, AnimFinishEventArgs args)
    {
        PropertyAnimation Animation = (PropertyAnimation)sender!;

        if (Animation.IsLooped)
        {
            _animationsToRemove.Add(Animation);
            Animation.AnimationFinish -= OnAnimationFinishEvent;
        }
    }


    // Inherited methods.
    public void Update(IProgramTime time)
    {
        foreach (PropertyAnimation Animation in _animations)
        {
            Animation.Update(time);
        }
        foreach (PropertyAnimation Animation in _animationsToRemove)
        {
            _animations.Remove(Animation);
        }
    }
}