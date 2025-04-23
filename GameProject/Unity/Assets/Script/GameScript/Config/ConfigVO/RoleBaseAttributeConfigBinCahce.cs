using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
namespace Config
{
  class RoleBaseAttributeConfigBinCache:CacheObject<RoleBaseAttributeConfig>
  {
    public RoleBaseAttributeConfigBinCache()
    {
#if UNITY_LOCAL_SCRIPT
      byte[] data = File.ReadAllBytes("Assets\\Resources\\Config\\RoleBaseAttributeConfig.bin");
#else
#endif
      var list = RoleBaseAttributeConfigList.Parser.ParseFrom(data);
      var enumerator = list.DataList.GetEnumerator();
      while (enumerator.MoveNext())
      {
         CacheList.Add(enumerator.Current);
       }
       enumerator.Dispose();
    }
  }
}
