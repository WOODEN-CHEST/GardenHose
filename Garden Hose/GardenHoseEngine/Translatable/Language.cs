using System.Collections.Generic;
using System.Linq;
using System;
using System.Transactions;

namespace GardenHoseEngine.Translatable;

public sealed class Language
{
    // Fields.
    public readonly string Name;


    // Private fields.
    private readonly Dictionary<string, string> _definitions;


    // Constructors.
    public Language(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException($"Invalid language name \"{name}\"", nameof(name));
        Name = name;
    }


    // Methods.
    public string GetText(string key) => this[key];

    public void AddTranslation(string key, string translation) => this[key] = translation;


    // Operators
    public string this[string key]
    {
        get
        {
            if (_definitions.TryGetValue(key, out string? Value))
            {
                return Value!;
            }
            return key;
        }
        set
        {
            _definitions[key] = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}