using System;
using Microsoft.Xna.Framework;


namespace GardenHose.Engine.Frame.UI.Item;

public class Line : ColoredItem
{
    // Public fields.
    public float Thickness
    {
        get => _thickness;
        set
        {
            if (!float.IsNormal(value)) throw new ArgumentException($"Invalid thickness value: \"{value}\"");
            _thickness = value;
        }
    }

    public Vector2 PositionEnd
    {
        get => _virtualPosEnd;
        set
        {
            _virtualPosEnd = value;

            _realPosEnd = value;
            DisplayInfo.VirtualToRealPosition(ref _realPosEnd);
        }
    }


    // Private fields.
    private float _thickness = 0f;
    private Vector2 _virtualPosEnd = Vector2.Zero;
    private Vector2 _realPosEnd = Vector2.Zero;


    // Inherited methods.
    public override void Draw()
    {
        base.Draw();

        if (!ShouldRender) return;
        GameFrame.DrawLine(Position, PositionEnd, Thickness, RealColorMask);
    }
}