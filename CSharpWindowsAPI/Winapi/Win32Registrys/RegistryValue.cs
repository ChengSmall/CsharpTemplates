using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ComponentModel;

using Cheng.Streams;
using Cheng.DataStructure.Collections;

namespace Cheng.Windows.Registrys
{

    /// <summary>
    /// 表示注册表项的值
    /// </summary>
    public unsafe abstract class RegistryValue
    {

        #region 参数

        /// <summary>
        /// 注册表值的类型，<see cref="RegValueType.None"/>表示错误值
        /// </summary>
        public abstract RegValueType ValueType { get; }

        /// <summary>
        /// 4字节整数
        /// </summary>
        /// <value>
        /// <para>4字节无符号整数的注册表值，仅当类型是<see cref="RegValueType.DWORD"/>或<see cref="RegValueType.DWORD_BigEndian"/>时有效；对象会自动将<see cref="RegValueType.DWORD_BigEndian"/>类型值转换为等效的<see cref="uint"/>值</para>
        /// </value>
        /// <exception cref="NotSupportedException">类型不符</exception>
        public virtual uint DWORD => throw new NotSupportedException();

        /// <summary>
        /// 8字节整数
        /// </summary>
        /// <value>
        /// <para>8字节无符号整数的注册表值，仅当类型是<see cref="RegValueType.QWORD"/>、<see cref="RegValueType.DWORD"/>或<see cref="RegValueType.DWORD_BigEndian"/>时有效</para>
        /// <para>对象会自动将其它类型的值转换为等效的<see cref="ulong"/>类型值</para>
        /// </value>
        /// <exception cref="NotSupportedException">类型不符</exception>
        public virtual ulong QWORD => throw new NotSupportedException();

