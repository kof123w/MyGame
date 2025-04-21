using System.Collections.Generic; 
using DebugTool; 
using ObjectPool;
using SingleTool;
using UnityEngine; 

namespace MyGame
{
    public sealed class GameWorld : Singleton<GameWorld>
    { 
        private Transform gameWorldTrans;
        private GameObject gameWorldGO;
        
        //场景相关
        private SceneType currentScene = SceneType.None;
        private SceneType previousScene = SceneType.None;
        private readonly List<BaseSubScene> scenes = new();
        private BaseSubScene currentSubScene;
        private BaseSubScene previousSubScene;
        
        //场景角色
        private List<BasePerformer> performers = new();
        private PlayerCharacter playerCharacter;
        private Camera mainCamera;   //场景相机
         
        public void Init()
        {
            DLogger.Log("==============>Init world system!");
            //关掉物理系统
            Physics.autoSyncTransforms = false;  //射线检测关闭
            Physics.autoSimulation = false;
            
            DLogger.Log("==============>Init scene Object");
            mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            gameWorldGO = NodePool.MallocEmptyNode();
            gameWorldTrans = gameWorldGO.transform;
        }
        
        public void Tick()
        { 
           // todo ..
        }

        public void LateTick()
        {
            // todo ..
        }

        public void FixedTick()
        {
          
        }

        public static Transform GetGameWorldTransform()
        {
            return Instance.gameWorldTrans;
        }

        public static void SetObjectToGameWorld(Transform trans,Vector3 position)
        { 
            trans.position = position;
        }  

        public void AddSubScene(BaseSubScene scene)
        {
            scenes.Add(scene);
        }

        public void ChangeScene(SceneType sceneType,out BaseSubScene previous,out BaseSubScene current)
        {
            previousScene = currentScene;
            currentScene = sceneType;
            previous = null;
            current = null;
            if (previousScene != currentScene)
            {
                DLogger.Log($"==============>Change SceneManager to {currentScene}");  
                for (int i = 0; i < scenes.Count; i++) {
                    var scene = scenes[i];
                    if (scene.GetSceneType() == currentScene)
                    { 
                        currentSubScene = scene; 
                        current = currentSubScene;
                    }

                    if (scene.GetSceneType() == previousScene)
                    { 
                        previousSubScene = scene;
                        previous = previousSubScene;
                    } 
                }
            }  
        } 

        //创建玩家角色
        public void CreatePlayerRole(int id)
        {
            if (playerCharacter != null)
            {
                if (playerCharacter.IsLoaded)
                {
                    playerCharacter.UnLoadResources().Forget(); 
                }
            }
            else
            {
                playerCharacter = Pool.Malloc<PlayerCharacter>();
            }

            playerCharacter.SetConfigID(id); 
            playerCharacter.SetGameObjectName("PlayerRole"); 
        }

        public Camera GetMainCamera()
        {
            return mainCamera;
        }

        public PlayerCharacter GetPlayerCharacter()
        {
            return playerCharacter;
        }

        /*public static BEPUphysicsSpace GetPhysicsSpace()
        {
            return Instance.bEpUPhysicsSpace;
        }*/
    }  
}
