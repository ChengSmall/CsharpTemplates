using System;

namespace Cheng.GameTemplates.Inventorys.Weights
{

    /// <summary>
    /// 派生公共接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ByInterFaceWeight<T> : GetItemWeight<T> where T : IObjItemWeight
    {
        public ByInterFaceWeight()
        {
        }

        public sealed override ulong GetWeight(T obj)
        {
            return (obj == null) ? 0 : obj.Weight;
        }
    }


    internal class ByInterFaceWeightValue<T> : GetItemWeight<T> where T : struct, IObjItemWeight
    {
        public ByInterFaceWeightValue()
        {
        }

        public sealed override ulong GetWeight(T obj)
        {
            return obj.Weight;
        }
    }

}
