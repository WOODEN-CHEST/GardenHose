using GardenHoseEngine.IO.DataFile;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
    internal const int SETTING_COMPOUND = 1;

    internal const int IS_FULL_SCREEN = 1;


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
            DataCompound BaseCompound = DataFileReaderCreator.GetReader().Read(SettingsFilePath, (version) => false)!;
            DataCompound SettingCompound = BaseCompound.GetItemOrDefault(SETTING_COMPOUND, new DataCompound());
            return ReadSettingsFromCompound(SettingCompound);
        }
        catch (InvalidCastException e)
        {
            GH.Engine.Logger.Critical($"Corrupted settings file? Creating new one. {e}");
            return CreateSettingsFile();
        }
    }

    internal static void WriteSettings()
    {
        IWriteableDataCompound BaseCompound = DataFileWriterCreator.GetCompound(null);
        IWriteableDataCompound SettingsCompound = DataFileWriterCreator.GetCompound(1);

        WriteSettingsToCompound(SettingsCompound);
        BaseCompound.WriteCompound(SettingsCompound);

        DataFileWriter Writer = DataFileWriterCreator.GetWriter();
        Writer.Write(SettingsFilePath, GH.GameVersion, BaseCompound);
    }

    internal static void ApplySettings()
    {
        GH.Engine.Display.IsFullScreen = Settings.IsFullScreen;
    }


    // Private static methods.
    private static GameSettings ReadSettingsFromCompound(DataCompound settingCompound)
    {
        return new()
        {
            IsFullScreen = settingCompound.GetItemOrDefault(IS_FULL_SCREEN, DefaultSettings.IsFullScreen)
        };
    }

    private static GameSettings CreateSettingsFile()
    {
        Settings = (GameSettings)DefaultSettings.Clone();
        WriteSettings();
        return Settings;
    }

    private static void WriteSettingsToCompound(IWriteableDataCompound compound)
    {
        compound.WriteBool(IS_FULL_SCREEN, Settings.IsFullScreen);
    }
}