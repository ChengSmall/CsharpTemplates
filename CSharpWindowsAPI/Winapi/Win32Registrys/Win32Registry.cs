using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Cheng.Windows.Registrys
{

    /// <summary>
    /// 注册表预定义的主键句柄值（HKEY）
    /// </summary>
    public enum RegHandleKey : uint
    {

        /// <summary>
        /// 空
        /// </summary>
        None = 0,

        /// <summary>
        /// 预定义值 <b>HKEY_CLASSES_ROOT</b>
        /// </summary>
        ClassesRoot = 0x80000000,

        /// <summary>
        /// 预定义值 <b>HKEY_CURRENT_USER</b>
        /// </summary>
        CurrentUser = 0x80000001,

        /// <summary>
        /// 预定义值 <b>HKEY_LOCAL_MACHINE</b>
        /// </summary>
        LocalMachine = 0x80000002,

        /// <summary>
        /// 预定义值 <b>HKEY_USERS</b>
        /// </summary>
        Users = 0x80000003,

        /// <summary>
        /// 预定义值 <b>HKEY_CURRENT_CONFIG</b>
        /// </summary>
        CurrentConfig = 0x80000005
    }

    /// <summary>
    /// 注册表项访问权限（REGSAM）
    /// </summary>
    [Flags]
    public enum RegSam : uint
    {

        /// <summary>
        /// 查询注册表项的值所必需的
        /// </summary>
        QueryValue = 0x0001,

        /// <summary>
        /// 创建、删除或设置注册表值所必需的
        /// </summary>
        SetValue = 0x0002,

        /// <summary>
        /// 创建注册表项的子项是必需的
        /// </summary>
        CreateSubKey = 0x0004,

        /// <summary>
        /// 枚举注册表项的子项是必需的
        /// </summary>
        EnumerateSubKey = 0x0008,

        /// <summary>
        /// 请求注册表项或注册表项子项的更改通知
        /// </summary>
        Notify = 0x0010,

        /// <summary>
        /// 指示 64 位 Windows 上的应用程序应在 64 位注册表视图中运行
        /// </summary>
        WOW64Key64 = 0x0100,

        /// <summary>
        /// 指示 64 位 Windows 上的应用程序应在 32 位注册表视图中运行
        /// </summary>
        WOW64Key32 = 0x0200,

        /// <summary>
        /// 复合掩码 用于读取注册表
        /// </summary>
        Read = 0x20019,

        /// <summary>
        /// 复合掩码 用于修改注册表
        /// </summary>
        Write = 0x20006,

        /// <summary>
        /// 所有权限
        /// </summary>
        ALL = 0xF003F,

        /// <summary>
        /// 保留供系统使用
        /// </summary>
        CREATE_LINK = 0x0020,
    }

    /// <summary>
    /// 注册表值类型
    /// </summary>
    public enum RegValueType : uint
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 字符串
        /// </summary>
        /// <remarks>
        /// <para>字符串类型，但不保证结尾有'\0'值</para>
        /// <para>字符串类型在ANSI版本函数返回ANSI字符串，在Unicode版本返回UTF16字符串</para>
        /// </remarks>
        String = 1,

        /// <summary>
        /// 无环境变量展开的字符串
        /// </summary>
        /// <remarks>
        /// <para>字符串类型，但不保证结尾有'\0'值</para>
        /// <para>该类型不会自动展开环境变量占位符</para>
        /// <para>字符串类型在ANSI版本函数返回ANSI字符串，在Unicode版本返回UTF16字符串</para>
        /// </remarks>
        ExpandString = 2,

        /// <summary>
        /// 原始字节序列
        /// </summary>
        /// <remarks>
        /// <para>一块原始二进制字节序列</para>
        /// </remarks>
        Binary = 3,

        /// <summary>
        /// 32位(4字节)无符号整数
        /// </summary>
        DWORD = 4,

        /// <summary>
        /// 64位(8字节)无符号整数
        /// </summary>
        QWORD = 0xB,

        /// <summary>
        /// 大端序32位(4字节)无符号整数
        /// </summary>
        DWORD_BigEndian = 5,

        /// <summary>
        /// 一个 Unicode 符号链接
        /// </summary>
        /// <remarks>
        /// <para>一个以 '\0' 结尾的 UTF16 字符串，其中包含通过使用 REG_OPTION_CREATE_LINK 调用 RegCreateKeyEx 函数创建的符号链接的目标路径</para>
        /// </remarks>
        Link = 6,

        /// <summary>
        /// 以'\0'结尾的字符串序列
        /// </summary>
        /// <remarks>
        /// <para>
        /// 该类型是包含多个字符串的列表，每一个'\0'结束一个字符串值，当在一个字符串的结尾'\0'之后还有一个'\0'表示列表结束<br/>
        /// 格式如下:
        /// <code>"字符串1\0字符串2\0字符串3\0\0"</code>
        /// </para>
        /// </remarks>
        MultString = 7,

        /// <summary>
        /// 设备驱动程序资源列表（系统级保留类型）
        /// </summary>
        /// <remarks>
        /// <para>复杂的二进制结构，数据需要按照 CM_RESOURCE_LIST 结构进行解析，该结构描述了一个设备已经分配到的系统硬件资源集合</para>
        /// <para>此类型不复合向下兼容性api，不建议使用</para>
        /// </remarks>
        ResorceList = 8,

        /// <summary>
        /// 物理设备正在使用的硬件资源列表（系统级保留类型）
        /// </summary>
        /// <remarks>
        /// <para>复杂的二进制结构，数据需要按照 CM_FULL_RESOURCE_DESCRIPTOR 结构进行解析，用于描述特定总线（如 PCI）上某个设备所拥有的完整硬件资源集</para>
        /// <para>此类型不复合向下兼容性api，不建议使用</para>
        /// </remarks>
        FullResourceDescriptor = 9,

        /// <summary>
        /// 设备驱动程序的可能硬件资源列表（系统级保留类型）
        /// </summary>
        /// <remarks>
        /// <para>复杂的二进制结构，数据需要按照 IO_RESOURCE_REQUIREMENTS_LIST 结构进行解析，描述了一个设备可以使用的所有可能的硬件资源范围</para>
        /// <para>此类型不复合向下兼容性api，不建议使用</para>
        /// </remarks>
        ResourceRequirementsList = 0xA
    }

    /// <summary>
    /// win32 注册表接口
    /// </summary>
    public static unsafe class WindowsRegistryAPI
    {

        #region api

        private const string Advapi32 = "advapi32.dll";

        /// <summary>
        /// （win32原生api）打开指定的注册表项
        /// </summary>
        /// <param name="hKey">已打开的主键句柄（HKEY），可将<see cref="RegHandleKey"/>值转换为地址值</param>
        /// <param name="lpSubKey">要打开的子项路径</param>
        /// <param name="ulOptions">保留参数，必须为 0</param>
        /// <param name="samDesired">访问权限，使用 <see cref="RegSam"/> 枚举</param>
        /// <param name="phkResult">指向指针变量的地址，返回打开项句柄的缓冲区指针（PHKEY）</param>
        /// <returns>成功返回 0（ERROR_SUCCESS），失败返回 win32 错误码</returns>
        [DllImport(Advapi32, EntryPoint = "RegOpenKeyExW")]
        public static extern uint api_RegOpenKeyEx(void* hKey, char* lpSubKey, uint ulOptions, uint samDesired, void* phkResult);

        /// <summary>
        /// （win32原生api）读取指定注册表项的值数据。
        /// </summary>
        /// <param name="hKey">已打开的注册表项句柄（HKEY）</param>
        /// <param name="lpValueName">值名称，宽字符串指针 char*（对应 LPCWSTR），空字符串表示默认值</param>
        /// <param name="lpReserved">保留参数，必须为 null</param>
        /// <param name="lpType">指向32位整数值的指针，该整数返回读取的数据类型；整数可转化为<see cref="RegValueType"/>枚举值；null表示不需要接收类型值</param>
        /// <param name="lpData">接收数据的字节缓冲区地址，如果不需要数据可为 null</param>
        /// <param name="lpcbData">
        /// <para>这是一个输入输出参数，输入时表示缓冲区<paramref name="lpData"/>可用的字节大小，输出时返回读取的数据整体字节大小</para>
        /// <para>如果值是字符串类型，则返回的数据大小不保证结尾存在 '\0'，需严格按照字节大小定义数据范围</para>
        /// <para>可以将<paramref name="lpData"/>设为null，然后用该参数只获取值的大小</para>
        /// <para>该参数可以设为null，但前提<paramref name="lpData"/>参数必须也是null，当<paramref name="lpData"/>和<paramref name="lpcbData"/>全部是null时，该函数仅判断指定的注册表项是否存在</para>
        /// </param>
        /// <returns>成功返回 0；否则返回错误码，使用<see cref="Cheng.Windows.Win32API.ErrorCodeAPI.GetLastError"/>获取错误码</returns>
        [DllImport(Advapi32, EntryPoint = "RegQueryValueExW")]
        public static extern uint api_RegQueryValueEx(void* hKey, char* lpValueName, uint* lpReserved, void* lpType, void* lpData, uint* lpcbData);

        /// <summary>
        /// （win32原生api）关闭注册表项句柄。
        /// </summary>
        /// <param name="hKey">要关闭的注册表项句柄（HKEY）</param>
        /// <returns>成功返回 0，失败返回非零错误码</returns>
        [DllImport(Advapi32, EntryPoint = "RegCloseKey")]
        public static extern uint api_RegCloseKey(void* hKey);

        #endregion

    }

}
