using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace Cheng.GameTemplates.Inventorys.InventoryByWeights
{

    /// <summary>
    /// 获取对象重量值的公共接口
    /// </summary>
    public interface IObjItemWeight
    {
        /// <summary>
        /// 派生此接口的对象重量参数
        /// </summary>
        ulong Weight { get; }
    }

    /// <summary>
    /// 重量获取的公共接口
    /// </summary>
    /// <remarks>派生该接口实现获取对象重量值的方法，也可以将类型继承<see cref="IObjItemWeight"/>接口匹配默认实现</remarks>
    /// <typeparam name="T"></typeparam>
    public interface IGetItemWeight<T>
    {
        /// <summary>
        /// 获取指定对象的重量参数
        /// </summary>
        /// <param name="obj">指定对象</param>
        /// <returns>对象<paramref name="obj"/>的重量参数</returns>
        ulong GetWeight(T obj);
    }

    /// <summary>
    /// 获取指定对象重量数据的公共基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GetItemWeight<T> : IGetItemWeight<T>
    {

        /// <summary>
        /// 获取指定对象的重量参数
        /// </summary>
        /// <param name="obj">指定对象</param>
        /// <returns>对象<paramref name="obj"/>的重量参数</returns>
        /// <exception cref="NotImplementedException">没有默认实现的类型使用<see cref="Default"/></exception>
        public abstract ulong GetWeight(T obj);

        /// <summary>
        /// 获取指定对象的默认重量获取实现
        /// </summary>
        /// <returns>
        /// <para>获取类型<typeparamref name="T"/>的重量获取默认实现；当<typeparamref name="T"/>派生于<see cref="IObjItemWeight"/>接口时，<see cref="GetWeight(T)"/>函数返回接口实现；如果没有则引发<see cref="NotImplementedException"/>异常</para>
        /// </returns>
        public static GetItemWeight<T> Default
        {
            get
            {
                return LazyLoad.def;
            }
        }

        /// <summary>
        /// 创建一个委托到重量获取接口
        /// </summary>
        /// <param name="toWeight">要封装的委托方法</param>
        /// <returns>重量接口</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static GetItemWeight<T> CreateDelegate(Func<T, ulong> toWeight)
        {
            if (toWeight is null) throw new ArgumentNullException();
            return new DelegateByWeight<T>(toWeight);
        }

        #region

        private static class LazyLoad
        {

            public static GetItemWeight<T> def = f_createDef();

            private static GetItemWeight<T> f_createDef()
            {
                Type objType = typeof(T);
                bool flag;
                try
                {
                    flag = typeof(IObjItemWeight).IsAssignableFrom(objType);
                    if (flag)
                    {
                        var isValue = objType.IsValueType;
                        Type t;
                        if (isValue)
                        {
                            t = typeof(ByInterFaceWeightValue<>);
                        }
                        else
                        {
                            t = typeof(ByInterFaceWeight<>);
                        }
                        t = t.MakeGenericType(objType);
                        return (GetItemWeight<T>)Activator.CreateInstance(t);
                    }
                }
                catch (Exception)
                {
                }

                return new NotInterFace();
            }

        }

        private class NotInterFace : GetItemWeight<T>
        {
            public override ulong GetWeight(T obj)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

    }

    #region

    internal sealed class DelegateByWeight<T> : GetItemWeight<T>
    {
        public DelegateByWeight(Func<T, ulong> func)
        {
            toWeight = func;
        }
        private readonly Func<T, ulong> toWeight;
        public override ulong GetWeight(T obj)
        {
            return toWeight.Invoke(obj);
        }
    }

    internal sealed class ByInterFaceWeight<T> : GetItemWeight<T> where T : IObjItemWeight
    {
        public ByInterFaceWeight()
        {
        }

        public sealed override ulong GetWeight(T obj)
        {
            return (obj == null) ? 0 : obj.Weight;
        }
    }

    internal sealed class ByInterFaceWeightValue<T> : GetItemWeight<T> where T : struct, IObjItemWeight
    {
        public ByInterFaceWeightValue()
        {
        }

        public sealed override ulong GetWeight(T obj)
        {
            return obj.Weight;
        }
    }

    #endregion

}
