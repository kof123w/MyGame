#if UNITY_EDITOR
using System.Collections.Generic;
using BEPUphysics.CollisionShapes; 
using UnityEngine;

namespace MyGame
{
    public class DrawMesh : MonoBehaviour
    {  
        private List<MobileMeshShape> mobileMeshShapes;
        private MeshFilter meshFilter;
        public void SetMeshFilter( List<MobileMeshShape> mobileMeshShapes )
        {
            this.mobileMeshShapes = mobileMeshShapes;
        }

        private void OnDrawGizmosSelected()
        {
            
            if (mobileMeshShapes == null) return;
            foreach (MobileMeshShape mobileMeshShape in mobileMeshShapes)
            {
                // 提取顶点和三角形数据
                var vertices = mobileMeshShape.TriangleMesh.Data.Vertices;
                var indices = mobileMeshShape.TriangleMesh.Data.Indices;

                // 转换为 Unity 的 Mesh
                Mesh gizmoMesh = new Mesh();
                Vector3[] unityVertices = new Vector3[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    unityVertices[i] = new Vector3((float)vertices[i].x, (float)vertices[i].y, (float)vertices[i].z);
                }
                gizmoMesh.vertices = unityVertices;
                gizmoMesh.triangles = indices; 
                gizmoMesh.RecalculateNormals();
                gizmoMesh.RecalculateBounds(); // 关键：必须计算边界！

                // 3. 设置 Gizmos 参数
                Gizmos.color = Color.Lerp(Color.blue, Color.clear, 0.5f);
                Gizmos.matrix = Matrix4x4.TRS(
                    transform.position,
                    transform.rotation,
                    Vector3.one
                );

                // 4. 绘制网格
                Gizmos.DrawMesh(gizmoMesh, 0);
            } 
        }
    }
}

#endif