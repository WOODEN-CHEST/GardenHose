using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GardenHose.Engine.IO;

// All this code just works, somehow. Must not touch it, it may break then.
internal static class UserInput
{
    public static void ListenForInput()
    {
        KeyboardEventListener.ListenForInput();
        MouseEventListener.ListenForInput();
    }
}