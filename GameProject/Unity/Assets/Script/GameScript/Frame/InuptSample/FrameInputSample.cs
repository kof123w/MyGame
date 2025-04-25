using EventSystem;
using FixMath.NET;
using UnityEngine;

namespace MyGame
{
    public class FrameInputSample
    {
        //获得到的输入字段
        private float dup = 0.0f;
        private float dright = 0.0f;
        private bool needSample = false;
        private FrameInputBuffer frameInputBuffer = new FrameInputBuffer();

        internal void SubscribeEvent()
        {
            this.Subscribe<float,float>(SignalEvent.SignalControl_MoveSignal,SignalControl_MoveSignal);
        }

        internal void UnSubscribeEvent()
        {
            this.UnSubscribe(SignalEvent.SignalControl_MoveSignal);
        }

        internal void InputSample(Fix64 currTickTime,Fix64 tickTime)
        {
            if (needSample)
            {
                FrameInput frameInput = ObjectPool.Pool.Malloc<FrameInput>();
                frameInput.SubFrameTime = currTickTime / tickTime;
                frameInput.AddInputCommand(new InputCommand(dup, dright)); 
                frameInputBuffer.AddInput(frameInput);
                needSample = false;
            }
        }

        internal CSFrameSample PackInput()
        {
            return frameInputBuffer.PackPlayerInput();
        }


        private void SignalControl_MoveSignal(float u,float r)
        {
            dup = u;
            dright = r;
            needSample = true;
        }
    }
}