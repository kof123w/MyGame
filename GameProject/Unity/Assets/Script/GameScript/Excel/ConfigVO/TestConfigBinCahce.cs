using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
namespace Config
{
  class TestConfigBinCache:CacheObject<TestConfig>
  {
    public TestConfigBinCache()
    {
      byte[] data = File.ReadAllBytes("Assets\\Resources\\Config\\TestConfig.bin");
      var list = TestConfigList.Parser.ParseFrom(data);
      var enumerator = list.DataList.GetEnumerator();
      while (enumerator.MoveNext())
      {
         CacheList.Add(enumerator.Current);
       }
       enumerator.Dispose();
    }
  }
}
