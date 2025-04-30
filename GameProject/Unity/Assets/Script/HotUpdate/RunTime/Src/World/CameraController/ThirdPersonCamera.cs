using System;
using System.Threading; 
using Cysharp.Threading.Tasks; 
using UnityEngine;

namespace MyGame
{
    public class ThirdPersonCamera : IDisposable
    {
        public Transform TargetEntity { get; set; }
        public Camera Camera { get; set; }
        public Transform CameraTransform { get; set; }
        public float maxDistance { get; set; } = 7f;
        public float minDistance { get; set; } = 2f;

        public float XSensitivity { get; set; } = 1f;  //X轴灵敏度
        public float YSensitivity { get; set; } = 0.8f;  //Y轴灵敏度

        private Transform cameraHandle;
        private Transform cameraPos;

        private float xRotation;
        private float yRotation;
        
        public float yMinLimit = -60f;
        public float yMaxLimit = 30f;
        
        //接收delta
        private Vector3 Delta { get; set; }

        private CancellationTokenSource fixedUpdateTokenSource;

        public ThirdPersonCamera(Transform targetEntity,Camera mainCamera,Transform cameraHandle,Transform cameraPos)
        {
            TargetEntity = targetEntity;
            Camera = mainCamera;
            CameraTransform = mainCamera.transform;
            this.cameraHandle = cameraHandle;
            this.cameraPos = cameraPos;
        }
        public void CheckCamera()
        {
            fixedUpdateTokenSource?.Cancel();
            fixedUpdateTokenSource?.Dispose();
            fixedUpdateTokenSource=new CancellationTokenSource(); 
            FixedUpdateAsync(fixedUpdateTokenSource.Token).Forget(); 
        }


        private async UniTaskVoid FixedUpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate,token); 
                xRotation += Delta.x * XSensitivity;
                yRotation -= Delta.y * YSensitivity;
                yRotation = Mathf.Clamp(yRotation, yMinLimit, yMaxLimit);
                cameraHandle.rotation = Quaternion.Euler(0, xRotation, yRotation);
                //CameraTransform.position = cameraPos.position; 
                CameraTransform.forward = cameraPos.forward;
                
                //安装碰撞获取一下地面，摄像机不能掉到地面里面
                /*Vector3 pos = cameraPos.position;
                RayCastResult hit; 
                var ray = new FPRay(cameraHandle.position, -cameraPos.forward); 
                var physicsSpace = GameWorld.GetPhysicsSpace();
                if (physicsSpace.RayCast(ray,maxDistance , entity =>
                    {
                        if ( entity.Tag!=null)
                        {
                           return  entity.Tag.Equals(PhysicsTag.MapTag);
                        } 
                        return false;
                    }, out hit ))
                {
                    //防止摄像机穿入地图内部
                    if (hit.HitObject.Tag!=null && hit.HitObject.Tag.Equals(PhysicsTag.MapTag))
                    {
                        var hitPos = hit.HitData.Location;
                        var hitDistance = VectorHelper.Vector3DistancePow(MathConvertor.FpVector3ConvertToVector3(ref hitPos), cameraHandle.position);

                        if (hitDistance < maxDistance * maxDistance)
                        {
                            pos = -cameraPos.forward * Mathf.Max(Mathf.Sqrt(hitDistance ) - 1,minDistance) + cameraHandle.position;
                        }
                    }
                }  
                CameraTransform.position = pos;*/
            }
        }

        public void RevDelta(Vector3 delta)
        {
            Delta = delta;
        }

        public void Dispose()
        {
            fixedUpdateTokenSource?.Cancel();
            fixedUpdateTokenSource?.Dispose();
            fixedUpdateTokenSource=null; 
        }
    }
}