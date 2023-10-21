using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.IO.DataFile;

public class DataFileCompound : IEnumerable<KeyValuePair<int, object>>
{
    // Private fields.
    private readonly Dictionary<int, object> _entries = new();


    // Methods.
    public DataFileCompound Add(int id, object entry)
    {
        VerifyID(id);

        if (_entries.ContainsKey(id))
        {
            throw new ArgumentException($"An entry with the id {id} already exists!", nameof(id));
        }
        _entries.Add(id, entry);

        return this;
    }

    public DataFileCompound Set(int id, object entry)
    {
        VerifyID(id);
        _entries[id] = entry;

        return this;
    }

    public T? Get<T>(int id)
    {
        _entries.TryGetValue(id, out var Value);
        return (T?)Value;
    }

    public T GetOrDefault<T>(int id, T defaultValue)
    {
        if (_entries.ContainsKey(id))
        {
            return (T)_entries[id];
        }

        return defaultValue;
    }

    public void Remove(int id) => _entries.Remove(id);

    public void Clear() => _entries.Clear();


    // Private methods.
    private void VerifyID(int id)
    {
        if (id == 0)
        {
            throw new ArgumentException("ID cannot be zero.", nameof(id));
        }
    }


    // Inherited methods.
    public IEnumerator<KeyValuePair<int, object>> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}