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
    internal static ConnectorButton CreateNormal(Direction connectDirection)
    {
        return CreateNormal(connectDirection, Vector2.Zero);
    }

    internal static ConnectorButton CreateNormal(Direction connectDirection, Vector2 position)
    {
        return CreateNormal(connectDirection, Vector2.Zero, Vector2.One);
    }

    internal static ConnectorButton CreateNormal(Direction connectDirection, Vector2 position, Vector2 scale)
    {
        return CreateButton(ConnectorButtonShape.Rectangle, connectDirection, position, scale, s_normalSize);
    }

    /* Square */
    internal static ConnectorButton CreateSquare()
    {
        throw new NotImplementedException();
    }


    // Private static methods.
    private static ConnectorButton CreateButton(ConnectorButtonShape shape, 
        Direction connectDirection, 
        Vector2 position, 
        Vector2 scale,
        Vector2 size)
    {
        AnimationInstance PanelInstance;
        AnimationInstance GlowInstance;

        switch (shape)
        {
            case ConnectorButtonShape.Rectangle:
                PanelInstance = s_normalPanelInstance!;
                GlowInstance = s_normalGlowInstance!;
                break;

            default:
                throw new EnumValueException(nameof(shape), nameof(ConnectorButtonShape),
                    shape.ToString(), (int)shape);
        }

        ConnectorButton Button = new(connectDirection,
            new SpriteItem(GH.Engine.Display, PanelInstance),
            new SpriteItem(GH.Engine.Display, GlowInstance),
            position, scale, size);

        return Button;
    }
}