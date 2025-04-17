using System;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RootTask : Attribute
    {
        public bool MIsRoot;
        public RootTask(bool value)
        {
            MIsRoot = value;
        }
    }
}