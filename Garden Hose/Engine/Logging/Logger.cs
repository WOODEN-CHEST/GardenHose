using System;
using System.IO;
using System.Text;

namespace GardenHose.Engine.Logging;

public enum LogLevel
{
    Info,
    Warning,
    Error,
    CRITICAL
}

public static class Logger
{
    // Static fields.
    public static FileStream FileWriter { get; private set; }
    public static readonly string FilePath;


    // Static constructors.
    static Logger()
    {
        try
        {
            DateTime Time = DateTime.Now;

            string DirectoryPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}" +
                $"{Path.DirectorySeparatorChar}GH Logs";
            Directory.CreateDirectory(DirectoryPath);

            int LogNumber = 0;
            do
            {
                LogNumber++;
                FilePath = $"{DirectoryPath}{Path.DirectorySeparatorChar}" +
                $"GH{GetFormattedDate(Time)}{(LogNumber > 1 ? $"({LogNumber})" : null)}.log";
            }
            while (File.Exists(FilePath));

            FileWriter = File.Create(FilePath);

            string Ordinal = (LogNumber % 100) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };

            FileWriter.Write(Encoding.UTF8.GetBytes(
                $"Program instance started on {GetFormattedDate(Time)} at {GetFormattedTime(Time)} " +
                $"for the {LogNumber}{Ordinal} time today.\n" +
                $"Log generated in \"{Path.GetDirectoryName(FilePath)}\"\n"));
        }
        catch (Exception e)
        {
            Environment.FailFast($"Failed to initalize logger. {e.Message} | {e.StackTrace}");
        }
    }


    // Methods.
    public static void Initialize() { } // Method usage forces the class to be initialized.

    public static void Info(string message) => Log(LogLevel.Info, message);

    public static void Warning(string message) => Log(LogLevel.Warning, message);

    public static void Error(string message) => Log(LogLevel.Error, message);

    public static void Critical(string message) => Log(LogLevel.CRITICAL, message);

    public static async void Log(LogLevel level, string message)
    {
        StringBuilder FullMessage = new(message.Length + 30);
        DateTime Time = DateTime.Now;

        FullMessage.Append($"\n[{GetFormattedTime(Time)}]");

        if (level == LogLevel.Info) FullMessage.Append(' ');
        else FullMessage.Append($"[{level.ToString()}] ");

        FullMessage.Append(message);

        await FileWriter.WriteAsync(Encoding.UTF8.GetBytes(FullMessage.ToString()));
    }

    public static string GetFormattedTime(DateTime time)
    {
        return $"{(time.Hour < 10 ? 0 : null)}{time.Hour}:" +
            $"{(time.Minute < 10 ? 0 : null)}{time.Minute}:" +
            $"{(time.Second < 10 ? 0 : null)}{time.Second}";
    }
    public static string GetFormattedDate(DateTime date)
    {
        return $"{date.Year}y" +
            $"{(date.Month < 10 ? 0 : null)}{date.Month}m" +
            $"{(date.Day < 10 ? 0 : null)}{date.Day}d";
    }

    public static void Dispose()
    {
        FileWriter.Flush();
        FileWriter.Dispose();
        FileWriter = null;
    }
}