        /// <summary>
        /// 获取整数值
        /// </summary>
        /// <value>仅当参数类型是整数类型时有效</value>
        /// <exception cref="NotSupportedException">注册表值类型不是整数类型</exception>
        public virtual long Intger
        {
            get
            {
                switch (ValueType)
                {
                    case RegValueType.DWORD:
                        return DWORD;
                    case RegValueType.QWORD:
                        return (long)QWORD;
                    case RegValueType.DWORD_BigEndian:
                        return DWORD;
                }
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 字符串值
        /// </summary>
        /// <value>
        /// <para>表示字符串的注册表参数，仅当类型是<see cref="RegValueType.String"/>，<see cref="RegValueType.ExpandString"/>，<see cref="RegValueType.Link"/>时可用</para>
        /// <para>当类型是<see cref="RegValueType.MultString"/>时，该值表示原始格式的字符串值</para>
        /// </value>
        /// <exception cref="NotSupportedException">类型不符</exception>
        public virtual string StringValue => throw new NotSupportedException();

        /// <summary>
        /// 字符串列表
        /// </summary>
        /// <value>
        /// <para>表示字符串列表的注册表参数，仅当类型是<see cref="RegValueType.MultString"/>时可用</para>
        /// </value>
        /// <exception cref="NotSupportedException">类型不符</exception>
        public virtual IReadOnlyList<string> StringList
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 二进制序列数据
        /// </summary>
        /// <value>
        /// <para>表示原始二进制数据，当类型是<see cref="RegValueType.Binary"/>或其它无法用基本类型表示的数据时可用</para>
        /// <para>对象支持随机读取</para>
        /// </value>
        /// <exception cref="NotSupportedException">类型不符</exception>
        public virtual IReadOnlyList<byte> Binrary => throw new NotSupportedException();

        /// <summary>
        /// 二进制序列的字节大小
        /// </summary>
        /// <value>表示原始二进制数据的字节大小，如果类型不是二进制数据则为-1</value>
        public virtual int BinrarySize => -1;

        /// <summary>
        /// 读取二进制数据到指定缓冲区
        /// </summary>
        /// <param name="buffer">指向要写入的缓冲区</param>
        /// <param name="size">缓冲区可用字节容量</param>
        /// <exception cref="NotSupportedException">类型不是二进制数据</exception>
        public virtual void GetBinraryBuffer(byte* buffer, int size)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 读取二进制数据到指定缓冲区
        /// </summary>
        /// <param name="buffer">要写入的缓冲区</param>
        /// <param name="offset">缓冲区起始偏移</param>
        /// <param name="count">要写入缓冲区的最大字节数</param>
        /// <exception cref="ArgumentException">参数小于0</exception>
        /// <exception cref="NotSupportedException">类型不是二进制数据</exception>
        public virtual void GetBinraryBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || count < 0 || (count + offset >= buffer.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            fixed (byte* bptr = buffer)
            {
                GetBinraryBuffer(bptr + offset, count);
            }
        }

        /// <summary>
        /// 创建二进制数据内容的流对象
        /// </summary>
        /// <returns></returns>
        public virtual Stream CreateBinraryStream()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 仅当类型是整数且值非0时为true
        /// </summary>
        public virtual bool IsIntgerNotZero => false;

        public override string ToString()
        {
            return ValueType.ToString();
        }

        #endregion

        #region 功能

        #region 读取

        /// <summary>
        /// 通过预定义主键和注册表项获取值
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="itemPath">注册表项路径</param>
        /// <param name="valueName">注册表项值名称</param>
        /// <returns>读取到的值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static RegistryValue GetRegistryValue(RegHandleKey key, string itemPath, string valueName)
        {
            return GetRegistryValue(key, itemPath, valueName, RegValueType.None, false, 0);
        }

        /// <summary>
        /// 通过预定义主键和注册表项获取值
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="itemPath">注册表项路径</param>
        /// <param name="valueName">注册表项值名称</param>
        /// <param name="type">
        /// <para>设置要限制匹配的类型，如果类型不匹配，则引发<see cref="ArgumentException"/>异常</para>
        /// <para>如果该参数是<see cref="RegValueType.None"/>，则不限制类型</para>
        /// </param>
        /// <returns>读取到的值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数类型不匹配</exception>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static RegistryValue GetRegistryValue(RegHandleKey key, string itemPath, string valueName, RegValueType type)
        {
            return GetRegistryValue(key, itemPath, valueName, type, false, 0);
        }

        /// <summary>
        /// 通过预定义主键和注册表项获取值
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="itemPath">注册表项路径</param>
        /// <param name="valueName">注册表项值名称</param>
        /// <param name="type">
        /// <para>设置要限制匹配的类型，如果类型不匹配，则引发<see cref="ArgumentException"/>异常</para>
        /// <para>如果该参数是<see cref="RegValueType.None"/>，则不限制类型</para>
        /// </param>
        /// <param name="fuzzyMatching">
        /// <para>是否开启模糊匹配</para>
        /// <para>当该参数为true时，类型匹配参数会将所有整数视为一种类型，会将所有字符串(包括多集合字符串)视为一种类型</para>
        /// </param>
        /// <returns>读取到的值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数类型不匹配</exception>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static RegistryValue GetRegistryValue(RegHandleKey key, string itemPath, string valueName, RegValueType type, bool fuzzyMatching)
        {
            return GetRegistryValue(key, itemPath, valueName, type, fuzzyMatching, 0);
        }

        /// <summary>
        /// 通过预定义主键和注册表项获取值
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="itemPath">注册表项路径</param>
        /// <param name="valueName">注册表项值名称</param>
        /// <param name="type">
        /// <para>设置要限制匹配的类型，如果类型不匹配，则引发<see cref="ArgumentException"/>异常</para>
        /// <para>如果该参数是<see cref="RegValueType.None"/>，则不限制类型</para>
        /// </param>
        /// <param name="fuzzyMatching">
        /// <para>是否开启模糊匹配</para>
        /// <para>当该参数为true时，类型匹配参数会将所有整数视为一种类型，会将所有字符串(包括多集合字符串)视为一种类型</para>
        /// </param>
        /// <param name="sam">为访问注册表添加额外的权限，0表示不添加</param>
        /// <returns>读取到的值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数类型不匹配</exception>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static RegistryValue GetRegistryValue(RegHandleKey key, string itemPath, string valueName, RegValueType type, bool fuzzyMatching, RegSam sam)
        {
            if (itemPath is null || valueName is null) throw new ArgumentNullException();

            IntPtr regH = IntPtr.Zero;
            uint err;
            fixed (char* vp = itemPath)
            {
                err = WindowsRegistryAPI.api_RegOpenKeyEx((void*)((uint)key), vp, 0, (uint)(RegSam.Read | sam), &regH);
            }
            if (err != 0)
            {
                throw new Win32Exception((int)err);
            }
            try
            {
                RegValueType rt = RegValueType.None;
                uint reSize = 0;
                fixed (char* vp = valueName)
                {
                    err = WindowsRegistryAPI.api_RegQueryValueEx(regH.ToPointer(), vp, null,
                    &rt, null, &reSize);
                    if (err != 0)
                    {
                        throw new Win32Exception((int)err);
                    }

                    if (rt == RegValueType.None)
                    {
                        throw new ArgumentException();
                    }

                    if (rt != RegValueType.None)
                    {
                        if (fuzzyMatching)
                        {
                            switch (rt)
                            {
                                case RegValueType.DWORD:
                                case RegValueType.DWORD_BigEndian:
                                case RegValueType.QWORD:
                                    switch (type)
                                    {
                                        case RegValueType.DWORD:
                                        case RegValueType.DWORD_BigEndian:
                                        case RegValueType.QWORD:
                                            goto IfTypeOver;
                                        default:
                                            throw new ArgumentException();
                                    }
                                default:
                                    break;
                            }
                            switch (rt)
                            {
                                case RegValueType.String:
                                case RegValueType.ExpandString:
                                case RegValueType.MultString:
                                case RegValueType.Link:
                                    switch (type)
                                    {
                                        case RegValueType.String:
                                        case RegValueType.ExpandString:
                                        case RegValueType.MultString:
                                        case RegValueType.Link:
                                            goto IfTypeOver;
                                        default:
                                            throw new ArgumentException();
                                    }
                                default:
                                    break;
                            }
                        }

                        if (rt != type)
                        {
                            throw new ArgumentException();
                        }
                    }

                    IfTypeOver:
                    if (rt == RegValueType.DWORD)
                    {
                        uint turs;
                        reSize = 4;
                        err = WindowsRegistryAPI.api_RegQueryValueEx(
                            regH.ToPointer(), vp, null,
                    null, &turs, &reSize);
                        if (err != 0)
                        {
                            throw new Win32Exception((int)err);
                        }
                        return new RegValue_Int(turs, rt);
                    }
                    else if (rt == RegValueType.QWORD)
                    {
                        reSize = 8;
                        ulong ti;

                        err = WindowsRegistryAPI.api_RegQueryValueEx(
                            regH.ToPointer(), vp, null,
                    null, &ti, &reSize);
                        if (err != 0)
                        {
                            throw new Win32Exception((int)err);
                        }
                        return new RegValue_Int(ti, rt);
                    }
                    else if (rt == RegValueType.DWORD_BigEndian)
                    {
                        reSize = 4;
                        byte* bufptr = stackalloc byte[4];

                        err = WindowsRegistryAPI.api_RegQueryValueEx(
                            regH.ToPointer(), vp, null,
                    null, bufptr, &reSize);
                        if (err != 0)
                        {
                            throw new Win32Exception((int)err);
                        }

                        uint ti = (((uint)bufptr[0]) << 24) |
                            (((uint)bufptr[1]) << 16) |
                            (((uint)bufptr[2]) << 8) |
                            (((uint)bufptr[3]))
                            ;

                        return new RegValue_Int(ti, rt);
                    }

                    if (rt == RegValueType.String || rt == RegValueType.ExpandString || rt == RegValueType.MultString || rt == RegValueType.Link)
                    {
                        if (reSize == 0)
                        {
                            return new RegValue_Str(string.Empty, rt);
                        }
                        char[] cbuf = new char[reSize / 2];
                        fixed (char* cbufPtr = cbuf)
                        {
                            err = WindowsRegistryAPI.api_RegQueryValueEx(
                            regH.ToPointer(), vp, null,
                            null, cbufPtr, &reSize);
                            if (err != 0)
                            {
                                throw new Win32Exception((int)err);
                            }
                        }
                        return new RegValue_Str(new string(cbuf), rt);
                    }

                    byte[] bin = new byte[reSize];
                    fixed (byte* binptr = bin)
                    {
                        err = WindowsRegistryAPI.api_RegQueryValueEx(
                            regH.ToPointer(), vp, null,
                            null, binptr, &reSize);
                        if (err != 0)
                        {
                            throw new Win32Exception((int)err);
                        }
                    }
                    return new RegValue_Bin(bin, rt);

                }

            }
            finally
            {
                WindowsRegistryAPI.api_RegCloseKey(regH.ToPointer());
            }

        }

        #endregion

        #endregion

    }

    #region

    internal sealed class RegValue_Int : RegistryValue
    {

        #region 初始化

        internal RegValue_Int(ulong value, RegValueType type)
        {
            p_value = value;
            p_type = type;
        }

        private readonly ulong p_value;
        private readonly RegValueType p_type;

        #endregion

        #region 派生

        public override RegValueType ValueType => p_type;

        public override uint DWORD
        {
            get
            {
                if(p_type == RegValueType.DWORD || p_type == RegValueType.DWORD_BigEndian)
                {
                    return (uint)p_value;
                }
                throw new NotSupportedException();
            }
        }

        public override ulong QWORD
        {
            get
            {
                switch (p_type)
                {
                    case RegValueType.DWORD:
                    case RegValueType.QWORD:
                    case RegValueType.DWORD_BigEndian:
                        return p_value;
                }
                throw new NotSupportedException();
            }
        }

        public override bool IsIntgerNotZero => p_value != 0;

        public override long Intger => (long)p_value;

        public override string ToString()
        {
            return p_value.ToString();
        }

        #endregion

    }

    internal sealed class RegValue_Str : RegistryValue
    {

        #region 初始化

        static ArrayReadOnly<string> f_createList(string str)
        {
            int length = str.Length;
            if(length == 0)
            {
                return new ArrayReadOnly<string>(Array.Empty<string>());
            }
            List<string> listar = new List<string>();
            int first = 0, next = 0;
            bool loop = true;
            while ((next < length) && loop)
            {
                while (next < length)
                {
                    if(str[next] == '\0')
                    {
                        var subs = str.Substring(first, next - first);
                        listar.Add(subs);
                        next++;
                        if(next < length)
                        {
                            if(str[next] == '\0') loop = false;
                        }
                        first = next;
                        break;
                    }
                    next++;
                }
            }

            if(listar.Count == listar.Capacity)
            {
                return new ArrayReadOnly<string>(listar);
            }
            return new ArrayReadOnly<string>(listar.ToArray());
        }

        internal RegValue_Str(string str, RegValueType type)
        {
            p_str = str ?? string.Empty;
            p_type = type;
            if(type == RegValueType.MultString)
            {
                p_strList = f_createList(str);
            }
            else
            {
                p_strList = null;
            }
            
        }

        private readonly string p_str;
        private readonly RegValueType p_type;
        private ArrayReadOnly<string> p_strList;

        #endregion

        #region 派生

        public override RegValueType ValueType => p_type;

        public override string StringValue => p_str;

        public override IReadOnlyList<string> StringList
        {
            get
            {
                if(p_type == RegValueType.MultString) return p_strList;
                throw new NotSupportedException();
            }
        }

        public override string ToString()
        {
            return p_str;
        }

        #endregion

    }

    internal class RegValue_Bin : RegistryValue
    {

        #region 参数

        public RegValue_Bin(byte[] buf, RegValueType type)
        {
            p_buffer = buf;
            p_type = type;
            p_rbuf = new ArrayReadOnly<byte>(p_buffer);
        }

        private RegValueType p_type;
        private byte[] p_buffer;
        private ArrayReadOnly<byte> p_rbuf;

        #endregion

        #region 派生

        public override RegValueType ValueType => p_type;

        public override IReadOnlyList<byte> Binrary
        {
            get => p_rbuf;
        }

        public override int BinrarySize => p_buffer.Length;

        public override unsafe void GetBinraryBuffer(byte* buffer, int size)
        {
            if (buffer == null) throw new ArgumentNullException();
            if (size == 0) return;
            fixed (byte* op = p_buffer)
            {
                Cheng.Memorys.MemoryOperation.MemoryCopy(op, buffer, Math.Min(size, p_buffer.Length));
            }
        }

        public override Stream CreateBinraryStream()
        {
            return new MemoryStream(p_buffer, false);
        }

        public override void GetBinraryBuffer(byte[] buffer, int offset, int count)
        {
            base.GetBinraryBuffer(buffer, offset, count);
        }

        #endregion

    }

    #endregion

}
