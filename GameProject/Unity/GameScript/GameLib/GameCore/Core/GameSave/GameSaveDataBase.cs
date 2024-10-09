using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyGame
{
    //数据库序列化顺序,不要乱修改类型的顺序，有新东西需要保存往后插入
    enum PlayerDataType
    {
        PlayerState,  //角色状态
        PlayerMap,   //角色地图状态
        PlayerBag,  //角色背包状态
        Max,
    }
    
    public class GameSaveDataBase : ISaveDataBase
    {
        public long Id { get;  set; }
        //通过元数据进行序列化
        private Dictionary<PlayerDataType, IMetaData> m_metaDatas = new Dictionary<PlayerDataType, IMetaData>();

        private bool m_isInited = false;
        
        //初始化 序列化结构
        public void Init()
        {
            PlayerState playerState = new PlayerState(); 
            m_metaDatas.Add(PlayerDataType.PlayerState,playerState);
            m_isInited = true;
        }

        //保存下
        [Obsolete("Obsolete")]
        public void Save()
        {
            int begin = (int)PlayerDataType.PlayerState;
            int end = (int)PlayerDataType.Max;
            var savePath = $"{GameSaveConst.SavePath}//{Id}";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            for (int i = begin;i < end;i++) {
                if (m_metaDatas.ContainsKey((PlayerDataType)i))
                {
                    IMetaData metaData = m_metaDatas[(PlayerDataType)i];
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream fileStream = File.Create($"{GameSaveConst.SavePath}//{Id}//{i}.sav");
                    bf.Serialize(fileStream,metaData);
                    fileStream.Close(); 
                }
            }
        }

        [Obsolete("Obsolete")]
        public void Load()
        {
            int begin = (int)PlayerDataType.PlayerState;
            int end = (int)PlayerDataType.Max; 
            var savePath = $"{GameSaveConst.SavePath}//{Id}";
            if (!Directory.Exists(savePath))
            {
                return;
            }

            if (!m_isInited)
            {
                Init();
            }

            for (int i = begin;i < end;i++) {
                if (m_metaDatas.ContainsKey((PlayerDataType)i))
                {
                    string fileName = $"{GameSaveConst.SavePath}//{Id}//{i}.sav";
                    BinaryFormatter bf = new BinaryFormatter();//创建一个二进制格式化程序
                    if (File.Exists(fileName))
                    {
                        FileStream fileStream = File.Open(fileName, FileMode.Open);//打开数据流
                        IMetaData data = (IMetaData)bf.Deserialize(fileStream);//调用二进制格式化程序中的反序列化方法，将数据流反序列化为save对象并进行保存
                        m_metaDatas[(PlayerDataType)i].Update(data);
                        fileStream.Close();//关闭文件流
                    }
                } 
            } 
        }
    }
}
