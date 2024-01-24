using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Translatable;

public class LanguageFileParser
{
    // Constructors.
    public LanguageFileParser() { }


    // Methods.
    public Language ParseLanguage(string path)
    {
        try
        {
            Language Lang = new(Path.ChangeExtension(Path.GetFileName(path), null).ToLower());

            string[] Definitions = File.ReadAllText(path).Split(';');

            foreach (string Definition in Definitions)
            {
                string[] KeyAndValue = Definition.Trim().Split('=');
                
                if (KeyAndValue.Length < 2)
                {
                    throw new LanguageFileFormatException(
                        $"No matching definition found for key \"{KeyAndValue[0]}\"");
                }

                Lang.AddTranslation(KeyAndValue[0].ToLower(), KeyAndValue[1]);
            }

            return Lang;
        }
        catch (Exception e) when (e is IOException or LanguageFileFormatException)
        {
            throw new AssetLoadException(path, e.ToString());
        }
    }
}