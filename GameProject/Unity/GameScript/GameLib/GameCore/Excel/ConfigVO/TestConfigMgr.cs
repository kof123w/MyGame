using System.Collections.Generic;
using System.IO;
using MyGame;

namespace Config
{
     class TestConfigMgr : Singleton<TestConfigMgr>
      {
             private  Dictionary<int,TestConfig> m_dict = new Dictionary<int,TestConfig>();
             public TestConfigMgr()
              {
                      using (StreamReader sr = new StreamReader("Assets\\StreamingAssets\\Config\\TestConfig.bin"))
                      {
                              while (sr.Peek() >= 0)
                              {
                                      string line = sr.ReadLine();
                                      string[] splitArr = line.Split('\t');
                                     TestConfig data = new TestConfig();
                                     data.ID=int.Parse(splitArr[0]);
                                     data.Name=splitArr[1];
                                     m_dict.Add(data.ID,data); 
                              }
                      }
              }
             public TestConfig GetTestConfigConfig(int id)
              {
                     TestConfig res = null;
                      if(m_dict.TryGetValue(id,out res)) return res;
                      return null;
              }
      }
}
