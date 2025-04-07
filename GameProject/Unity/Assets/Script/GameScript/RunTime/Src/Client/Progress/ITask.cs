using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGame
{
    public interface ITask
    {
        public UniTaskVoid Run(); 
    } 
}