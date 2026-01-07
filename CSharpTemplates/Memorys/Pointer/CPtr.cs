using Cheng.Algorithm.HashCodes;
using System;
using System.Text;

namespace Cheng.Memorys
{

    /// <summary>
    /// 指针对象
    /// </summary>
    [Serializable]
    public unsafe readonly struct CPtr : IEquatable<CPtr>, IHashCode64, IComparable<CPtr>, IFormattable
    {

        #region 初始化

        /// <summary>
        /// 初始化指针对象
        /// </summary>
        /// <param name="pointer">要初始胡的指针值</param>
        public CPtr(void* pointer)
        {
            this.p_ptr = pointer;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 表示空值的指针
        /// </summary>
        public static CPtr Null => default;

        /// <summary>
        /// 获取当前进程内指针对象占用的字节大小
        /// </summary>
        public static int Size => sizeof(void*);

        internal readonly void* p_ptr;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取内部封装的指针
        /// </summary>
        /// <returns></returns>
        public void* ToPointer() => p_ptr;

        /// <summary>
        /// 该指针是否为表示null的空指针
        /// </summary>
        public bool IsEmpty => p_ptr == null;

        #endregion

        #region 解引用

        /// <summary>
        /// 解引用并转换到引用对象
        /// </summary>
        /// <typeparam name="T">要使其访问的非托管类型</typeparam>
        /// <param name="offset">要添加的类型偏移量</param>
        /// <returns>引用对象</returns>
        public ref T Ref<T>(long offset) where T : unmanaged
        {
            return ref *(((T*)p_ptr) + offset);
        }

        /// <summary>
        /// 解引用并转换到引用对象
        /// </summary>
        /// <typeparam name="T">要使其访问的非托管类型</typeparam>
        /// <param name="offset">要添加的类型偏移量</param>
        /// <returns>引用对象</returns>
        public ref T Ref<T>(int offset) where T : unmanaged
        {
            return ref *(((T*)p_ptr) + offset);
        }

        /// <summary>
        /// 解引用并转换到引用对象
        /// </summary>
        /// <typeparam name="T">要使其访问的非托管类型</typeparam>
        /// <returns>引用对象</returns>
        public ref T Ref<T>() where T : unmanaged
        {
            return ref *((T*)p_ptr);
        }

        #endregion

        #region 运算符重载

        /// <summary>
        /// 添加指定字节偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要添加的字节偏移</param>
        /// <returns></returns>
        public static CPtr operator +(CPtr p1, int offset)
        {
            return new CPtr((byte*)p1.p_ptr + offset);
        }

        /// <summary>
        /// 减少指定字节偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要减少的字节偏移</param>
        /// <returns></returns>
        public static CPtr operator -(CPtr p1, int offset)
        {
            return new CPtr((byte*)p1.p_ptr - offset);
        }

        /// <summary>
        /// 添加指定字节偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要添加的字节偏移</param>
        /// <returns></returns>
        public static CPtr operator +(CPtr p1, long offset)
        {
            return new CPtr((byte*)p1.p_ptr + offset);
        }

        /// <summary>
        /// 减少指定字节偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要减少的字节偏移</param>
        /// <returns></returns>
        public static CPtr operator -(CPtr p1, long offset)
        {
            return new CPtr((byte*)p1.p_ptr - offset);
        }

        /// <summary>
        /// 添加指定字节偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要添加的字节偏移</param>
        /// <returns></returns>
        public static CPtr operator +(CPtr p1, uint offset)
        {
            return new CPtr((byte*)p1.p_ptr + offset);
        }

        /// <summary>
        /// 减少指定字节偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要减少的字节偏移</param>
        /// <returns></returns>
        public static CPtr operator -(CPtr p1, uint offset)
        {
            return new CPtr((byte*)p1.p_ptr - offset);
        }

        /// <summary>
        /// 添加指定字节偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要添加的字节偏移</param>
        /// <returns></returns>
        public static CPtr operator +(CPtr p1, ulong offset)
        {
            return new CPtr((byte*)p1.p_ptr + offset);
        }

        /// <summary>
        /// 减少指定字节偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要减少的字节偏移</param>
        /// <returns></returns>
        public static CPtr operator -(CPtr p1, ulong offset)
        {
            return new CPtr((byte*)p1.p_ptr - offset);
        }

        /// <summary>
        /// 添加一个字节偏移
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static CPtr operator ++(CPtr p)
        {
            return new CPtr((byte*)p.p_ptr + 1);
        }

        /// <summary>
        /// 减少一个字节偏移
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static CPtr operator --(CPtr p)
        {
            return new CPtr((byte*)p.p_ptr - 1);
        }

        public static bool operator ==(CPtr a, CPtr b)
        {
            return a.p_ptr == b.p_ptr;
        }

        public static bool operator !=(CPtr a, CPtr b)
        {
            return a.p_ptr != b.p_ptr;
        }

        public static bool operator <(CPtr a, CPtr b)
        {
            return a.p_ptr < b.p_ptr;
        }

        public static bool operator >(CPtr a, CPtr b)
        {
            return a.p_ptr > b.p_ptr;
        }

        public static bool operator <=(CPtr a, CPtr b)
        {
            return a.p_ptr <= b.p_ptr;
        }

        public static bool operator >=(CPtr a, CPtr b)
        {
            return a.p_ptr >= b.p_ptr;
        }

        #endregion

        #region 转换

        public static implicit operator void*(CPtr cp)
        {
            return cp.p_ptr;
        }

        public static implicit operator CPtr(void* ptr)
        {
            return new CPtr(ptr);
        }

        public static implicit operator IntPtr(CPtr cp)
        {
            return new IntPtr(cp.p_ptr);
        }

        public static implicit operator CPtr(IntPtr ptr)
        {
            return new CPtr(ptr.ToPointer());
        }

        public static implicit operator UIntPtr(CPtr cp)
        {
            return new UIntPtr(cp.p_ptr);
        }

        public static implicit operator CPtr(UIntPtr ptr)
        {
            return new CPtr(ptr.ToPointer());
        }

        public static explicit operator Pointer32(CPtr cp)
        {
            return new Pointer32(cp.p_ptr);
        }

        public static explicit operator CPtr(Pointer32 cp)
        {
            return new CPtr((void*)cp.p_ptr);
        }

        public static implicit operator Pointer64(CPtr cp)
        {
            return new Pointer64(cp.p_ptr);
        }

        public static explicit operator CPtr(Pointer64 cp)
        {
            return new CPtr((void*)cp.p_ptr);
        }

        public static explicit operator CPtr(int value)
        {
            return new CPtr((void*)value);
        }

        public static implicit operator CPtr(uint value)
        {
            return new CPtr((void*)value);
        }

        public static explicit operator CPtr(long value)
        {
            return new CPtr((void*)value);
        }

        public static explicit operator CPtr(ulong value)
        {
            return new CPtr((void*)value);
        }

        public static explicit operator int(CPtr ptr)
        {
            return (int)ptr.p_ptr;
        }

        public static explicit operator uint(CPtr ptr)
        {
            return (uint)ptr.p_ptr;
        }

        public static explicit operator long(CPtr ptr)
        {
            return (long)ptr.p_ptr;
        }

        public static implicit operator ulong(CPtr ptr)
        {
            return (ulong)ptr.p_ptr;
        }

        #endregion

        #region 派生

        private string f_toStrDef()
        {
            if (p_ptr == null) return "null";
            if (sizeof(void*) == 4)
            {
                char* cp = stackalloc char[8];
                Cheng.Texts.TextManipulation.ValueToFixedX16Text((uint)p_ptr, true, cp);
                return new string(cp, 0, 8);
            }

            char* cps = stackalloc char[16];
            Cheng.Texts.TextManipulation.ValueToFixedX16Text((ulong)p_ptr, true, cps);
            return new string(cps, 0, 16);
        }

        /// <summary>
        /// 返回指针的字符串格式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return f_toStrDef();
        }

        public bool Equals(CPtr other)
        {
            return p_ptr == other.p_ptr;
        }

        public override bool Equals(object obj)
        {
            if (obj is CPtr cp) return p_ptr == cp.p_ptr;
            return false;
        }

        public override int GetHashCode()
        {
            if (sizeof(void*) == 4) return (int)p_ptr;
            return ((ulong)p_ptr).GetHashCode();
        }

        public long GetHashCode64()
        {
            return (long)p_ptr;
        }

        public int CompareTo(CPtr other)
        {
            return (this.p_ptr == other.p_ptr) ? 0 : (this.p_ptr < other.p_ptr ? -1 : 1);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format is null)
            {
                if(formatProvider is null)
                {
                    return f_toStrDef();
                }
                //format = "X";
            }
            if (format != null)
                if (format.Length > 0 && (format[0] == 'G' || format[0] == 'g'))
                {
                    var cars = format.ToCharArray();
                    cars[0] = 'X';
                    format = new string(cars);
                }

            if (sizeof(void*) == 4)
            {
                return ((uint)p_ptr).ToString(format, formatProvider);
            }
            return ((ulong)p_ptr).ToString(format, formatProvider);
        }

        /// <summary>
        /// 使用指定格式对当前实例的值设置格式
        /// </summary>
        /// <param name="format">要使用的格式； null表示使用默认格式</param>
        /// <returns>采用指定格式的当前实例的值</returns>
        public string ToString(string format) => ToString(format, null);

        /// <summary>
        /// 使用指定格式对当前实例的值设置格式
        /// </summary>
        /// <param name="formatProvider">要用于格式化值的提供程序； null表示使用默认的格式化模式</param>
        /// <returns>采用指定格式的当前实例的值</returns>
        public string ToString(IFormatProvider formatProvider) => ToString(null, formatProvider);

        #endregion

        #endregion

    }



}
