using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.IO.DataFile;

public class DataCompound
{
    // Private fields.
    private readonly Dictionary<int, dynamic> _items;


    // Constructors.
    public DataCompound() => _items = new();

    public DataCompound(int capacity) => _items = new(capacity);


    // Methods.
    public void AddItem(int id, dynamic value)
    {
        //ArgumentNullException.ThrowIfNull(value);

        if (id == 0)
        {
            throw new DataReadException("ID of a DataFile entry cannot be 0");
        }

        if (_items.ContainsKey(id))
        {
            throw new DataReadException($"An object with the ID {id} already exists in the DataFile");
        }

        _items[id] = value;
    }

    public bool GetItem(int id, out dynamic value)
    {
        if (_items.TryGetValue(id, out value))
        {
            return true;
        }
        else return false;
    }

    public dynamic GetItemOrDefault(int id, dynamic defaultValue)
    {
        if (_items.TryGetValue(id, out dynamic Value))
        {
            return Value;
        }
        else return defaultValue;
    }
}