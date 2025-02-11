
using UnityEngine;

namespace MyGame
{
    public sealed class GameWorld : Singleton<GameWorld>
    {
        private EntityManager m_entityManager;
        public EntityManager EntityManager => m_entityManager;

        //初始化函数
        public void Init()
        {
            DLogger.Log("=====Init Game World!!!!====="); 
            m_entityManager = new EntityManager();
             
        }

        //所有实体的生命周期都从这里获取
        public void Update()
        {
            EntityManager.EntityUpdate();
        } 
    }  
}
