using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using AssetsLoad;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace Config
{
  class RoleBaseAttributeConfigBinCache:CacheObject<RoleBaseAttributeConfig>
  {
    public RoleBaseAttributeConfigBinCache()
    {
         LoadConfig();
    }
#if UNITY_LOCAL_SCRIPT
    private void LoadConfig()
     {
        byte[] data = File.ReadAllBytes("Assets\\Resources_moved\\Config\\RoleBaseAttributeConfig.bytes");
#else
    private async UniTaskVoid LoadConfig()
    {
        TextAsset textAsset =  await ResourcerDecorator.Instance.LoadConfigAssetAsync("RoleBaseAttributeConfig");
        byte[] data =  textAsset.bytes;
#endif
        var list = RoleBaseAttributeConfigList.Parser.ParseFrom(data);
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
