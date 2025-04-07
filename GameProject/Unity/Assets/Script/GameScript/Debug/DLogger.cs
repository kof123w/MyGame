 
using System;
using System.Diagnostics; 
using Debug = UnityEngine.Debug;

namespace MyGame
{
    public enum DebugMode
    {
        None,
        AllLog,
        Warring,
        WarringOrError,
        Error,
    }
 
    public static class DLogger 
    { 
        public static DebugMode LogType;  

        [Conditional("DEBUG_LOG")]
        public static void Log(string log)
        {
            if (LogType == DebugMode.AllLog)
            {
                Debug.Log(log);
            }
        }
        
        [Conditional("DEBUG_LOG")]
        public static void Error(string log)
        {
            if (LogType == DebugMode.AllLog || LogType == DebugMode.Error || LogType == DebugMode.WarringOrError)
            {
                Debug.LogError(log);
            }
        }
        
        [Conditional("DEBUG_LOG")]
        public static void Error(Exception log)
        {
            if (LogType == DebugMode.AllLog || LogType == DebugMode.Error || LogType == DebugMode.WarringOrError)
            {
                Debug.LogError(log);
            }
        }
        
        [Conditional("DEBUG_LOG")]
        public static void Warring(string log)
        {
            if (LogType == DebugMode.AllLog || LogType == DebugMode.Warring || LogType == DebugMode.WarringOrError)
            {
                Debug.LogError(log);
            }
        }
    }

}


