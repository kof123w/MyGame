using System.Collections.Generic;
using FixMath.NET;
using NUnit.Framework;

namespace MyGame
{
    public class FrameInput : IMemoryPool
    {
        private int frameIndex;
        private Fix64 subFrameTime;
        
        private List<InputCommand> inputCommands = new List<InputCommand>();
    }
}