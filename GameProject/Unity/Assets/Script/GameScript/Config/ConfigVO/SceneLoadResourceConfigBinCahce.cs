using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
namespace Config
{
  class SceneLoadResourceConfigBinCache:CacheObject<SceneLoadResourceConfig>
  {
    public SceneLoadResourceConfigBinCache()
    {
#if UNITY_LOCAL_SCRIPT
      byte[] data = File.ReadAllBytes("Assets\\Resources\\Config\\SceneLoadResourceConfig.bin");
#else
#endif
      var list = SceneLoadResourceConfigList.Parser.ParseFrom(data);
      var enumerator = list.DataList.GetEnumerator();
      while (enumerator.MoveNext())
      {
         CacheList.Add(enumerator.Current);
       }
       enumerator.Dispose();
    }
  }
}
