using System;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Collections.Generic;

public interface ILogger
{
    void Log(string message);
}

public class TextLogger : ILogger
{
    private readonly string _filePath;

    public TextLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Log(string message)
    {
        using (StreamWriter writer = new StreamWriter(_filePath, true))
        {
            writer.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}

public class JsonLogger : ILogger
{
    private readonly string _filePath;

    public JsonLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Log(string message)
    {
        var logEntry = new
        {
            Timestamp = DateTime.Now,
            Message = message
        };

        var json = JsonConvert.SerializeObject(logEntry, Newtonsoft.Json.Formatting.Indented);
        using (StreamWriter writer = new StreamWriter(_filePath, true))
        {
            writer.WriteLine(json);
        }
    }
}

public class MyLogger
{
    private readonly List<ILogger> _loggers;

    public MyLogger(List<ILogger> loggers)
    {
        _loggers = loggers;
    }

    public void Log(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.Log(message);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var textLogger = new TextLogger("log.txt");
        var jsonLogger = new JsonLogger("log.json");

        var loggers = new List<ILogger> { textLogger, jsonLogger };
        var myLogger = new MyLogger(loggers);

        myLogger.Log("This is a log message.");
        myLogger.Log("This is another log message.");

        Console.WriteLine("Logs have been written.");
    }
}