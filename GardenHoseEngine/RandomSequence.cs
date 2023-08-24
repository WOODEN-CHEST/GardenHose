using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine;

public class RandomSequence<T>
{
    // Fields.
    public T[] Items
    {
        get => _items.ToArray();
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value.Length == 0)
            {
                throw new ArgumentException("At least one item is required in a random sequence.");
            }

            _items = value;
        }
    }


    // Private fields.
    private T[] _items;
    private int _index;


    // Constructors.
    public RandomSequence(T[] items)
    {
        Items = items;
        Randomize();
    }


    // Methods.
    public T GetItem()
    {
        if (_index == -1)
        {
            Randomize();
            _index = _items.Length - 1;
        }

        return _items[_index--];
    }

    public void Randomize()
    {
        int RandIndex;

        for (int Index = 0; Index < (_items.Length / 2); Index++)
        {
            RandIndex = Random.Shared.Next(_items.Length);
            (_items[RandIndex], _items[Index]) = (_items[Index], _items[RandIndex]);
        }
    }
}