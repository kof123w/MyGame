using System;
using System.Collections.Generic; 
using Config;
using Cysharp.Threading.Tasks; 
using UnityEngine;

namespace MyGame
{
    
    public class PerformerVisual : VisualShape
    { 
        private RoleResourceConfig roleResourceConfig;
        protected RoleBaseAttributeConfig roleBaseAttributeConfig; 
        
       

        public async UniTaskVoid UnLoadResources()
        { 
            await UnloadResource(); 
        } 
        
        public async void SetConfigID(int id)
        {
            roleResourceConfig = await ResourceConfigManager.Instance.GetRoleResourceConfig(id);
            roleBaseAttributeConfig = await RoleBaseAttributeConfigManager.Instance.GetRoleBaseAttributeConfig(id);
        } 
        
        public async UniTask LoadActor()
        {
            await UniTask.WaitUntil(() => roleResourceConfig != null);
            await LoadAsset(roleResourceConfig.AssetPath); 
            trans.localPosition = Vector3.zero;
            roleTransform = trans.Find("RoleModel");
        }

        public void SetWorldPos(Vector3 pos)
        {
            worldTransform.position = pos;
        }

        public void SetWorldScale(Vector3 scale)
        {
            worldTransform.localScale = scale;
        }

        public void SetWorldRot(Quaternion rot)
        {
            worldTransform.rotation = rot;
        } 

        public void Forward()
        {
             
        }

        public void Jump()
        {
            
        }
    } 
}