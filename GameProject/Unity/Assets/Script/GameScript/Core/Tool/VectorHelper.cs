using UnityEngine;

namespace MyGame
{
    public class VectorHelper
    {
        public static void SquareToCircle(ref float up,ref float right)
        {
            up *= Mathf.Sqrt(1 - (right * right) / 2);
            right *= Mathf.Sqrt(1 - (up * up) / 2); 
        }

        public static Vector2 SquareToCircle(Vector2 input)
        {
            Vector2 output = new Vector2
            (
                input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2),
                input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2)
            );
            return output;
        }

        public static float Vector3DistancePow(Vector3 a, Vector3 b)
        { 
            return (a.x - b.x) * (a.x - b.x) + (a.y- b.y) * (a.y- b.y) + (a.z - b.z) * (a.z - b.z);
        }
    }
}