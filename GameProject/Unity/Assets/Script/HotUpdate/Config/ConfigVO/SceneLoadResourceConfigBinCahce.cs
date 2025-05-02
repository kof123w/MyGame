using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using AssetsLoad;
using Cysharp.Threading.Tasks;
using DebugTool;
using UnityEngine;
namespace Config
{
  class SceneLoadResourceConfigBinCache:CacheObject<SceneLoadResourceConfig>
  {
    public SceneLoadResourceConfigBinCache()
    {
         LoadConfig();
    }
#if UNITY_LOCAL_SCRIPT
    private void LoadConfig()
     {
        byte[] data = File.ReadAllBytes("Assets\\Resources_moved\\Config\\SceneLoadResourceConfig.bytes");
#else
    private async UniTaskVoid LoadConfig()
    {
        TextAsset textAsset =  await ResourcerDecorator.Instance.LoadConfigAssetAsync("SceneLoadResourceConfig");
        byte[] data =  textAsset.bytes;
#endif
        var list = SceneLoadResourceConfigList.Parser.ParseFrom(data);
        var enumerator = list.DataList.GetEnumerator();
        while (enumerator.MoveNext())
        {
           CacheList.Add(enumerator.Current);
         }
         enumerator.Dispose();
         IsCompleteLoad = true;
         
         DLogger.Log(list.ToString());
       }
    }
}
