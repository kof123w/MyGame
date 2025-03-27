using System;
using System.Collections.Generic;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyProgress : Attribute
    {
        private Type[] _dependencies;

        public DependencyProgress(params Type[] dependencies)
        {
            _dependencies = dependencies;
        }
    }
}