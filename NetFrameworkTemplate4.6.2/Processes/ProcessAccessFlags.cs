

namespace Cheng.Windows.Processes
{

    /// <summary>
    /// 进程访问权限
    /// </summary>
    public enum ProcessAccessFlags : uint
    {

        /// <summary>
        /// 所有权限
        /// </summary>
        All = 0x001F0FFF,

        /// <summary>
        /// 允许终止进程
        /// </summary>
        Terminate = 0x00000001,

        /// <summary>
        /// 可创建线程
        /// </summary>
        CreateThread = 0x00000002,

        /// <summary>
        /// 需要对进程的地址空间执行操作
        /// </summary>
        Operation = 0x00000008,
        /// <summary>
        /// 可读取进程内存
        /// </summary>
        Read = 0x00000010,
        /// <summary>
        /// 可修改进程内存
        /// </summary>
        Write = 0x00000020,

        /// <summary>
        /// 可复制句柄
        /// </summary>
        DupHandle = 0x00000040,
        /// <summary>
        /// 需要设置有关进程的某些信息，例如其优先级类
        /// </summary>
        SetInformation = 0x00000200,
        /// <summary>
        /// 检索有关进程的某些信息（例如其令牌、退出代码和优先级类）
        /// </summary>
        QueryInformation = 0x00000400,
        /// <summary>
        /// 需要使用等待函数等待进程终止
        /// </summary>
        Synchronize = 0x00100000,
        /// <summary>
        /// 可删除的进程对象
        /// </summary>
        Delete = 0x10000,

        /// <summary>
        /// 表示可对进程内存读写修改的权限
        /// </summary>
        MemoryModification = Operation | Read | Write

    }

}
