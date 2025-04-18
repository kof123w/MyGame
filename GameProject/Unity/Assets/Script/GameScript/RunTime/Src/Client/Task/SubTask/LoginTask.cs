using Cysharp.Threading.Tasks;

namespace MyGame
{
    public class LoginTask : ITask
    {
        public async UniTaskVoid Run()
        { 
            await UIManager.Show<MatchUI>();
            UIManager.Close<TitleUI>();
        }
    }
}