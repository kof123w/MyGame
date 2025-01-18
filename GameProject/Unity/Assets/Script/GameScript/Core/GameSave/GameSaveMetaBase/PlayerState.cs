
//测试用元数据

using System;

namespace MyGame
{
    [Serializable]
    public class PlayerState : IMetaData
    {
        public string Name { get; set; }
        public byte No { get; set; }
        public int Level { get; set; } 
        
        public void Update(IMetaData metaData)
        {
            PlayerState tmp = metaData as PlayerState;
            if (tmp != null)
            {
                Name = tmp.Name;
                No = tmp.No;
                Level = tmp.Level;
            }
        }
    }
}
