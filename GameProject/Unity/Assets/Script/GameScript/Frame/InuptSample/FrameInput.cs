using System.Collections.Generic;
using FixMath.NET;
using NUnit.Framework;

namespace MyGame
{
    public class FrameInput : IMemoryPool
    {
        public Fix64 SubFrameTime { get; set; }
        private List<InputCommand> inputCommands = new List<InputCommand>();

        public void AddInputCommand(InputCommand inputCommand)
        {
            inputCommands.Add(inputCommand);
        }

        public int GetCount()
        {
            return inputCommands.Count;
        }

        public InputCommand GetInputCommand(int index)
        {
            return inputCommands[index];
        }

        public void OnDestroy()
        {
            inputCommands.Clear();
        }
    }
}