using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World;

internal class TickedFunctionAttribute : Attribute
{
    // Fields.
    internal bool CanBeCalledOutsideTick { get; private init; }


    // Constructors.
    internal TickedFunctionAttribute(bool canBeCalledOutsideTick) => CanBeCalledOutsideTick = canBeCalledOutsideTick;
}