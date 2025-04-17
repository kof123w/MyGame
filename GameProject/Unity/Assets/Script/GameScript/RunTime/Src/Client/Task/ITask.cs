using Cysharp.Threading.Tasks;

namespace MyGame
{
    public interface ITask
    {
        public UniTaskVoid Run(); 
    } 
}