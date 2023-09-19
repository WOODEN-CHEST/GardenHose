using GardenHoseEngine.Frame.Animation;

namespace GardenHoseEngine.Frame.Item;

public interface ISpriteItem : IDrawableItem
{
    public AnimationInstance ActiveAnimation { get; set; }
}