using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Script
{
    public class RefTypes
    {
        public void MyAOTRefs()
        {
            new List<object>(); // 也可以用 new List<string>()
            new UniTask<object>();
            new UniTask<Object>();
            
            _ = (object)(new ValueTuple<int, object>());
            _ = (object)(default(ValueTuple<int, object>));
        }
    }
}
