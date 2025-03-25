using System.Collections.Generic;
using System.IO;

namespace MyGame
{
    public class GameSaveMgr : Singleton<GameSaveMgr>,ISaveDataMgr
    { 
        private Dictionary<long, ISaveDataBase> m_dict = new Dictionary<long, ISaveDataBase>(); 
        
        public void SaveBin()
        {
            //保存存档
            if (!Directory.Exists(GameSaveConst.SavePath))
            {
                Directory.CreateDirectory(GameSaveConst.SavePath);
            }

            foreach (var pair in m_dict)
            {
                pair.Value.Save();
            }
        }

        public void LoadBin()
        {
            //读取存档
            if (!Directory.Exists(GameSaveConst.SavePath))
            {
                Directory.CreateDirectory(GameSaveConst.SavePath);
            }
            
            if (Directory.Exists(GameSaveConst.SavePath))
            {
                string[] directories = Directory.GetDirectories($"{GameSaveConst.SavePath}{GameSaveConst.SavePathSy}");
                for (int i = 0; i < directories.Length; i++)
                {
                    string directory = directories[i]; 
                    string directoryName = Path.GetFileName(directory);
                    long id = long.Parse(directoryName);
                    ISaveDataBase dataBase;
                    if (m_dict.TryGetValue(id, out dataBase))
                    {
                        dataBase.Load();
                    }
                    else
                    {
                        dataBase = new GameSaveDataBase();
                        dataBase.Id = id;
                        dataBase.Load();
                        m_dict.Add(id,dataBase);
                    }
                }
            }
        }

        public void CreateBin()
        {
            //暂时测试用的
            ISaveDataBase dataBase = new GameSaveDataBase();
            dataBase.Id = TimeTool.GetTimeStamp(); 
            dataBase.Init();
            m_dict.Add(dataBase.Id,dataBase);
        }

        public void DeleteBin()
        {
            
        }
    } 
}

