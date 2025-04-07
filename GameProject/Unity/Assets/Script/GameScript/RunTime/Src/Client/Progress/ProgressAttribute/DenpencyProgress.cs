using System;
using System.Collections.Generic;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyProgress : Attribute
    {
        public Type[] Dependencies;

        public DependencyProgress(params Type[] dependencies)
        {
            Dependencies = dependencies;
        }
    }
}