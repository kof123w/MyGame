using System.Collections.Generic;
using System.IO;
using Google.Protobuf;

namespace Config
{
    class SceneLoadResourceConfigBinCache : CacheObject<SceneLoadResourceConfig>
    {
        public SceneLoadResourceConfigBinCache()
        {
            byte[] data = File.ReadAllBytes("Assets\\Resources\\Config\\SceneLoadResourceConfig.bin");
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