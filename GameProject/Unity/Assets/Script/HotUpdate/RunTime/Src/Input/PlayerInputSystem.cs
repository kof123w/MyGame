using System;
using System.Collections.Generic;
using Config;
using DebugTool;
using EventSystem;
using SingleTool;
using UnityEngine;

namespace MyGame
{
    //包含键盘输入手柄输入等等
    public class PlayerInputSystem : Singleton<PlayerInputSystem>
    {
        private List<DefInputConfig> defInputConfigs;
        private Dictionary<int, KeyCode> cmdKeyMap = new();
        private Vector3 previousMousePosition;
        private DefInputConfig runningConfig = null;
        private KeyCode runningKeyCode = KeyCode.None;
        public void Init()
        {
            DLogger.Log("==============>Init Input Module");
            defInputConfigs = DefInputConfigManager.Instance.GetInputConfigs();
            if (defInputConfigs != null)
            {
                for (int i = 0; i < defInputConfigs.Count; i++)
                {
                    foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                    {
                        if (keyCode.ToString().Equals(defInputConfigs[i].Keys))
                        { 
                            if (defInputConfigs[i].ID == (int)InputKey.Run)
                            {
                                runningConfig = defInputConfigs[i];
                                runningKeyCode = keyCode;
                                continue;
                            }
                            
                            cmdKeyMap.Add(defInputConfigs[i].ID, keyCode);
                        }
                    }
                }
            }

            this.Subscribe<InputKey>(InputEvent.KeyboardHold, RevInput);
        }

        //讲键盘输入转成signal
        private int Dup = 0;
        private int Ddown = 0;
        private int Dright = 0;
        private int Dleft = 0;
        private float UpVelocity = 0.0f;
        private float RightVelocity = 0.0f; 
        private bool isRunning = false;  


        //接收输入
        public void RevInput()
        {
            //收集前先清理一下
            ClearInput();
            //这里收集这一帧数的输入
            if (defInputConfigs != null)
            {
                if (Input.anyKey)
                {
                    for (int i = 0; i < defInputConfigs.Count; i++)
                    {
                        if (cmdKeyMap.ContainsKey(defInputConfigs[i].ID))
                        {
                            var keyCode = cmdKeyMap[defInputConfigs[i].ID];
                            if (Input.GetKey(keyCode))
                            {
                                RevInput((InputKey)defInputConfigs[i].ID);
                            }
                        }
                    } 
                }
                
                //奔跑键是按压型的 
                isRunning = Input.GetKey(runningKeyCode);
            }

            int dup = Dup - Ddown;
            int dright = Dright - Dleft;      
            /*UpVelocity = Mathf.SmoothDamp(UpVelocity, dup, ref UpVelocity, 0.05f);
            RightVelocity = Mathf.SmoothDamp(RightVelocity, dright, ref RightVelocity, 0.05f);

            if (!Mathf.Approximately(UpVelocity,0.0001f) || !Mathf.Approximately(RightVelocity,0.0001f))
            {
                GameEvent.Push(SignalEvent.SignalControl_MoveSignal, UpVelocity, -RightVelocity);
            } */
            
            if (dup!=0 || dright!=0)
            {
                GameEvent.Push(InputSignal.InputSignal_MoveSignal, (float)dup, (float)-dright);
            }
            
            // 检测鼠标左键按下时的滑动
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 delta = currentMousePosition - previousMousePosition;
            GameEvent.Push(InputSignal.InputSignal_CameraMoveSignal, delta);
            previousMousePosition = Input.mousePosition;
        }

        private void RevInput(InputKey inputKey)
        {
            if (inputKey == InputKey.MoveForward)
            {
                Dup = isRunning ? 2 :  1;
            }

            else if (inputKey == InputKey.BackForward)
            {
                Ddown = isRunning ? 2 :  1;
            }

            else if (inputKey == InputKey.Right)
            {
                Dright = isRunning ? 2 :  1;
            }

            else if (inputKey == InputKey.Left)
            {
                Dleft = isRunning ? 2 :  1;
            }
        }

        private void ClearInput()
        {
            Dup = 0;
            Ddown = 0;
            Dright = 0;
            Dleft = 0;
        }
        
        private void SquareToCircle(ref float up,ref float right)
        {
            up *= Mathf.Sqrt(1 - (right * right) / 2);
            right *= Mathf.Sqrt(1 - (up * up) / 2); 
        }

        private Vector2 SquareToCircle(Vector2 input)
        {
            Vector2 output = new Vector2
            (
                input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2),
                input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2)
            );
            return output;
        }
    }
}