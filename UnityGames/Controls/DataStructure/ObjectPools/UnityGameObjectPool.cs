using UnityEngine;

namespace Cheng.Unitys.ObjectPools
{

    /// <summary>
    /// Unity游戏对象池
    /// </summary>
    /// <remarks>
    /// 使用<see cref="GameObject"/>作为生成样本对象的对象池
    /// </remarks>
    public sealed class UnityGameObjectPool : UnityObjectPool<GameObject>
    {

        #region 构造
        /// <summary>
        /// 实例化一个Unity公共对象池
        /// </summary>
        /// <param name="generatorObj">指定对象生成样本</param>
        public UnityGameObjectPool(GameObject generatorObj) : base(generatorObj)
        {

        }
        /// <summary>
        /// 实例化一个Unity公共对象池
        /// </summary>
        public UnityGameObjectPool()
        {
            p_baseObj = null;
        }
        #endregion

        #region 功能

        /// <summary>
        /// 生成一个对象并激活
        /// </summary>
        /// <param name="value">生成的对象</param>
        /// <returns>是否成功生成</returns>
        public override bool Generator(out GameObject value)
        {
            bool flag = base.Generator(out value);
            if (flag) value.SetActive(true);
            return flag;
        }
        /// <summary>
        /// 存入一个对象并禁用
        /// </summary>
        /// <param name="value">要存入的对象</param>
        /// <returns>是否存入</returns>
        public override bool Push(GameObject value)
        {
            bool flag = base.Push(value);
            if (flag) value.SetActive(false);
            return flag;
        }
        /// <summary>
        /// 将缓存对象全部取出并激活
        /// </summary>
        /// <returns>所有的缓存对象，若没有则为空</returns>
        public GameObject[] PopAll()
        {
            var st = p_stack;
            GameObject[] rarr = new GameObject[st.Count];
            int length = rarr.Length;

            int i;
            GameObject tg;
            for (i = 0; i < length; i++)
            {
                tg = p_stack.Pop();
                tg.SetActive(true);
                rarr[i] = tg;
            }

            return rarr;
        }

        #endregion

    }


}
