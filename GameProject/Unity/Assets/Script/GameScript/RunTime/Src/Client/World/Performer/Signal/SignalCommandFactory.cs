using System;

namespace MyGame
{
    public class SignalCommandFactory
    {
        public static SignalCommand Create(SignalEnum signal){
            switch(signal)
            {
                case SignalEnum.MoveSignal: return new MoveCommand(); 
           
                default:
                    throw new Exception("no signal specified!!!");
            }
        } 
    }
}