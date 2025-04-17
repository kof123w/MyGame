using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Entities;
using Cysharp.Threading.Tasks;
using DebugTool;
using FixedMath;
using FixMath.NET;
using MyGame;
using UnityEngine;
using Material = BEPUphysics.Materials.Material;

namespace MyGame
{
    
    /// <summary>
    /// 地形初始化走自己的逻辑
    /// </summary>
    public class AssetFix64Terrain : AssetShape
    { 
        private UnityEngine.Terrain terrain;
        private BEPUphysics.BroadPhaseEntries.Terrain fixedTerrain; 
        private float resolutionScaleDiv = 1.0f;
        private List<MobileMeshShape> mobileMeshShapes = new List<MobileMeshShape>();
        private List<Entity> entityShapes = new List<Entity>();
        public override void Start()
        {
            //把地形加到物理世界
            AddFixedTerrain();
            
            //把场景建筑的网格加到物理世界
             CreateEntityShapes();
            
            /*var drawHeight = terrain.transform.gameObject.AddComponent<DrawTerrainHeight>();
            drawHeight.SetFixTerrain(fixedTerrain,terrain);
            var buildingRoot = trans.Find("Building");
            var drawMesh = buildingRoot.gameObject.AddComponent<DrawMesh>();
            drawMesh.SetMeshFilter(mobileMeshShapes);*/
            var physicsSpace = GameWorld.GetPhysicsSpace();
            foreach (var entity in entityShapes)
            {
                entity.BecomeKinematic();
                entity.Tag = PhysicsTag.MapTag;
                physicsSpace.Add(entity);
            }
            fixedTerrain.Tag = PhysicsTag.MapTag;
            physicsSpace.Add(fixedTerrain);
        }

       
        protected void CreateEntityShapes()
        {
            var buildingRoot = trans.Find("Building");
            if (buildingRoot == null)
            {
                return;  //这个场景没有建筑
            }

            MeshFilter[] meshFilters = buildingRoot.GetComponentsInChildren<MeshFilter>(); //获取 所有子物体的网格
            if (meshFilters == null || meshFilters.Length == 0)
            {
                return;
            }

            foreach (var meshFilter in meshFilters)
            {
                var vs = new FPVector3[meshFilter.mesh.vertices.Length];
                for (int i = 0; i < vs.Length; i++)
                {
                    vs[i] = meshFilter.mesh.vertices[i];
                } 

                var q = meshFilter.transform.localRotation;
                var meshShape = new MobileMeshShape(vs, meshFilter.mesh.triangles, new AffineTransform(meshFilter.transform.lossyScale,MathConvertor.QuaternionToFpQuaternion(ref q),FPVector3.Zero), MobileMeshSolidity.DoubleSided)
                    {
                        Volume = 1
                    };
                mobileMeshShapes.Add(meshShape);
                Entity entity = new Entity(meshShape);
                entity.Position = meshFilter.transform.position;
                entity.BecomeKinematic();
                entityShapes.Add(entity);
            }
        }

        private void AddFixedTerrain()
        {
            terrain = gameObject.GetComponentInChildren<UnityEngine.Terrain>();
            var resolusion = terrain.terrainData.heightmapResolution;
            var heights = terrain.terrainData.GetHeights(0,0, resolusion, resolusion);
            var size = new FPVector3(
                terrain.terrainData.size.x/(resolusion/ resolutionScaleDiv - 1)
                , 1,
                terrain.terrainData.size.x/(resolusion/ resolutionScaleDiv - 1)
            );
            var fixHeights = new Fix64[resolusion, resolusion];
            for (int i = 0; i < fixHeights.GetLength(0); i++)
            {
                for (int j = 0; j < fixHeights.GetLength(1); j++)
                {
                    fixHeights[i, j] =  (Fix64)heights[j, i] * terrain.terrainData.size.y;
                }
            }

            var q = terrain.transform.rotation; 
            fixedTerrain = new BEPUphysics.BroadPhaseEntries.Terrain(fixHeights,
                new AffineTransform(size, MathConvertor.QuaternionToFpQuaternion(ref q),terrain.transform.position )); 
            
            fixedTerrain.Material ??= new Material(); 
            fixedTerrain.Material.StaticFriction = 0.3f;
            fixedTerrain.Material.KineticFriction = 0.4f;
        }

        protected override void OnDestroy()
        {
            var physicsSpace = GameWorld.GetPhysicsSpace();
            foreach (var entity in entityShapes)
            { 
                physicsSpace.Remove(entity);
            }
            physicsSpace.Remove(fixedTerrain);
            entityShapes.Clear();
        }
    }
}