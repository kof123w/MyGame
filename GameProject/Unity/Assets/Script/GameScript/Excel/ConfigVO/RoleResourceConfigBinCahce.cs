using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
namespace Config
{
  class RoleResourceConfigBinCache:CacheObject<RoleResourceConfig>
  {
    public RoleResourceConfigBinCache()
    {
#if UNITY_LOCAL_SCRIPT
      byte[] data = File.ReadAllBytes("Assets\\Resources\\Config\\RoleResourceConfig.bin");
#else
#endif
      var list = RoleResourceConfigList.Parser.ParseFrom(data);
      var enumerator = list.DataList.GetEnumerator();
      while (enumerator.MoveNext())
      {
         CacheList.Add(enumerator.Current);
       }
       enumerator.Dispose();
    }
  }
}
