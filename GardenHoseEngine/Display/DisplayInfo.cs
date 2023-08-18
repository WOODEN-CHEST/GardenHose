using Microsoft.Xna.Framework;


namespace GardenHoseEngine;


public readonly struct DisplayInfo
{
    // Fields.
    public readonly Vector2 Size;
    public readonly bool IsFullScreen;


    // Constructors.
    public DisplayInfo(Vector2 size, bool isFullScreen)
    {
        if (float.IsNaN(size.X) || float.IsNaN(size.Y) 
            || float.IsInfinity(size.X) || float.IsInfinity(size.Y))
        {
            throw new ArgumentException($"Invalid display size: {size}");
        }

        size.X = Math.Max(Display.MinWidth, size.X);
        size.Y = Math.Max(Display.MinHeight, size.Y);

        Size = size;
        IsFullScreen = isFullScreen;
    }
}