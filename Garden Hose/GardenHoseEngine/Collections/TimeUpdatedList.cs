using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Collections;

public class DiscreteTimeList<T> : IEnumerable<T>
{
    // Fields.
    public int Count => _current.Count;


    // Private fields.
    private readonly List<T> _current;
    private readonly List<T> _toRemove = new();
    private readonly List<T> _toAdd = new();


    // Constructors.
    public DiscreteTimeList()
    {
        _current = new();
    }

    public DiscreteTimeList(IEnumerable<T> startingList)
    {
        _current = new(startingList);
    }

    // Methods.
    public void Add(T item) => _toAdd.Add(item);

    public void Remove(T item) => _toRemove.Add(item);

    public void Clear() => _toRemove.AddRange(_current);

    public void ForceClear()
    {
        _toAdd.Clear();
        _current.Clear();
        _toRemove.Clear();
    }

    public void ApplyAddedItems()
    {
        foreach (var Item in _toAdd)
        {
            _current.Add(Item);
        }
        _toAdd.Clear();
    }

    public void ApplyRemovedItems()
    {
        foreach (var Item in _toRemove)
        {
            _current.Remove(Item);
        }
        _toRemove.Clear();
    }

    public void ApplyChanges()
    {
        ApplyAddedItems();
        ApplyRemovedItems();
    }


    // Inherited methods.
    public IEnumerator<T> GetEnumerator()
    {
        foreach (var Item in _current)
        {
            yield return Item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    // Operators.
    public T this[int index]
    {
        get => _current[index];
    }
}