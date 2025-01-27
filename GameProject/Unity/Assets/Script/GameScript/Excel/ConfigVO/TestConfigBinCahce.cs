using System.Collections.Generic;
using System.IO;
namespace Config
{
	class TestConfigBinCache:CacheObject<TestConfig>
	{
		public TestConfigBinCache()
		{
			FileStream fileStream = new FileStream("Assets\\StreamingAssets\\Config\\TestConfig.bin",FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			string[] strArray = binaryReader.ReadString().Split('\n');
			for (int i = 0; i < strArray.Length; i++)
			{
				var str = strArray[i];
				if(string.IsNullOrEmpty(str)) continue;
				var element = str.Split('|');
				TestConfig config = new TestConfig();
				config.ID=int.Parse(element[0]);
				config.Name=element[1];
				CacheList.Add(config);
			}
			binaryReader.Close();
			binaryReader.Dispose();
			fileStream.Close();
			fileStream.Dispose();
		}
	}
}
