#if UNITY_EDITOR
using FixedMath;
using UnityEngine;

namespace MyGame
{
    public class DrawTerrainHeight : MonoBehaviour
    {
        private BEPUphysics.BroadPhaseEntries.Terrain fixTerrain;
        private Terrain terrain;
        
        public Color color1 = Color.blue;
        public Color color2 = Color.clear;
        public float time = 0.5f;
        public float resolutionScaleDiv = 1;
        public void SetFixTerrain(BEPUphysics.BroadPhaseEntries.Terrain fixTerrain, Terrain terrain)
        {
            this.fixTerrain = fixTerrain;
            this.terrain = terrain;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (fixTerrain == null) return;
            Gizmos.color = Color.Lerp(color1, color2, time);

            var resolusion = terrain.terrainData.heightmapResolution;
            var size = new FPVector3(
                terrain.terrainData.size.x/(resolusion/ resolutionScaleDiv - 1)
                , 1,
                terrain.terrainData.size.x/(resolusion/ resolutionScaleDiv - 1)
            );
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, ((Vector3)size));
            for (int i = 0; i < fixTerrain.Shape.Heights.GetLength(0); i+=1)
            {
                for (int j = 0; j < fixTerrain.Shape.Heights.GetLength(1); j+=1)
                {
                    Gizmos.DrawCube(new Vector3(i, ((float)fixTerrain.Shape.Heights[i,j]), j), Vector3.one);
                }
            }
        }
    }
}
#endif

