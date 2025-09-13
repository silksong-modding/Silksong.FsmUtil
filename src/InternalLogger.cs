namespace Silksong.FsmUtil;

internal static class InternalLogger
{
    internal static BepInEx.Logging.ManualLogSource? Logger = null;

    internal static void LogFine(string message)
    {
        Logger?.LogDebug(message);
        //UnityEngine.Debug.Log(message);
    }
    internal static void LogDebug(string message)
    {
        Logger?.LogInfo(message);
        UnityEngine.Debug.Log(message);
    }
    internal static void Log(string message)
    {
        Logger?.LogMessage(message);
        UnityEngine.Debug.Log(message);
    }
    internal static void LogWarn(string message)
    {
        Logger?.LogWarning(message);
        UnityEngine.Debug.LogWarning(message);
    }
    internal static void LogError(string message)
    {
        Logger?.LogError(message);
        UnityEngine.Debug.LogError(message);
    }
    internal static void LogFatal(string message)
    {
        Logger?.LogFatal(message);
        UnityEngine.Debug.LogError(message);
    }
}