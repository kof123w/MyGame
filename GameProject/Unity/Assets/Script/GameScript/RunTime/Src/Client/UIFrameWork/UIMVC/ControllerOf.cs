using System;
using UnityEngine;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ControllerOf : Attribute
    {
         public Type ModelType;

         public ControllerOf(Type modelType)
         {
             ModelType = modelType;
         }
    }

}