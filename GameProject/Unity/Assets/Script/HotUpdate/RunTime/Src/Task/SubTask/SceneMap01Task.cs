using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FixedMath;
using UnityEngine; 

namespace MyGame
{ 
    public class SceneMap01Task : ITask
    {
        public async UniTaskVoid Run()
        {
            await UIManager.Show<LoadUI>();
            GameWorld.Instance.ChangeScene(SceneType.Map01,out var previous,out var current);
            if (previous != null)
            {
                previous.UnloadScene().Forget();
            }
            LoadTaskSort loadTaskSort = new LoadTaskSort();
            //加载场景
            loadTaskSort.AddLoadTask(current.LoadScene(),current);
            //处理需要创建的玩家
            var playerDataList = FrameContext.Context.GetPlayerDataList();
            for (int i = 0; i < playerDataList.Count; i++)
            {
                var roleVisual = GameWorld.Instance.CreateRole(playerDataList[i].PlayerRoleId,1);
                loadTaskSort.AddLoadTask(roleVisual.LoadActor(),roleVisual);
            }
            
            await loadTaskSort.WaitAllTasksAsync();
            await UniTask.Yield(PlayerLoopTiming.Update);
            var terrain = current.GetComponentInChild<Terrain>();
            var heightmapResolution = terrain.terrainData.heightmapResolution;
            var heights = terrain.terrainData.GetHeights(0,0, heightmapResolution, heightmapResolution);
            var terrainSize = new FPVector2(terrain.terrainData.size.x, terrain.terrainData.size.x); 
            var terrainPosition = terrain.transform.position;
            var terrainQuaternion = terrain.transform.rotation;
            var meshVertices = new List<FPVector3[]>(); 
            var meshRotations = new List<FPQuaternion>();
            var meshTriangles = new List<int[]>();
            var meshPositions = new List<FPVector3>();
            var meshScales = new List<FPVector3>();
            
            //获取 所有子物体的网格
            var meshFilters = current.GetComponentByNodeName<MeshFilter>("Building");
            if (meshFilters != null && meshFilters.Length > 0)
            {
                foreach (var meshFilter in meshFilters)
                { 
                    var vs = new FPVector3[meshFilter.mesh.vertices.Length];
                    for (int i = 0; i < vs.Length; i++)
                    {
                        vs[i] = meshFilter.mesh.vertices[i];
                    } 
                    meshVertices.Add(vs);   
                    var q = meshFilter.transform.localRotation;
                    meshRotations.Add(MathConvertor.QuaternionToFixQ(ref q));
                    meshTriangles.Add(meshFilter.mesh.triangles);
                    meshPositions.Add(meshFilter.transform.position);
                    meshScales.Add(meshFilter.transform.localScale);
                }
            }
            
            //把网格数据传入局内
            FrameContext.Context.InitWorldTerrain
            (
                heightmapResolutionParam:heightmapResolution, 
                heightsParam:heights, 
                terrainSizeParam:terrainSize, 
                terrainRotationParam:MathConvertor.QuaternionToFixQ(ref terrainQuaternion),
                terrainPositionParam:terrainPosition,
                meshVerticesParam:meshVertices, 
                meshRotationsParam:meshRotations,
                meshTrianglesParam:meshTriangles,
                meshPositionsParam:meshPositions,
                meshScalesParam:meshScales
            ); 
            
            FrameContext.Instance.CreateLogicRole(); 
            UIManager.Close<LoadUI>();
            UIManager.Close<MatchUI>();
            FrameContext.Context.Start();
            await UIManager.Show<FrameSortUI>();
        } 
    }
}