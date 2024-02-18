using GardenHoseEngine.Frame.Item;


namespace GardenHoseEngine.Frame;

public interface ILayer : IColorMaskable, IDrawableItem
{
    // Static fields.
    public const float DEFAULT_Z_INDEX = 1.0f;


    // Properties.
    string Name { get; }

    public int DrawableItemCount { get; }


    // Methods.
    public void AddDrawableItem(IDrawableItem item);

    public void AddDrawableItem(IDrawableItem item, float zindex);

    public void RemoveDrawableItem(IDrawableItem item);

    public void ClearDrawableItems();
}