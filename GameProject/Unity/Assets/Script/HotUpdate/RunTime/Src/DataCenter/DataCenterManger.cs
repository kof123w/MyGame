using System;
using System.Collections.Generic;
using SingleTool;
using UnityEngine.UI;

namespace MyGame
{
    public class DataCenterManger : Singleton<DataCenterManger>
    {
        private Dictionary<Type, DataClass> dataDict = new Dictionary<Type, DataClass>();
        
        public void Initialize()
        {
             // todo   
        }

        public void AddDataClass(Type type,DataClass dataClass)
        {
            dataDict.Add(type, dataClass);
        }

        public T GetDataClass<T>() where T : DataClass
        {
            return dataDict.GetValueOrDefault(typeof(T)) as T;
        }

        public void OutLogin()
        {
            foreach (var pair in dataDict)
            {
                var dataClass = pair.Value;
                dataClass.OutLogin();
            }
        }
    }
}