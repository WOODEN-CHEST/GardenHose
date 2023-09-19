using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine;

public class DeltaValue<T>
{
    // Fields.
    public T? Current { get; private set; }
    public T? Previous { get; private set; }


    // Constructors.
    public DeltaValue(T startingValue)
    {
        Current = startingValue;
        Previous = startingValue;
    }

    public DeltaValue()
    {
        Current = default;
        Previous = default;
    }


    // Methods.
    public void Update(T? newValue)
    {
        Previous = Current;
        Current = newValue;
    }
}