 
using MyGame;
using UnityEngine; 
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
 
    public class DLogger
    { 
        public static DebugMode LogType;
        public static void Log(string log)
        {
            if (LogType == DebugMode.AllLog)
            {
                Debug.Log(log);
            }
        }
        
        public static void Error(string log)
        {
            if (LogType == DebugMode.AllLog || LogType == DebugMode.Error || LogType == DebugMode.WarringOrError)
            {
                Debug.LogError(log);
            }
        }
        
        public static void Warring(string log)
        {
            if (LogType == DebugMode.AllLog || LogType == DebugMode.Warring || LogType == DebugMode.WarringOrError)
            {
                Debug.LogError(log);
            }
        }
    }

}


