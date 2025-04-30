using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class ConsoleToScreen : MonoBehaviour
    {
        private const int MaxLines = 50;
        private const int MaxLineLength = 100;
        private string logStr = "";

        private static readonly object locker = new object();
        private readonly List<string> lines = new List<string>();

        public int fontSize = 15;

        void OnEnable() { Application.logMessageReceivedThreaded += Log; }
        void OnDisable() { Application.logMessageReceivedThreaded -= Log; }

        private void Log(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Error && type != LogType.Exception && type !=LogType.Assert)
            {
                return;
            }

            foreach (var line in logString.Split('\n'))
            {
                if (line.Length <= MaxLineLength && line.Length > 5)
                {
                    lock (locker)
                    {
                        lines.Add(line);
                    }

                    continue;
                }
                var lineCount = line.Length / MaxLineLength + 1;
                for (int i = 0; i < lineCount; i++)
                {
                    if ((i + 1) * MaxLineLength <= line.Length)
                    {
                        lock (locker)
                        {
                            lines.Add(line.Substring(i * MaxLineLength, MaxLineLength));
                        }
                    }
                    else
                    {
                        lock (locker)
                        {
                            lines.Add(line.Substring(i * MaxLineLength, line.Length - i * MaxLineLength));
                        }
                    }
                }
            }
            
            foreach (var line in stackTrace.Split('\n'))
            {
                if (line.Length <= MaxLineLength)
                {
                    lock (locker)
                    {
                        lines.Add(line);
                    }

                    continue;
                }
                var lineCount = line.Length / MaxLineLength + 1;
                for (int i = 0; i < lineCount; i++)
                {
                    if ((i + 1) * MaxLineLength <= line.Length)
                    {
                        lock (locker)
                        {
                            lines.Add(line.Substring(i * MaxLineLength, MaxLineLength));
                        }
                    }
                    else
                    {
                        lock (locker)
                        {
                            lines.Add(line.Substring(i * MaxLineLength, line.Length - i * MaxLineLength));
                        }
                    }
                }
            }
            
            lock (locker)
            {
                if (lines.Count > MaxLines)
                {
                    lines.RemoveRange(0, lines.Count - MaxLines); 
                }
            }  
           
            logStr = string.Join("\n", lines);
        }

        void OnGUI()
        {
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
                new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
            GUI.Label(new Rect(10, 10, 800, 370), logStr, new GUIStyle() { fontSize = Math.Max(10, fontSize) });
        }
    }
}