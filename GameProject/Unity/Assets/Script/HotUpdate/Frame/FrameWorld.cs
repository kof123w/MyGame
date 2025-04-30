using System;
using System.Collections.Generic;
using System.Linq;
using BEPUphysics;
using BEPUphysics.Entities;
using DebugTool;
using FixedMath;
using FixedMath.Threading;
using FixMath.NET;
using MyGame.Map;
using ObjectPool;
using UnityEngine;

namespace MyGame
{  
    internal class FrameWorld : IDisposable
    {
        private BEPUphysicsSpace bEpUPhysicsSpace; 
        
        //场景地形
        Fix64Terrain fix64Terrain;
        
        //场景角色
        List<RolePerformer> roleList = new List<RolePerformer>();

        public void InitFix64Terrain(int heightmapResolutionParam, float[,] heightsParam,FPVector2 terrainSizeParam,FPQuaternion terrainRotationParam,FPVector3 terrainPositionParam,
            List<FPVector3[]> meshVerticesParam,List<FPQuaternion> meshRotationsParam,List<int[]> meshTrianglesParam,List<FPVector3> meshPositionsParam,List<FPVector3> meshScalesParam)
        {
            if (fix64Terrain != null)
            {
                fix64Terrain.OnDestroy();
                fix64Terrain = null;
            }

            fix64Terrain = new Fix64Terrain();
            fix64Terrain.Init(
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

        public void InitPhysicsSpace()
        {  
            //var parallelLooper = new ParallelLooper();
            bEpUPhysicsSpace = new BEPUphysicsSpace();
            bEpUPhysicsSpace.ForceUpdater.Gravity = new FPVector3(0, -9.81m, 0);
            bEpUPhysicsSpace.TimeStepSettings.TimeStepDuration = Time.fixedDeltaTime;
            
            //强制多线程确定性模式
            //bEpUPhysicsSpace.BroadPhase.AllowMultithreading = false;
            //bEpUPhysicsSpace.NarrowPhase.AllowMultithreading = false;
            //bEpUPhysicsSpace.Solver.AllowMultithreading = false; 
        }

        public void CreateRolePerformer(PlayerData playerData)
        {
            RolePerformer rolePerformer = Pool.Malloc<RolePerformer>();
            rolePerformer.PlayerRoleID = playerData.PlayerRoleId;
            //临时
            Fix64 x = (float)2 * (rolePerformer.PlayerRoleID - 1);
            Fix64 z = (float)2 * (rolePerformer.PlayerRoleID - 1);
            FPVector3 playerPosition = new FPVector3(x,5,z);
            rolePerformer.SetPosition(playerPosition);
            roleList.Add(rolePerformer);
        }

        public void FixedTick()
        {
            foreach (var rolePerformer in roleList)
            {
                rolePerformer.SyncWorld();  
            }
            
            for (int i = bEpUPhysicsSpace.Entities.Count - 1; i >= 0; i--)
            {
                var entity = bEpUPhysicsSpace.Entities[i];
            } 
            
            bEpUPhysicsSpace.Update();
        }

        public BEPUphysicsSpace GetSpace()
        {
            return bEpUPhysicsSpace;
        }

        public void AddEntityShape(Entity entity)
        {
            bEpUPhysicsSpace.Add(entity);
        }

        public void RemoveEntityShape(Entity entity)
        {
            bEpUPhysicsSpace.Remove(entity);
        }

        public Entity GetEntity(long roleId)
        {
            for (int i = 0; i < roleList.Count; i++) {
                if (roleList[i].PlayerRoleID == roleId)
                {
                    return roleList[i].EntityShape;
                }
            }

            return null;
        }

        public void Dispose()
        { 
             // 停止空间模拟  
             fix64Terrain.OnDestroy();
             fix64Terrain = null;
             // 移除所有实体
            for (int i = bEpUPhysicsSpace.Entities.Count - 1; i >= 0; i--)
            {
                var entity = bEpUPhysicsSpace.Entities[i];
                bEpUPhysicsSpace.Remove(entity);
            } 
 
            bEpUPhysicsSpace.DeactivationManager = null;
            bEpUPhysicsSpace.BroadPhase = null;
            bEpUPhysicsSpace = null;
        }
    }
}