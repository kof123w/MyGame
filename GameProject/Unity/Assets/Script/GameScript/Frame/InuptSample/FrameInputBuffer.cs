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

        private void ClearInputs()
        {
            frameInputs.Clear();
        }
    }
}