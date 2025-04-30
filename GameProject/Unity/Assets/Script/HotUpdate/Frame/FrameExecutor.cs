using DebugTool;
using FixedMath;
using FixMath.NET;

namespace MyGame
{ 

    internal class FrameExecutor
    {
        internal void Execute(FrameData frameData)
        {
            if (frameData != null)
            { 
                for (int i = 0;i < frameData.FramePlayerInputList.Count;i++) {
                     var framePlayerInput = frameData.FramePlayerInputList[i]; 
                     var playerEntity = FrameContext.Context.GetEntityFromWorld(framePlayerInput.PlayerId);
                     if (framePlayerInput.FrameInput != null)
                     {
                         var x = default(Fix64);
                         x.RawValue = framePlayerInput.FrameInput.PlayerInput.Dup;
                         var z = default(Fix64);
                         z.RawValue = framePlayerInput.FrameInput.PlayerInput.Dright;
                         if (framePlayerInput.PlayerId == FrameContext.Context.CtrlRoleID)
                         {
                             DLogger.Log($"Frame executed:X Axis: {(float)x}, Z Axis: {(float)-z}"); 
                         }
                         playerEntity.linearVelocity = new FPVector3(x,playerEntity.linearVelocity.y,-z);
                     }
                     else
                     {
                         playerEntity.linearVelocity = new FPVector3(0, playerEntity.linearVelocity.y, 0);
                     }
                }
            } 
        }
    }
}