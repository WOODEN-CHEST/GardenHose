using GardenHoseEngine.Frame.UI.Animation;


namespace GardenHoseEngine.Frame.UI.Item;

public interface ISpriteItem
{
    public AnimationInstance ActiveAnimation { get; set; }
}