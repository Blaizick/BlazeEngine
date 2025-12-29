using System.Runtime.CompilerServices;

namespace BlazeEngine;

public static class Debug
{
    public static ILogger logger;
    
    public static void Construct(ILogger logger)
    {
        Debug.logger = logger;
    }

    public static void Log(string message)
    {
        logger.Log(message);
    }
    public static void Log(object? obj)
    {
        logger.Log(obj);
    }
    public static void LogWarning(string warning)
    {
        logger.LogWarning(warning);
    }
    public static void LogError(string error)
    {
        logger.LogError(error);
    }
}

public interface ILogger
{
    public void Log(string message);
    public void Log(object? obj);
    public void LogError(string error);
    public void LogWarning(string warning);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
    public void Log(object? obj)
    {
        Console.WriteLine(obj);
    }
    public void LogError(string error)
    {
        Console.WriteLine($"[Error] {error}");
    }
    public void LogWarning(string warning)
    {
        Console.WriteLine($"[Warning] {warning}");
    }
}