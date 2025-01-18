using System.Collections.Generic;
using System.IO;
namespace Config
{
     class DefInputConfigMgr : Singleton<DefInputConfigMgr>
      {
             private  Dictionary<int,DefInputConfig> m_dict = new Dictionary<int,DefInputConfig>();
             public DefInputConfigMgr()
              {
                      using (StreamReader sr = new StreamReader("Assets\\StreamingAssets\\Config\\DefInputConfig.bin"))
                      {
                              while (sr.Peek() >= 0)
                              {
                                      string line = sr.ReadLine();
                                      string[] splitArr = line.Split('|');
                                     DefInputConfig data = new DefInputConfig();
                                     data.ID=int.Parse(splitArr[0]);
                                     data.Keys=splitArr[1];
                                     data.Cmd=splitArr[2];
                                     m_dict.Add(data.ID,data); 
                              }
                      }
              }
             public DefInputConfig GetDefInputConfigConfig(int id)
              {
                     DefInputConfig res = null;
                      if(m_dict.TryGetValue(id,out res)) return res;
                      return null;
              }
      }
}
