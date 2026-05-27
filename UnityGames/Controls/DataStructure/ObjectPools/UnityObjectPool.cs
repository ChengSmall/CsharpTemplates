using Cheng.DataStructure.ObjectPools;

namespace Cheng.Unitys.ObjectPools
{

    /// <summary>
    /// Unity对象池
    /// </summary>
    /// <typeparam name="T">Unity对象</typeparam>
    public class UnityObjectPool<T> : ObjectPool<T> where T : UnityEngine.Object
    {

        #region 构造
        /// <summary>
        /// 实例化一个Unity对象池
        /// </summary>
        public UnityObjectPool()
        {
            f_init();
        }
        /// <summary>
        /// 实例化一个Unity对象池
        /// </summary>
        /// <param name="baseObject">指定对象生成样本</param>
        public UnityObjectPool(T baseObject)
        {
            p_baseObj = baseObject;
            f_init();
        }

        private void f_init()
        {
            this.p_createObj = f_createObj;
        }

        #endregion

        #region 参数
        /// <summary>
        /// 对象生成样本
        /// </summary>
        protected T p_baseObj;
        #endregion

        #region 封装
        /// <summary>
        /// 对象生成器
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual bool f_createObj(out T obj)
        {
            obj = UnityEngine.Object.Instantiate(p_baseObj);
            return true;
        }
        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 访问或设置对象生成样本
        /// </summary>
        public T BaseObject
        {
            get => p_baseObj;
            set => p_baseObj = value;
        }

        /// <summary>
        /// 此功能无效
        /// </summary>
        /// <param name="func"></param>
        public override void SetObjectGenerator(ObjectGenerator<T> func)
        {
        }

        #endregion

        #endregion

    }

}
