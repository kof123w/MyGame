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
                frameInput.SubFrameTime = sample.SubFrameTime.RawValue;
                for (int j = 0; j < sample.GetCount();j++)
                {
                    var inputCommand = sample.GetInputCommand(j);
                    PlayerInput playerInput = new PlayerInput
                    {
                        Dright = inputCommand.Dright.RawValue,
                        Dup = inputCommand.Dup.RawValue
                    };
                    frameInput.PlayerInputList.Add(playerInput);
                }

                csFrameSample.FrameInputList.Add(frameInput);
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