using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace GardenHoseEngine.Logging;


public static class Logger
{
    // Internal static fields.
    internal static string LogPath { get; private set; }


    // Private static fields.
    private static StreamWriter s_fileWriter;
    private static BlockingCollection<string> s_messages = new(new ConcurrentQueue<string>());
    private static CancellationTokenSource s_cancellationTokenSource = new();
    private static Task s_loggerTask;

    // Static methods.
    public static void Initialize(string logDirectory)
    {
        if  (s_loggerTask != null)
        {
            throw new InvalidOperationException("Logger already initialized!");
        }

        if (logDirectory == null)
        {
            throw new ArgumentNullException(nameof(logDirectory));
        }
        if (!Path.IsPathFullyQualified(logDirectory))
        {
            throw new ArgumentException($"fileDirectory for logging is not fully qualified: \"{logDirectory}\"");
        }

        DateTime Time = DateTime.Now;
        Directory.CreateDirectory(logDirectory);

        LogPath = Path.Combine(logDirectory, "latest.log");
        if (File.Exists(LogPath))
        {
            ArchiveOldLog(Path.Combine(logDirectory, "old"), LogPath);
        }

        s_fileWriter = new(File.Create(LogPath), Encoding.UTF8);
        s_fileWriter.WriteLine($"Program instance started on {GetFormattedDate(Time)} at {GetFormattedTime(Time)} " +
            $"Log generated in \"{logDirectory}\"");

        s_loggerTask = Task.Factory.StartNew(LogWriterAction, new CancellationTokenSource().Token,
            TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public static void Stop()
    {
        if (s_loggerTask == null)
        {
            throw new InvalidOperationException("Cannot stop logger since it is not initialized yet.");
        }

        s_cancellationTokenSource.Cancel();
        Info("Logger stopped, goodbye world!");
        s_loggerTask.Wait();

        s_fileWriter.Flush();
        s_fileWriter.Dispose();
    }

    public static void Info(string message) => Log(LogLevel.Info, message);

    public static void Warning(string message) => Log(LogLevel.Warning, message);

    public static void Error(string message) => Log(LogLevel.Error, message);

    public static void Critical(string message) => Log(LogLevel.CRITICAL, message);

    public static void Log(LogLevel level, string message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(message);
        }

        StringBuilder FullMessage = new(message.Length + 30);

        FullMessage.Append($"{Environment.NewLine}[{GetFormattedTime(DateTime.Now)}]");
        FullMessage.Append(level == LogLevel.Info ? ' ' : $"[{level}] ");
        FullMessage.Append(message);

        s_messages.Add(FullMessage.ToString());
    }


    // Private static methods.
    private static void ArchiveOldLog(string oldLogDirectory, string oldLogPath)
    {
        Directory.CreateDirectory(oldLogDirectory);
        DateTime LogDate = File.GetLastWriteTime(oldLogPath);

        string ArchivePath;
        int LogNumber = 0;
        do
        {
            LogNumber++;
            ArchivePath = Path.Combine(oldLogDirectory,
                $"{GetFormattedDate(LogDate)}{(LogNumber > 1 ? $" ({LogNumber})" : null)}.zip");
        }
        while (File.Exists(ArchivePath));

        byte[] LogData = File.ReadAllBytes(oldLogPath);

        using ZipArchive Archive = new(File.Open(ArchivePath, FileMode.Create), ZipArchiveMode.Create);
        using Stream EntryStream = Archive.CreateEntry($"{GetFormattedDate(LogDate)}.log",
            CompressionLevel.SmallestSize).Open();
        EntryStream.Write(LogData);
    }

    private static string GetFormattedTime(DateTime time)
    {
        return $"{(time.Hour < 10 ? 0 : null)}{time.Hour}:" +
            $"{(time.Minute < 10 ? 0 : null)}{time.Minute}:" +
            $"{(time.Second < 10 ? 0 : null)}{time.Second}";
    }
    private static string GetFormattedDate(DateTime date)
    {
        return $"{date.Year}y" +
            $"{(date.Month < 10 ? 0 : null)}{date.Month}m" +
            $"{(date.Day < 10 ? 0 : null)}{date.Day}d";
    }

    private static void LogWriterAction()
    {
        try
        {
            while (!s_cancellationTokenSource.IsCancellationRequested)
            {
                s_fileWriter.WriteLine(s_messages.Take(s_cancellationTokenSource.Token));
            }
        }
        catch (OperationCanceledException)
        {
            while (s_messages.TryTake(out string? message))
            {
                s_fileWriter.WriteLine(message);
            }
        }
    }
}