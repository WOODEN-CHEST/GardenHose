using GardenHoseEngine.IO.DataFile;
using GardenHoseEngine.Logging;
using GardenHoseEngine.Screen;
using System;
using System.IO;

namespace GardenHose.Settings;

internal static class SettingsManager
{
    // Internal static fields.
    internal readonly static GameSettings DefaultSettings = new()
    {
        IsFullScreen = false
    };

    internal static GameSettings Settings { get; set; }

    internal static string DataRootPath
    {
        get => s_dataRootPath;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!Path.IsPathFullyQualified(value))
            {
                throw new ArgumentException($"Path not fully qualified: \"{value}\"" ,nameof(value));
            }

            s_dataRootPath = value;
            s_settingsFilePath = Path.ChangeExtension(Path.Combine(value, "settings"), IDataFile.FileExtension);
        }
    }

    internal static string SettingsFilePath => s_settingsFilePath;

    /* IDs. */
    internal const int ID_SETTING_COMPOUND = 1;

    internal const int ID_IS_FULL_SCREEN = 1;


    // Private static fields.
    private static string s_dataRootPath;
    private static string s_settingsFilePath;



    // Static methods.
    internal static GameSettings ReadSettings()
    {
        if (!File.Exists(SettingsFilePath))
        {
            return CreateSettingsFile();
        }

        try
        {
            using DataFileReader Reader = DataFileReader.GetReader(SettingsFilePath);
            DataFileCompound BaseCompound = Reader.Read();
            DataFileCompound SettingCompound = BaseCompound.GetOrDefault(ID_SETTING_COMPOUND, new DataFileCompound());
            return ReadSettingsFromCompound(SettingCompound);
        }
        catch (InvalidCastException e)
        {
            Logger.Critical($"Corrupted settings file? Cast of setting failed. Creating new settings file. {e}");
            return CreateSettingsFile();
        }
    }

    internal static void WriteSettings()
    {
        DataFileCompound BaseCompound = new();
        DataFileCompound SettingsCompound = new();

        WriteSettingsToCompound(SettingsCompound);
        BaseCompound.Add(ID_SETTING_COMPOUND, SettingsCompound);

        DataFileWriter Writer = DataFileWriter.GetWriter();
        Writer.Write(SettingsFilePath, BaseCompound);
    }

    internal static void ApplySettings()
    {
        Display.IsFullScreen = Settings.IsFullScreen;
    }


    // Private static methods.
    private static GameSettings ReadSettingsFromCompound(DataFileCompound settingCompound)
    {
        return new GameSettings()
        {
            IsFullScreen = settingCompound.GetOrDefault(ID_IS_FULL_SCREEN, DefaultSettings.IsFullScreen)
        };
    }

    private static GameSettings CreateSettingsFile()
    {
        Settings = (GameSettings)DefaultSettings.Clone();
        WriteSettings();
        return Settings;
    }

    private static void WriteSettingsToCompound(DataFileCompound compound)
    {
        compound.Add(ID_IS_FULL_SCREEN, Settings.IsFullScreen);
    }
}