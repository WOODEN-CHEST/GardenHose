using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace GardenHoseEngine.Logging;


public sealed class Logger : IDisposable
{
    // Internal fields.
    internal readonly string LogPath;


    // Private fields.
    private readonly StreamWriter _fileWriter;
    private BlockingCollection<string> _messages = new(new ConcurrentQueue<string>());
    private CancellationTokenSource _cancellationTokenSource = new();
    private Task _loggerTask;


    // Constructors.
    internal Logger(string logDirectory)
    {
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

        _fileWriter = new(File.Create(LogPath), Encoding.UTF8);
        _fileWriter.WriteLine($"Program instance started on {GetFormattedDate(Time)} at {GetFormattedTime(Time)} " +
            $"Log generated in \"{logDirectory}\"");

        _loggerTask = Task.Factory.StartNew(WriteLog, new CancellationTokenSource().Token, 
            TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }


    // Methods.
    public void Info(string message) => Log(LogLevel.Info, message);

    public void Warning(string message) => Log(LogLevel.Warning, message);

    public void Error(string message) => Log(LogLevel.Error, message);

    public  void Critical(string message) => Log(LogLevel.CRITICAL, message);

    public void Log(LogLevel level, string message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(message);
        }

        StringBuilder FullMessage = new(message.Length + 30);

        FullMessage.Append($"{Environment.NewLine}[{GetFormattedTime(DateTime.Now)}]");
        FullMessage.Append(level == LogLevel.Info ? ' ' : $"[{level}] ");
        FullMessage.Append(message);

        _messages.Add(FullMessage.ToString());
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

    private void WriteLog()
    {
        try
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                _fileWriter.WriteLine(_messages.Take(_cancellationTokenSource.Token));
            }
        }
        catch (OperationCanceledException)
        {
            while (_messages.TryTake(out string? message))
            {
                _fileWriter.WriteLine(message);
            }
        }
    }


    // Inherited methods.
    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        Info("Logger stopped, goodbye world!");
        _loggerTask.Wait();

        _fileWriter.Flush();
        _fileWriter.Dispose();
    }
}