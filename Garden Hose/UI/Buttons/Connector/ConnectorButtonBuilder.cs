using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GardenHose.UI.Buttons.Connector;

internal partial class ConnectorButton
{
    // Private static fields.
    private static readonly Vector2 s_normalSize = new(800, 200);

    // Internal static methods.
    /* Rectangle. */
    internal static ConnectorButton CreateNormal(ITimeUpdater frame, Direction connectDirection, IDrawer? drawer)
    {
        return CreateNormal(frame, connectDirection, Vector2.Zero, drawer);
    }

    internal static ConnectorButton CreateNormal(ITimeUpdater frame, Direction connectDirection, Vector2 position, IDrawer? drawer)
    {
        return CreateNormal(frame, connectDirection, Vector2.Zero, Vector2.One, drawer);
    }

    internal static ConnectorButton CreateNormal(ITimeUpdater frame, Direction connectDirection, Vector2 position,
        Vector2 scale, IDrawer? drawer)
    {
        return CreateButton(frame, ConnectorButtonShape.Rectangle, connectDirection, position, scale, s_normalSize, drawer);
    }

    /* Square */
    internal static ConnectorButton CreateSquare()
    {
        throw new NotImplementedException();
    }


    // Private static methods.
    private static ConnectorButton CreateButton(ITimeUpdater frame, 
        ConnectorButtonShape shape, 
        Direction connectDirection, 
        Vector2 position, 
        Vector2 scale,
        Vector2 size,
        IDrawer? drawer)
    {
        if (frame == null)
        {
            throw new ArgumentNullException(nameof(frame));
        }

        SpriteAnimation PanelAnim;
        SpriteAnimation GlowAnim;

        switch (shape)
        {
            case ConnectorButtonShape.Rectangle:
                PanelAnim = s_normalPanelAnim!;
                GlowAnim = s_normalGlowAnim!;
                break;

            default:
                throw new EnumValueException(nameof(shape), nameof(ConnectorButtonShape),
                    shape.ToString(), (int)shape);
        }

        ConnectorButton Button = new(frame, drawer, connectDirection,
            new SpriteItem(frame, GH.Engine.Display, drawer, PanelAnim),
            new SpriteItem(frame, GH.Engine.Display, drawer, GlowAnim),
            position, scale, size);

        return Button;
    }
}