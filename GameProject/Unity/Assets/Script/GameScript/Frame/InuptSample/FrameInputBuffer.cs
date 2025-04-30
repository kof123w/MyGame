using System.Collections.Generic;
using NUnit.Framework;

namespace MyGame
{
    internal class FrameInputBuffer
    {
        List<FrameInput> frameInputs = new List<FrameInput>();
        
        public void AddInput(FrameInput frameInput)
        {
            frameInputs.Add(frameInput);
        }

        //把输入进行打包
        public CSFrameSample PackPlayerInput()
        {
            CSFrameSample csFrameSample = new CSFrameSample();

            for (int i = 0; i < frameInputs.Count; i++) {
                global::FrameInput frameInput = new global::FrameInput(); 
                var sample = frameInputs[i]; 
                if (sample.GetCount() > 0)
                {
                    var inputCommand = sample.GetInputCommand(sample.GetCount() - 1);
                    PlayerInput playerInput = new PlayerInput
                    {
                        Dright = inputCommand.Dright.RawValue,
                        Dup = inputCommand.Dup.RawValue
                    };
                    frameInput.PlayerInput = playerInput;
                } 
                csFrameSample.FrameInput = frameInput;
            }
            ClearInputs();
            return csFrameSample;
        }

        private void ClearInputs()
        {
            ObjectPool.Pool.Free(frameInputs);
        }
    }
}