using System;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProgressLoopCheck : Attribute
    {
        public bool MIsLoopCheck;

        public ProgressLoopCheck(bool isLoopCheck)
        {
            MIsLoopCheck = isLoopCheck;
        }
    }
}