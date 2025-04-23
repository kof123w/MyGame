using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
namespace Config
{
  class DefInputConfigBinCache:CacheObject<DefInputConfig>
  {
    public DefInputConfigBinCache()
    {
#if UNITY_LOCAL_SCRIPT
      byte[] data = File.ReadAllBytes("Assets\\Resources\\Config\\DefInputConfig.bin");
#else
#endif
      var list = DefInputConfigList.Parser.ParseFrom(data);
      var enumerator = list.DataList.GetEnumerator();
      while (enumerator.MoveNext())
      {
         CacheList.Add(enumerator.Current);
       }
       enumerator.Dispose();
    }
  }
}
