namespace GardenHose.Engine.Frame.UI.Item;

public interface IDrawableItem
{
    // Properties.
    public bool IsVisible { get; set; }


    // Methods.
    public void Draw();

    public void OnDisplayChange();
}