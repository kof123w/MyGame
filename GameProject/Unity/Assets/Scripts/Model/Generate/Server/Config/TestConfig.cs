using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System.ComponentModel;

namespace ET
{
    [Config]
    public partial class TestConfigCategory : Singleton<TestConfigCategory>, IMerge
    {
        [BsonElement]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        private Dictionary<int, TestConfig> dict = new();
		
        public void Merge(object o)
        {
            TestConfigCategory s = o as TestConfigCategory;
            foreach (var kv in s.dict)
            {
                this.dict.Add(kv.Key, kv.Value);
            }
        }
		
        public TestConfig Get(int id)
        {
            this.dict.TryGetValue(id, out TestConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (TestConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, TestConfig> GetAll()
        {
            return this.dict;
        }

        public TestConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

	public partial class TestConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		public int Id { get; set; }
		/// <summary>数据库地址</summary>
		public string DBConnection { get; set; }
		/// <summary>数据库名</summary>
		public string DBName { get; set; }

	}
}
