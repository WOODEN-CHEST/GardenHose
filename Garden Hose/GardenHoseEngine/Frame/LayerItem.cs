using GardenHoseEngine.Frame.Item;
using System.Diagnostics.CodeAnalysis;

namespace GardenHoseEngine.Frame;

internal struct LayerItem
{
    // Internal fields.
    internal float ZIndex { get; init; }
    internal IDrawableItem Item { get; init; }


    // Constructors.
    internal LayerItem(IDrawableItem item, float zIndex)
    {
        ZIndex = zIndex;
        Item = item;
    }


    // Inherited methods.
    public override bool Equals([NotNullWhen(true)] object? obj) => Item.Equals(obj);

    public override int GetHashCode() => Item.GetHashCode();

    public static bool operator ==(LayerItem left, LayerItem right) => left.Equals(right);
    public static bool operator !=(LayerItem left, LayerItem right) => !left.Equals(right);
}