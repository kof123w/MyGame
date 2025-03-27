using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyGame
{
    public interface IResource
    {
        void UnloadUnusedAssets();

        IEnumerator LoadResourceGameObjectAsync(string path, Transform parent, Action<GameObject> onLoaded,AsyncTiming asyncTiming);
    }
}