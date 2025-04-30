using System.Collections.Generic;
using BEPUphysics;
using BEPUphysics.Entities;
using EventSystem;
using FixedMath;
using Google.Protobuf.Collections;
using SingleTool;

namespace MyGame
{
    public class FrameContext : Singleton<FrameContext>
    {
        private FrameCore frameCore;
        private FrameWorld frameWorld; 
        
        public static FrameContext Context => Instance;

        //玩家列表
        List<PlayerData> playerDataList = new List<PlayerData>();
        
        public long CtrlRoleID { get; private set; }
        public int SrvRoomID { get; private set; }

        //开启帧同步核心逻辑
        public void InitParam(int randomSeed,int tickParam,long roleParamId,int roomParamId,RepeatedField<PlayerData> playerList,INetworkService network)
        {
            CtrlRoleID = roleParamId;
            SrvRoomID = roomParamId;
            frameCore = new FrameCore
            {
                sRandom = new SRandom(randomSeed),
                tick = tickParam,
                tickTime = 1f / tickParam,
                curTickTime = 0.0f,
                networkService = network, 
                frameInputSample = new FrameInputSample()
            };
            frameCore.frameInputSample.SubscribeEvent(); 
            frameCore.frameBuffer = new FrameBuffer(); 
            frameCore.frameExecutor = new FrameExecutor();
            frameWorld = new FrameWorld();
            frameWorld.InitPhysicsSpace();
            playerDataList.Clear();
            playerDataList.AddRange(playerList);
            this.Subscribe<SCFrameData>(FrameSignal.Signal_FrameSync,frameCore.ReceivedData);
        }

        public void Start()
        {
            frameCore.IsUpdate = true;  
        }
        
        public void FixedTick()
        {
            frameCore?.FixedTick();
            frameWorld?.FixedTick();
        }

        public void Exit()
        {
            this.UnSubscribe(FrameSignal.Signal_FrameSync);
            frameCore.CloseFrameSync();
            frameWorld.Dispose();
            frameCore = null;
            frameWorld = null;
        }

        public int GetSyncFrame()
        {
            return frameCore.GetSyncFrame();
        }

        public BEPUphysicsSpace GetSpace()
        {
            return frameWorld.GetSpace();
        }

        public void AddEntityToWorld(Entity entity)
        {
            frameWorld.AddEntityShape(entity);
        }

        public void RemoveEntityFromWorld(Entity entity)
        {
            frameWorld.RemoveEntityShape(entity);
        }

        public List<PlayerData> GetPlayerDataList()
        {
            return playerDataList;
        }

        public void InitWorldTerrain(int heightmapResolutionParam, float[,] heightsParam,FPVector2 terrainSizeParam,FPQuaternion terrainRotationParam,FPVector3 terrainPositionParam,
            List<FPVector3[]> meshVerticesParam = null,List<FPQuaternion> meshRotationsParam= null,List<int[]> meshTrianglesParam= null,List<FPVector3> meshPositionsParam= null,List<FPVector3> meshScalesParam= null)
        {
            frameWorld.InitFix64Terrain(
                heightmapResolutionParam,
                heightsParam,
                terrainSizeParam,
                terrainRotationParam,
                terrainPositionParam,
                meshVerticesParam,
                meshRotationsParam,
                meshTrianglesParam,
                meshPositionsParam,
                meshScalesParam
                );
        }

        internal Entity GetEntityFromWorld(long roleId)
        {
            return frameWorld.GetEntity(roleId);
        }


        public void CreateLogicRole()
        {
            foreach (var playerData in playerDataList)
            {
                frameWorld.CreateRolePerformer(playerData);
            } 
        }
    }
}