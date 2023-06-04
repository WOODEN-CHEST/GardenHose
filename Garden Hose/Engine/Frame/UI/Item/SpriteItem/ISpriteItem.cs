using GardenHose.Engine.Frame.UI.Animation;


namespace GardenHose.Engine.Frame.UI.Item;

public interface ISpriteItem
{
    public AnimationInstance ActiveAnimation { get; set; }
}