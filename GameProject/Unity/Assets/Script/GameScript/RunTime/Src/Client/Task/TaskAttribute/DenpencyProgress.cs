using System;
using System.Collections.Generic;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyTask : Attribute
    {
        public Type[] Dependencies;

        public DependencyTask(params Type[] dependencies)
        {
            Dependencies = dependencies;
        }
    }
}