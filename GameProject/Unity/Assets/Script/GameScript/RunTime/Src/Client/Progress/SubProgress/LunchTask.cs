using Cysharp.Threading.Tasks;

namespace MyGame
{
    [RootProgress(true)]
    public class LunchTask : ITask   //启动流程
    {
        public async UniTaskVoid Run()
        { 
            await UIManager.Show<TitleUI>(); 
        } 
    }
}