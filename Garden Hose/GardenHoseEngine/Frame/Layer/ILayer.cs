using GardenHoseEngine.Frame.Item;


namespace GardenHoseEngine.Frame;

public interface ILayer : IColorMaskable, IDrawableItem
{
    // Properties.
    string Name { get; }

    public int DrawableItemCount { get; }


    // Methods.
    public void AddDrawableItem(IDrawableItem item);

    public void AddDrawableItem(IDrawableItem item, int index);

    public void RemoveDrawableItem(IDrawableItem item);

    public void ClearDrawableItems();
}