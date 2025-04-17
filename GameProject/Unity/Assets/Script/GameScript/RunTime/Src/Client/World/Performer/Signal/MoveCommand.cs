using UnityEngine;

namespace MyGame
{
    public class MoveCommand : SignalCommand
    { 
        public override void Execute(IAction action)
        {
            //action.Move();
        }
    }
}