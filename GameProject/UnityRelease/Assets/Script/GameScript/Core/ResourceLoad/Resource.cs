using System;
using System.IO;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyGame
{
    //Use the basic assets-bundle solution (Async and Sync) 
    public class Resource : IResource
    {
        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        public void DestroyObject(Object go)
        {
            Object.Destroy(go);
        }

        public void DestroyObject(Object go, float delayTime)
        {
            Object.Destroy(go, delayTime);
        }

        public GameObject AllocGameObject(string resPath, Transform parent)
        {
            var prefab = LoadResourceAsset(resPath, typeof(GameObject));
            return Object.Instantiate(prefab, parent) as GameObject;
        }

        public IEnumerator AllocGameObjectAsync(string resPath, Action<GameObject> onLoaded, Transform parent)
        {
            yield return LoadResourceAssetAsync(resPath, parent, onLoaded);
        }

        public Object LoadResourceAsset(string path, Type type)
        {
            if (type == null)
            {
                Debug.LogError(string.Format("ERROR: Asset type {0} not Exist!",path));
                return null;
            }
            return Resources.Load(path, type);
        }

        public IEnumerator LoadResourceAssetAsync(string path, Transform parent,  Action<GameObject> onLoaded)
        {
            AssetBundleCreateRequest rq = AssetBundle.LoadFromMemoryAsync(UnityEngine.Windows.File.ReadAllBytes(path));
            yield return rq;

            var assetName = Path.GetFileNameWithoutExtension(path);
            if (!String.IsNullOrEmpty(assetName))
            {
                var loadAsset  = rq.assetBundle.LoadAssetAsync<GameObject>(assetName);
                yield return loadAsset ;
                var finalGo = Object.Instantiate(loadAsset.asset , parent) as GameObject;
                if (onLoaded != null && !finalGo)
                {
                    onLoaded(finalGo);
                }   
            }
        }
    }
}