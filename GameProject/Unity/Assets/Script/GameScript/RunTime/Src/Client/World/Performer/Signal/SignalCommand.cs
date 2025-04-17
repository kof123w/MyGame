namespace MyGame
{
    public abstract class SignalCommand
    {
        public virtual void Execute(IAction action)
        {
            // to execute ..
        }
    }
}