using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using AssetsLoad;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace Config
{
  class RoleResourceConfigBinCache:CacheObject<RoleResourceConfig>
  {
    public RoleResourceConfigBinCache()
    {
         LoadConfig();
    }
#if UNITY_LOCAL_SCRIPT
    private void LoadConfig()
     {
        byte[] data = File.ReadAllBytes("Assets\\Resources_moved\\Config\\RoleResourceConfig.bytes");
#else
    private async UniTaskVoid LoadConfig()
    {
        TextAsset textAsset =  await ResourcerDecorator.Instance.LoadConfigAssetAsync("RoleResourceConfig");
        byte[] data =  textAsset.bytes;
#endif
        var list = RoleResourceConfigList.Parser.ParseFrom(data);
        var enumerator = list.DataList.GetEnumerator();
        while (enumerator.MoveNext())
        {
           CacheList.Add(enumerator.Current);
         }
         enumerator.Dispose();
         IsCompleteLoad = true;
       }
    }
}
