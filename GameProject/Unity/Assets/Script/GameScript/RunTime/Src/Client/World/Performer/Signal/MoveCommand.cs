using UnityEngine;

namespace MyGame
{
    public class MoveCommand : SignalCommand
    {
        private Vector2 input;
        private bool isInputKey;
        
        public override void Execute(IAction action)
        {
            //action.Move();
        }
    }
}