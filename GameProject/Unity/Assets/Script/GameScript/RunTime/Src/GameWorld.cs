using System;

namespace MyGame
{
    public sealed class GameWorld : Singleton<GameWorld>
    {
        public EntityManager EntityManager { get; private set; } 
        
        public WindowEntity WindowEntityObject { get; private set; }
        public UIRootComponent UIRootComponentObject { get; private set; }

        //初始化函数
        public void Init()
        {
            DLogger.Log("=====Init Game World!!!!====="); 
            EntityManager = new EntityManager();

            WindowEntityObject = Instantiate<WindowEntity>();
            UIRootComponentObject = WindowEntityObject.AddComponent<UIRootComponent>();
            
            //创建下主界面先
            //this.Push<Type>(UIRootComponentObject.EventOpenWindowEventID,typeof(MainUIComponent));
        }

        public T Instantiate<T>() where T : Entity,new()
        {
            return EntityManager.InstantiateEntity<T>();
        }

        public bool DestroyEntity(Entity entity)
        {
            
            return false;
        }

        //所有实体的生命周期都从这里获取
        public void Update()
        {
            EntityManager.EntityUpdate();
        }  
    }  
}
