using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using AssetsLoad;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace Config
{
  class DefInputConfigBinCache:CacheObject<DefInputConfig>
  {
    public DefInputConfigBinCache()
    {
         LoadConfig();
    }
#if UNITY_LOCAL_SCRIPT
    private void LoadConfig()
     {
        byte[] data = File.ReadAllBytes("Assets\\Resources_moved\\Config\\DefInputConfig.bytes");
#else
    private async UniTaskVoid LoadConfig()
    {
        TextAsset textAsset =  await ResourcerDecorator.Instance.LoadConfigAssetAsync("DefInputConfig");
        byte[] data =  textAsset.bytes;
#endif
        var list = DefInputConfigList.Parser.ParseFrom(data);
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
