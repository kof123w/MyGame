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
        FrameInputBuffer frameInputBuffer = new FrameInputBuffer();

        internal void InitEvent()
        {
            this.Subscribe<float,float>(SignalEvent.SignalControl_MoveSignal,SignalControl_MoveSignal);
        }

        internal void InputSample()
        {
            
        }
        

        private void SignalControl_MoveSignal(float u,float r)
        {
            dup = u;
            dright = r;
        }
    }
}