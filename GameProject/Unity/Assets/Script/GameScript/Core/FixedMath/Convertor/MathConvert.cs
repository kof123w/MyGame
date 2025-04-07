using FixedMath;
using UnityEngine;

namespace MyGame
{
    public static class MathConvertor
    {
        public static Vector2 FpVector2ConvertToVector2(ref FPVector2 FPVector2)
        {
            Vector2 vector2 = new Vector2((float)FPVector2.X, (float)FPVector2.Y);
            return vector2;
        }
    }

}