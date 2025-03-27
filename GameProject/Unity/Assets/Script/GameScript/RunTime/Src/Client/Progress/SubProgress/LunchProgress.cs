using System;
using MyGame;

namespace MyGameProgress
{
    [RootProgress(true)]
    public class LunchProgress : IProgress   //启动流程
    {
        public void Run()
        {
            UIManager.ShowWindow<TitleUI>();
            
            
        }

        public void Check()
        {
            //todo..
        }
    }
}