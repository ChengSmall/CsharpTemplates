

namespace Cheng.Memorys
{
    /// <summary>
    /// 使用析构函数安全管理释放非托管资源
    /// </summary>
    /// <remarks>
    /// <para>继承此类，能够自动管理和释放非托管资源</para>
    /// </remarks>
    public abstract class ReleaseDestructor : SafreleaseUnmanagedResources
    {

        /// <summary>
        /// 析构函数用于保证在实例回收前清理非托管资源
        /// </summary>
        ~ReleaseDestructor()
        {
            Dispose(false);
        }
    }

}
