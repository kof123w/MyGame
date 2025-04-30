using FixedMath;
using UnityEngine;

namespace MyGame
{
    public static class MathConvertor
    {
        public static Vector2 Fix2ToVector2(ref FPVector2 fpVector2)
        {
            return new Vector2((float)fpVector2.x, (float)fpVector2.y); 
        }

        public static FPVector2 Vector2ToFix2(ref Vector2 vector)
        {
            return new FPVector2(vector.x, vector.y);
        }

        public static FPVector3 Vector3ToFix3(ref Vector3 vector3)
        {
            return new FPVector3(vector3.x, vector3.y, vector3.z);
        }

        public static Vector3 Fix3ToVector3(ref FPVector3 fpVector3)
        {
            return new Vector3((float)fpVector3.x, (float)fpVector3.y, (float)fpVector3.z);
        }


        public static Quaternion FixQToQuaternion(ref FPQuaternion fpQuaternion)
        {
            return new Quaternion((float)fpQuaternion.x, (float)fpQuaternion.y, (float)fpQuaternion.z, (float)fpQuaternion.w);
        }
        
        public static FPQuaternion QuaternionToFixQ(ref Quaternion quaternion)
        {
            return new FPQuaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }

}