using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace Cheng.GameTemplates.Inventorys.Weights
{

    /// <summary>
    /// 包含重量对象的公共接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
                return def;
            }
        }
        private static GetItemWeight<T> def = f_createDef();

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

        private static GetItemWeight<T> f_createDef()
        {
            Type objType = typeof(T);
            bool flag;

            flag = typeof(IObjItemWeight).IsAssignableFrom(objType);
            if (flag)
            {
                var isValue = objType.IsValueType;
                Type t;
                if (isValue)
                {
                    t = typeof(ByInterFaceWeightValue<>);
                }
                else {
                    t = typeof(ByInterFaceWeight<>);
                }
                t = t.MakeGenericType(objType);
                return (GetItemWeight<T>)Activator.CreateInstance(t);
            }

            return new NotInterFace();
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


    internal sealed class DelegateByWeight<T> : GetItemWeight<T>
    {
        public DelegateByWeight(Func<T, ulong> func)
        {
            toWeight = func;
        }
        private Func<T, ulong> toWeight;
        public override ulong GetWeight(T obj)
        {
            return toWeight.Invoke(obj);
        }
    }

}
