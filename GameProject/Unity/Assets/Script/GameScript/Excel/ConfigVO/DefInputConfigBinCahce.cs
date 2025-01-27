using System.Collections.Generic;
using System.IO;
namespace Config
{
	class DefInputConfigBinCache:CacheObject<DefInputConfig>
	{
		public DefInputConfigBinCache()
		{
			FileStream fileStream = new FileStream("Assets\\StreamingAssets\\Config\\DefInputConfig.bin",FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			string[] strArray = binaryReader.ReadString().Split('\n');
			for (int i = 0; i < strArray.Length; i++)
			{
				var str = strArray[i];
				if(string.IsNullOrEmpty(str)) continue;
				var element = str.Split('|');
				DefInputConfig config = new DefInputConfig();
				config.ID=int.Parse(element[0]);
				config.Keys=element[1];
				config.Cmd=element[2];
				CacheList.Add(config);
			}
			binaryReader.Close();
			binaryReader.Dispose();
			fileStream.Close();
			fileStream.Dispose();
		}
	}
}
