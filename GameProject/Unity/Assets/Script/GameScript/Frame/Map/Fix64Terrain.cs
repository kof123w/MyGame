using System.Collections.Generic;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Entities;
using FixedMath;
using FixMath.NET;
using UnityEngine;
using Material = BEPUphysics.Materials.Material;

namespace MyGame.Map
{
    internal class Fix64Terrain : Fix64Shape
    { 
        private BEPUphysics.BroadPhaseEntries.Terrain fixedTerrain;
        private const float resolutionScaleDiv = 1.0f;
        private List<MobileMeshShape> mobileMeshShapes = new();
        private readonly List<Entity> entityShapes = new();
        
        //terrain参数
        private int heightmapResolution;
        private float[,] heights;
        private FPVector2 terrainSize; 
        private FPQuaternion terrainRotation; 
        private FPVector3 terrainPosition;
        
        private List<FPVector3[]> meshVertices;
        private List<FPQuaternion> meshRotations;
        private List<int[]> meshTriangles;
        private List<FPVector3> meshPositions;
        private List<FPVector3> meshScales;
        
        public void Init(int heightmapResolutionParam, float[,] heightsParam,FPVector2 terrainSizeParam,FPQuaternion terrainRotationParam,FPVector3 terrainPositionParam,
            List<FPVector3[]> meshVerticesParam,List<FPQuaternion> meshRotationsParam,List<int[]> meshTrianglesParam,List<FPVector3> meshPositionsParam,List<FPVector3> meshScalesParam)
        {
            mobileMeshShapes.Clear();
            entityShapes.Clear();
            heightmapResolution = heightmapResolutionParam;
            heights = heightsParam;
            terrainSize = terrainSizeParam;
            terrainRotation = terrainRotationParam;
            terrainPosition = terrainPositionParam;
            meshVertices = meshVerticesParam;
            meshRotations = meshRotationsParam;
            meshTriangles = meshTrianglesParam;
            meshPositions = meshPositionsParam;
            meshScales = meshScalesParam;
            
            //把地形加到物理世界
            AddFixedTerrain();
            //把场景建筑的网格加到物理世界
            CreateEntityShapes();
            var physicsSpace = FrameContext.Context.GetSpace();
            foreach (var entity in entityShapes)
            {
                entity.BecomeKinematic();
                entity.Tag = PhysicsTag.MapTag;
                physicsSpace.Add(entity);
            }
            fixedTerrain.Tag = PhysicsTag.MapTag;
            physicsSpace.Add(fixedTerrain); 
        }
         
         
        private void AddFixedTerrain()
         { 
             var size = new FPVector3(
                 terrainSize.x/(heightmapResolution/ resolutionScaleDiv - 1)
                 , 1,
                 terrainSize.y/(heightmapResolution/ resolutionScaleDiv - 1)
             );
             var fixHeights = new Fix64[heightmapResolution, heightmapResolution];
             for (int i = 0; i < fixHeights.GetLength(0); i++)
             {
                 for (int j = 0; j < fixHeights.GetLength(1); j++)
                 {
                     fixHeights[i, j] = heights[j, i] * terrainSize.x;
                 }
             }

             fixedTerrain = new BEPUphysics.BroadPhaseEntries.Terrain(
                 heights:fixHeights,
                 worldTransform : new AffineTransform(
                     scaling:size, 
                     orientation:terrainRotation,
                     translation:terrainPosition 
                     )
                 ); 
            
             fixedTerrain.Material ??= new Material(); 
             fixedTerrain.Material.StaticFriction = 0.3f;
             fixedTerrain.Material.KineticFriction = 0.4f;
         }


        private void CreateEntityShapes()
        {
            for (int i = 0; i < meshVertices.Count; i++) { 
                var meshShape = new MobileMeshShape(
                    vertices:meshVertices[i], 
                    indices:meshTriangles[i], 
                    localTransform:new AffineTransform(
                        scaling:meshScales[i],
                        orientation:meshRotations[i],
                        translation:FPVector3.Zero), 
                    solidity:MobileMeshSolidity.DoubleSided);
                meshShape.Volume = 1;
                mobileMeshShapes.Add(meshShape);
                Entity entity = new Entity(meshShape);
                entity.Position = meshPositions[i];
                entity.BecomeKinematic();
                entityShapes.Add(entity);
            } 
        } 
        
        public new void OnDestroy()
        {
            var physicsSpace = FrameContext.Context.GetSpace();
            foreach (var entity in entityShapes)
            { 
                physicsSpace.Remove(entity);
            }
            physicsSpace.Remove(fixedTerrain);
            entityShapes.Clear();
        }
    }
}