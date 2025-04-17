using FixedMath;
using UnityEngine;

namespace MyGame
{
    public static class MathConvertor
    {
        public static Vector2 FpVector2ConvertToVector2(ref FPVector2 fpVector2)
        {
            return new Vector2((float)fpVector2.x, (float)fpVector2.y); 
        }

        public static FPVector3 Vector3ConvertToFpVector3(ref Vector3 vector3)
        {
            return new FPVector3(vector3.x, vector3.y, vector3.z);
        }

        public static Vector3 FpVector3ConvertToVector3(ref FPVector3 fpVector3)
        {
            return new Vector3((float)fpVector3.x, (float)fpVector3.y, (float)fpVector3.z);
        }


        public static Quaternion FpQuaternionToQuaternion(ref FPQuaternion fpQuaternion)
        {
            return new Quaternion((float)fpQuaternion.x, (float)fpQuaternion.y, (float)fpQuaternion.z, (float)fpQuaternion.w);
        }
        
        public static FPQuaternion QuaternionToFpQuaternion(ref Quaternion quaternion)
        {
            return new FPQuaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }

}