using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Buttons;

public enum ButtonEvent
{
    LeftClick = 0,
    MiddleClick = 1,
    RightClick = 2,

    LeftRelease = 3,
    MiddleRelease = 4,
    RightRelease = 5,

    LeftHold = 6,
    MiddleHold = 7,
    RightHold = 8,

    ScrollUp = 9,
    ScrollDown = 10,
    Scroll = 11,

    OnHover,
    Hovering,
    OnUnhover,
    NotHovering
}