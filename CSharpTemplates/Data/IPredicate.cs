

namespace Cheng.DataStructure
{

    /// <summary>
    /// 表示一个谓词结构公共接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPredicate<in T>
    {
        /// <summary>
        /// 谓词函数
        /// </summary>
        /// <param name="obj">检索的参数</param>
        /// <returns>是否匹配条件</returns>
        bool Predicate(T obj);
    }

}
