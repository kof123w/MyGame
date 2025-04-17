using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.JointLimits;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Constraints.TwoEntity.Motors;
using EventSystem;
using FixedMath;
using FixMath.NET;
using UnityEngine;

namespace MyGame
{
    public class PlayerCharacter : BasePerformer
    { 

        //做一下摄像机控制
        private ThirdPersonCamera thirdPersonCamera;
        private Transform cameraHandle;
        private Transform cameraPos;
        private Animator animator;
        private const string moveAnimaParam = "MoveSpeed";
        
        public override void Start()
        { 
            base.Start();
            var body = EntityShape;
            // 禁用旋转（防止角色摔倒）
            body.LocalInertiaTensorInverse = new FPMatrix3x3();
            cameraHandle = trans.Find("CameraHandle");
            cameraPos = trans.Find("CameraHandle/CameraPos");
            animator = trans.GetComponentInChildren<Animator>();
            //射线机跟随
            var camera = GameWorld.Instance.GetMainCamera();
            thirdPersonCamera = new ThirdPersonCamera(
                EntityShape, 
                camera,
                cameraHandle,
                cameraPos
            );
            thirdPersonCamera.CheckCamera();
            this.Subscribe<Vector2,bool>(SignalEvent.SignalControl_MoveSignal,MoveSignalHandle);
            this.Subscribe<Vector3>(SignalEvent.SignalControl_CameraMoveSignal,thirdPersonCamera.RevDelta);
        } 
        
        private Quaternion lastMoveRotation = Quaternion.identity;

        private void MoveSignalHandle(Vector2 input,bool isInputKey)
        {
            if (input.magnitude <= 0.001f)
            {
                return;
            } 
            
            animator.SetFloat(moveAnimaParam, input.magnitude);
            
            var moveVector =  new  Vector3(input.x, 0, input.y);
            if (isInputKey)
            {
                moveVector = cameraHandle.rotation * moveVector;
                lastMoveRotation = cameraHandle.rotation;
            }
            else
            {
                moveVector = lastMoveRotation * moveVector;
            }

            var moveDirection = moveVector.normalized;
            var speed = roleBaseAttributeConfig.MoveSpeed;  
            EntityShape.LinearVelocity = new FPVector3(moveVector.x * speed , EntityShape.LinearVelocity.y, moveVector.z * speed);

            var modelForward = new Vector3(moveDirection.x, 0, moveDirection.z);
            Quaternion targetRotation = Quaternion.LookRotation(modelForward, Vector3.up);
            roleTransform.rotation = Quaternion.Slerp(roleTransform.rotation, targetRotation, Time.deltaTime * 20f);
        }

        protected override void OnDestroy()
        {
            thirdPersonCamera.Dispose();
            this.UnSubscribe(InputEvent.KeyboardHold);
            this.UnSubscribe(InputEvent.MoveMouse);
            base.OnDestroy();  
        }
    }
}