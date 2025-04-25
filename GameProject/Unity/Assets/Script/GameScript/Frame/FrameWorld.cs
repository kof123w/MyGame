using System;
using System.Collections.Generic;
using System.Linq;
using BEPUphysics;
using FixedMath;
using FixedMath.Threading;
using MyGame.Map;
using UnityEngine;

namespace MyGame
{  
    internal class FrameWorld : IDisposable
    {
        private BEPUphysicsSpace bEpUPhysicsSpace; 
        
        
        //场景地形
        Fix64Terrain fix64Terrain;

        public void InitFix64Terrain(int heightmapResolutionParam, float[,] heightsParam,FPVector2 terrainSizeParam,FPQuaternion terrainRotationParam,FPVector3 terrainPositionParam,
            List<FPVector3[]> meshVerticesParam,List<FPQuaternion> meshRotationsParam,List<int[]> meshTrianglesParam,List<FPVector3> meshPositionsParam,List<FPVector3> meshScalesParam)
        {
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

        public void InitBepuPhysicsSpace()
        {
            var parallelLooper = new ParallelLooper();
            bEpUPhysicsSpace = new BEPUphysicsSpace(parallelLooper);
            bEpUPhysicsSpace.ForceUpdater.Gravity = new FPVector3(0, -9.81m, 0);
            bEpUPhysicsSpace.TimeStepSettings.TimeStepDuration = Time.fixedDeltaTime;
            
            //强制多线程确定性模式
            bEpUPhysicsSpace.BroadPhase.AllowMultithreading = false;
            bEpUPhysicsSpace.NarrowPhase.AllowMultithreading = false;
            bEpUPhysicsSpace.Solver.AllowMultithreading = false; 
        }

        public void FixedTick()
        {
            bEpUPhysicsSpace.Update();
        }

        public BEPUphysicsSpace GetSpace()
        {
            return bEpUPhysicsSpace;
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