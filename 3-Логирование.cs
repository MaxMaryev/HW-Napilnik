using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        PathFinder log1 = new PathFinder(new FileLogWritter());
        PathFinder log2 = new PathFinder(new ConsoleLogWritter());
        PathFinder log3 = new PathFinder(new SecureConsoleLogWritter(new FileLogWritter()));
        PathFinder log4 = new PathFinder(new SecureConsoleLogWritter(new ConsoleLogWritter()));
        PathFinder log5 = new PathFinder(new ConsoleLogWritter(), new SecureConsoleLogWritter(new FileLogWritter()));

        log5.Find();
    }
}

interface ILogger
{
    public ILogger Logger { get; }

    public void WriteError(string message);
}

abstract class LogWritter : ILogger
{
    public ILogger Logger { get; }

    public LogWritter() { }
    public LogWritter(ILogger logger)
    {
        if (logger == null)
            throw new NullReferenceException(nameof(logger));

        Logger = logger;
    }

    public abstract void WriteError(string message);
}

class PathFinder
{
    private readonly List<ILogger> _loggers = new List<ILogger>();

    public PathFinder(params ILogger[] loggers)
    {
        if (loggers == null)
            throw new NullReferenceException(nameof(loggers));

        foreach (var logger in loggers)
        {
            _loggers.Add(logger);
        }
    }

    public void Find()
    {
        foreach (var logger in _loggers)
        {
            logger.WriteError("some shit");
        }
    }
}

class ConsoleLogWritter : LogWritter
{
    public ConsoleLogWritter() : base() { }
    public ConsoleLogWritter(ILogger logger) : base(logger) { }

    public override void WriteError(string message)
    {
        Console.WriteLine(message);
    }
}

class FileLogWritter : LogWritter
{
    public FileLogWritter() : base() { }
    public FileLogWritter(ILogger logger) : base(logger) { }

    public override void WriteError(string message)
    {
        File.WriteAllText("log.txt", message);
    }
}

class SecureConsoleLogWritter : LogWritter
{
    public SecureConsoleLogWritter() : base() { }
    public SecureConsoleLogWritter(ILogger logger) : base(logger) { }

    public override void WriteError(string message)
    {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            Logger.WriteError(message);
    }
}

class File
{
    public static void WriteAllText(string log, string message)
    {
        if (log == null)
            throw new NullReferenceException(nameof(log));

        Console.WriteLine(log);
        Console.WriteLine(message);
    }
}
