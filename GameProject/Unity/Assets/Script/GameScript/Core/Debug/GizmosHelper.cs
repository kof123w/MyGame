using UnityEngine;

namespace DebugTool
{
    public static class DebugDrawHelper {
        // 辅助方法：绘制立方体线框
        public static void DebugDrawWireCube(Vector3 center, Vector3 size, Color color, float duration = 0)
        {
            // 计算立方体的半长宽高
            Vector3 halfSize = size * 0.5f;

            // 立方体的 8 个顶点（局部坐标）
            Vector3[] vertices = new Vector3[8]
            {
                new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), // 0: 左前下
                new Vector3(halfSize.x, -halfSize.y, -halfSize.z),  // 1: 右前下
                new Vector3(halfSize.x, halfSize.y, -halfSize.z),   // 2: 右前上
                new Vector3(-halfSize.x, halfSize.y, -halfSize.z),  // 3: 左前上
                new Vector3(-halfSize.x, -halfSize.y, halfSize.z),   // 4: 左后下
                new Vector3(halfSize.x, -halfSize.y, halfSize.z),    // 5: 右后下
                new Vector3(halfSize.x, halfSize.y, halfSize.z),    // 6: 右后上
                new Vector3(-halfSize.x, halfSize.y, halfSize.z)     // 7: 左后上
            };

            // 将顶点从局部空间转换到世界空间
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += center;
            }

            // 绘制 12 条边
            Debug.DrawLine(vertices[0], vertices[1], color, duration); // 下前边
            Debug.DrawLine(vertices[1], vertices[2], color, duration); // 前右边
            Debug.DrawLine(vertices[2], vertices[3], color, duration); // 上前边
            Debug.DrawLine(vertices[3], vertices[0], color, duration); // 前左边

            Debug.DrawLine(vertices[4], vertices[5], color, duration); // 下后边
            Debug.DrawLine(vertices[5], vertices[6], color, duration); // 后右边
            Debug.DrawLine(vertices[6], vertices[7], color, duration); // 上后边
            Debug.DrawLine(vertices[7], vertices[4], color, duration); // 后左边

            Debug.DrawLine(vertices[0], vertices[4], color, duration); // 左下边
            Debug.DrawLine(vertices[1], vertices[5], color, duration); // 右下边
            Debug.DrawLine(vertices[2], vertices[6], color, duration); // 右上边
            Debug.DrawLine(vertices[3], vertices[7], color, duration); // 左上边
        }
    }
}