using System.Collections.Generic;
using System.Linq;
using System;

namespace GardenHoseEngine.Translatable;

public sealed class Language
{
    // Static fields.
    public static List<Language> Langauges
    {
        get
        {
            List<Language> Langs = new(s_languages.Values);
            Langs.Sort();
            return Langs;
        }
    }


    // Private static fields.
    private static readonly Dictionary<string, Language> s_languages;


    // Fields.
    public readonly string Name;


    // Private fields.
    private readonly Dictionary<string, string> _text;


    // Constructors.
    public Language(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArithmeticException($"Invalid language name \"{name}\"");
        Name = name;

        s_languages.Add(Name, this);
    }


    // Methods.
    public string GetText(string key)
    {
        if (_text.TryGetValue(key, out string Value)) return Value;
        return key;
    }

    public void AddTranslation(string key, string translation) => _text[key] = translation;
}