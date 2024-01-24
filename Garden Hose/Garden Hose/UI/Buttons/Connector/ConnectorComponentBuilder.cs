using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;


namespace GardenHose.UI.Buttons.Connector;


internal abstract partial class ConnectorElement
{
    // Internal static methods.
    /* Rectangle button. */
    internal static ConnectorRectangleButton CreateNormalButton(Direction connectDirection, Vector2 position, Vector2 scale)
    {
        return CreateRectangleButton(RectangleButtonType.Normal, connectDirection, position, scale);
    }

    internal static ConnectorRectangleButton CreateSquareButton(Direction connectDirection, Vector2 position, Vector2 scale)
    {
        return CreateRectangleButton(RectangleButtonType.Square, connectDirection, position, scale);
    }

    internal static ConnectorRectangleButton CreateNarrowButton(Direction connectDirection, Vector2 position, Vector2 scale)
    {
        return CreateRectangleButton(RectangleButtonType.Narrow, connectDirection, position, scale);
    }

    internal static ConnectorRectangleButton CreateWideButton(Direction connectDirection, Vector2 position, Vector2 scale)
    {
        return CreateRectangleButton(RectangleButtonType.Wide, connectDirection, position, scale);
    }

    /* Drop-down list */
    internal static DropDownList<T> CreateDropDownList<T>(Direction connectDirection, T[]? options, int selectedOption)
    {
        return new DropDownList<T>(connectDirection, options, selectedOption);
    }


    // Private static methods.
    private static ConnectorRectangleButton CreateRectangleButton(RectangleButtonType buttonType,
        Direction connectDirection, 
        Vector2 position, 
        Vector2 scale)
    {
        AnimationInstance PanelInstance;
        AnimationInstance GlowInstance;
        Vector2 Size;

        switch (buttonType)
        {
            case RectangleButtonType.Normal:
                PanelInstance = NormalPanelInstance!;
                GlowInstance = NormalGlowInstance!;
                Size = new Vector2(800, 200);
                break;

            case RectangleButtonType.Square:
                PanelInstance = SquarePanelInstance!;
                GlowInstance = SquareGlowInstance!;
                Size = new Vector2(200, 200);
                break;

            case RectangleButtonType.Narrow:
                PanelInstance = NarrowPanelInstance!;
                GlowInstance = NarrowGlowInstance!;
                Size = new Vector2(400, 200);
                break;

            case RectangleButtonType.Wide:
                PanelInstance = WidePanelInstance!;
                GlowInstance = WideGlowInstance!;
                Size = new Vector2(1600, 200);
                break;

            default:
                throw new EnumValueException(nameof(buttonType), nameof(RectangleButtonType),
                    buttonType.ToString(), (int)buttonType);
        }

        ConnectorRectangleButton Button = new(connectDirection,
            new SpriteItem(PanelInstance),
            new SpriteItem(GlowInstance),
            position, scale, Size);

        return Button;
    }


    // Types.
    private enum RectangleButtonType
    {
        Normal,
        Square,
        Narrow,
        Wide
    }
}