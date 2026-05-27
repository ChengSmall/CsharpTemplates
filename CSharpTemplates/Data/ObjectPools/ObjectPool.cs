using Cheng.DataStructure.Collections;
using System;
using System.Collections.Generic;


namespace Cheng.DataStructure.ObjectPools
{

    /// <summary>
    /// 在对象池中创建对象的委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">返回创建后的对象</param>
    /// <returns>是否成功创建对象，若成功创建对象返回true，且让<paramref name="obj"/>引用接收创建好的对象；若返回false则此次无法创建对象，<paramref name="obj"/>引用可不做接收</returns>
    public delegate bool ObjectGenerator<T>(out T obj);

    /// <summary>
    /// 公用对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> 
    {

        #region 构造

        /// <summary>
        /// 实例化一个对象池
        /// </summary>
        /// <param name="objGenerator">
        /// 对象生成器
        /// <para>生成对象时，若对象池缓冲区中没有对象则使用该生成器生成对象；</para>
        /// <para>设置为null表示使用默认生成器，默认生成器使用无参构造实例化对象，若无法实例化则表示不可生成</para>
        /// <para>请确保调用该委托不会引发异常</para>
        /// </param>
        /// <param name="maxBufferObjectCount">对象池内最多可储存的元素数</param>
        /// <exception cref="ArgumentOutOfRangeException">最大储存数小于0</exception>
        public ObjectPool(ObjectGenerator<T> objGenerator, int maxBufferObjectCount)
        {
            if (maxBufferObjectCount < 0) throw new ArgumentOutOfRangeException();
            f_init(objGenerator);
        }

        /// <summary>
        /// 实例化一个对象池
        /// </summary>
        /// <param name="objGenerator">
        /// 对象生成器
        /// <para>生成对象时，若对象池缓冲区中没有对象则使用该生成器生成对象；</para>
        /// <para>设置为null表示使用默认生成器，默认生成器使用无参构造实例化对象，若无法实例化则表示不可生成</para>
        /// <para>请确保调用该委托不会引发异常</para>
        /// </param>
        public ObjectPool(ObjectGenerator<T> objGenerator)
        {
            f_init(objGenerator);
        }

        /// <summary>
        /// 实例化一个对象池
        /// </summary>
        public ObjectPool()
        {
            f_init(null);
        }

        private void f_init(ObjectGenerator<T> objGenerator)
        {
            SetObjectGenerator(objGenerator);
            p_stack = new ImmediatelyStack<T>();
        }

        #endregion

        #region 参数

        /// <summary>
        /// 对象创建器
        /// </summary>
        protected ObjectGenerator<T> p_createObj;

        /// <summary>
        /// 对象缓冲栈
        /// </summary>
        protected ImmediatelyStack<T> p_stack;

        #endregion

        #region 功能

        #region 封装

        static bool f_defCreateObj(out T value)
        {
            try
            {
                if (typeof(T).IsValueType)
                {
                    value = default;
                    return true;
                }

                value = Activator.CreateInstance<T>();
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
            
        }

        private bool f_generatorObjInvoke(out T value)
        {
            return p_createObj.Invoke(out value);
        }

        #endregion

        #region 参数

        /// <summary>
        /// 对象池内暂存的元素数
        /// </summary>
        public virtual int Count
        {
            get => p_stack.Count;
        }

        #endregion

        #region 创建对象

        /// <summary>
        /// 设置对象生成器
        /// </summary>
        /// <param name="func">
        /// 对象生成器
        /// <para>生成对象时，若对象池缓冲区中没有对象则使用该生成器生成对象；</para>
        /// <para>设置为null表示使用默认生成器，默认生成器使用无参构造实例化对象，若无法实例化则表示不可生成</para>
        /// <para>请确保调用该委托不会引发异常</para>
        /// </param>
        public virtual void SetObjectGenerator(ObjectGenerator<T> func)
        {
            p_createObj = (func is null) ? f_defCreateObj : func;
        }

        #endregion

        #region 弹出和压入

        /// <summary>
        /// 将指定的对象存入对象池
        /// </summary>
        /// <param name="value">要存入的对象</param>
        /// <returns>是否存入成功</returns>
        public virtual bool Push(T value)
        {          
            p_stack.Push(value);
            return true;
        }

        /// <summary>
        /// 生成一个对象
        /// </summary>
        /// <remarks>
        /// 在缓存区弹出一个对象，若缓存区没有对象则使用生成器生成一个对象
        /// </remarks>
        /// <param name="value">生成的对象</param>
        /// <returns>是否成功生成对象；成功生成返回true，无法生成返回false</returns>
        public virtual bool Generator(out T value)
        {          
            if(p_stack.Count == 0) return f_generatorObjInvoke(out value);
            
            value = p_stack.Pop();
            return true;
        }

        /// <summary>
        /// 清除该对象池中所有暂存的对象
        /// </summary>
        public virtual void Clear()
        {
            p_stack.Clear();
        }

        #endregion

        #endregion

    }

}
