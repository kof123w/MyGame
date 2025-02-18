using System;
using UnityEngine;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SystemOfComponent :Attribute
    {
        public Type Type { get; private set; }

        public SystemOfComponent(Type type)
        {
            Type = type;
        }
    }
}
