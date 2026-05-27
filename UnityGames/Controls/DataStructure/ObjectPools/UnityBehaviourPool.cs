using UnityEngine;

namespace Cheng.Unitys.ObjectPools
{

    /// <summary>
    /// Unity可激活对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnityBehaviourPool<T> : UnityObjectPool<T> where T : Behaviour
    {

        #region 构造
        /// <summary>
        /// 实例化一个unity可激活对象池
        /// </summary>
        public UnityBehaviourPool() : base()
        {
        }
        /// <summary>
        /// 实例化一个unity可激活对象池
        /// </summary>
        /// <param name="behaviour">指定对象生成样本</param>
        public UnityBehaviourPool(T behaviour) : base(behaviour)
        {
        }
        #endregion

        #region 功能

        /// <summary>
        /// 生成一个对象并激活
        /// </summary>
        /// <param name="value">生成的对象</param>
        /// <returns>是否生成成功</returns>
        public override bool Generator(out T value)
        {
            bool flag = base.Generator(out value);
            if (flag) value.enabled = true;
            return flag;
        }
        /// <summary>
        /// 将对象存入对象池并停止对象活动
        /// </summary>
        /// <param name="value">要存入的对象</param>
        /// <returns>是否存放成功</returns>
        public override bool Push(T value)
        {
            bool flag = base.Push(value);
            if (flag) value.enabled = false;
            return flag;
        }

        #endregion

    }

}
