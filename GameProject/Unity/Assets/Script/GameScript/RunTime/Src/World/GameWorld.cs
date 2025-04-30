using System.Collections.Generic; 
using DebugTool;
using EventSystem;
using ObjectPool;
using SingleTool;
using UnityEngine; 

namespace MyGame
{
    public sealed class GameWorld : Singleton<GameWorld>
    { 
        private Transform worldTrans;
        private GameObject gameWorldGo;
        
        //场景相关
        private SceneType currentScene = SceneType.None;
        private SceneType previousScene = SceneType.None;
        private readonly List<BaseSubScene> scenes = new();
        private BaseSubScene currentSubScene;
        private BaseSubScene previousSubScene;
        
        //场景角色
        private readonly Dictionary<long,PerformerVisual> performers = new();
        private Camera mainCamera;   //场景相机
         
        public void Init()
        {
            DLogger.Log("==============>Init world system!"); 
            //关掉物理系统
            Physics.autoSyncTransforms = false;  //射线检测关闭
            Physics.autoSimulation = false;
            mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            gameWorldGo = NodePool.MallocEmptyNode();
            worldTrans = gameWorldGo.transform;
            
            this.Subscribe<long,Vector3,Quaternion>(VisualSignal.VisualSignal_SetVisualPosition,SetVisualPosition);
        }

        private void SetVisualPosition(long roleId,Vector3 position,Quaternion rotation)
        {
            if (performers.TryGetValue(roleId,out var performer))
            {
                performer.SetWorldPos(position);
                performer.SetWorldRot(rotation);
            }
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

        public static Transform GetWorldTrans()
        {
            return Instance.worldTrans;
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

        public RoleVisual CreateRole(long roleId,int id)
        {
            RoleVisual visual = Pool.Malloc<RoleVisual>();
            visual.SetConfigID(id);
            visual.SetGameObjectName("PlayerRoleVisual");
            performers.Add(roleId,visual);
            return visual;
        } 
       

        public Camera GetMainCamera()
        {
            return mainCamera;
        } 
    }  
}
