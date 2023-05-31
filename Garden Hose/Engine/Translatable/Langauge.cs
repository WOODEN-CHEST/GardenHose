using System;
using System.Collections.Generic;


namespace GardenHose.Engine.Translatable;

public class Langauge
{
    // Fields.
    public readonly string Name;


    // Private fields.
    private readonly Dictionary<string, string> _text;


    // Constructors.
    public Langauge(string name)
    {
        Name = name;
    }


    // Methods.
    public string GetText(string key)
    {
        string Value;
        if (_text.TryGetValue(key, out Value)) return Value;
        return key;
    }

    public void AddTranslation(string key, string translation) => _text.Add(key, translation);
}