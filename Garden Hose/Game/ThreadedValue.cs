using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game;

internal class ThreadedValue<T>
{
    // Internal fields.
    internal T NewValue { get; set; }

    internal T Value { get; set; }


    // Constructors.
    internal ThreadedValue(T value)
    {
        NewValue = value;
        Value = value;
    }

    //Internal methods.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void UpdateValue() => Value = NewValue;
}