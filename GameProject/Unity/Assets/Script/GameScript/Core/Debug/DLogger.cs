 
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DebugTool
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
        //private static string logFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");
        private static readonly string  LOGFilePath = $"E:/UnityWorkSpace/Log/game_log.txt";
        
        [Conditional("DEBUG_LOG")]
        public static void Log(string log)
        {
            if (LogType == DebugMode.AllLog)
            {
                string logMessage = $"[{DateTime.Now}] {log}\n"; 
                // 写入文件
                File.AppendAllText(LOGFilePath, logMessage);
                Debug.Log(log);
            }
        }
        
        [Conditional("DEBUG_LOG")]
        public static void Error(string log)
        {
            if (LogType == DebugMode.AllLog || LogType == DebugMode.Error || LogType == DebugMode.WarringOrError)
            {
                string logMessage = $"[{DateTime.Now}] {log}\n"; 
                // 写入文件
                File.AppendAllText(LOGFilePath, logMessage);
                Debug.LogError(log);
            }
        }
        
        [Conditional("DEBUG_LOG")]
        public static void Error(Exception log)
        {
            if (LogType == DebugMode.AllLog || LogType == DebugMode.Error || LogType == DebugMode.WarringOrError)
            {
                string logMessage = $"[{DateTime.Now}] {log}\n"; 
                // 写入文件
                File.AppendAllText(LOGFilePath, logMessage);
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


