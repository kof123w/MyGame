using System;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RootProgress : Attribute
    {
        public bool MIsRoot;
        public RootProgress(bool value)
        {
            MIsRoot = value;
        }
    }
}