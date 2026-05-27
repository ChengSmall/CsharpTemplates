using System;
using System.Text;

using Ptr = Cheng.Memorys.Pointer32;

namespace Cheng.Memorys
{

    /// <summary>
    /// 一个32位指针，该指针在任何环境下都是4个字节大小
    /// </summary>
    public unsafe readonly struct Pointer32 : IEquatable<Pointer32>, IComparable<Pointer32>, IConvertible
    {

        #region 构造

        /// <summary>
        /// 初始化指针值
        /// </summary>
        /// <param name="ptr">要初始化的值</param>
        public Pointer32(void* ptr)
        {
            p_ptr = (uint)ptr;
        }

        /// <summary>
        /// 初始化指针值
        /// </summary>
        /// <param name="ptr">要初始化的值</param>
        public Pointer32(uint ptr)
        {
            p_ptr = ptr;
        }

        /// <summary>
        /// 初始化指针值
        /// </summary>
        /// <param name="ptr">要初始化的值</param>
        public Pointer32(int ptr)
        {
            p_ptr = (uint)ptr;
        }

        /// <summary>
        /// 初始化指针值
        /// </summary>
        /// <param name="ptr">要初始化的值</param>
        public Pointer32(IntPtr ptr)
        {
            p_ptr = (uint)ptr;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 表示空值的指针
        /// </summary>
        public static Pointer32 Null => default;

        internal readonly uint p_ptr;

        #endregion

        #region 功能

        #region 运算
        /// <summary>
        /// 将添加指定字节偏移后的指针返回
        /// </summary>
        /// <param name="offset">字节偏移</param>
        /// <returns></returns>
        public Pointer32 AddOffset(int offset)
        {
            return new Pointer32(p_ptr + (uint)offset);
        }

        /// <summary>
        /// 添加偏移
        /// </summary>
        /// <param name="p"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Pointer32 operator +(Pointer32 p, int offset)
        {
            return new Pointer32(p.p_ptr + (uint)offset);
        }
        /// <summary>
        /// 添加偏移
        /// </summary>
        /// <param name="p"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Pointer32 operator +(Pointer32 p, uint offset)
        {
            return new Pointer32(p.p_ptr + (uint)offset);
        }
        /// <summary>
        /// 减去偏移
        /// </summary>
        /// <param name="p"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Pointer32 operator -(Pointer32 p, int offset)
        {
            return new Pointer32(p.p_ptr - (uint)offset);
        }
        /// <summary>
        /// 减去偏移
        /// </summary>
        /// <param name="p"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Pointer32 operator -(Pointer32 p, uint offset)
        {
            return new Pointer32(p.p_ptr - (uint)offset);
        }

        public static implicit operator void*(Pointer32 p)
        {
            return (void*)p.p_ptr;
        }

        public static implicit operator Pointer32(void* p)
        {
            return new Pointer32(p);
        }

        public static explicit operator IntPtr(Pointer32 p)
        {
            return new IntPtr(p.p_ptr);
        }

        public static explicit operator Pointer32(IntPtr p)
        {
            return new Pointer32(p.ToPointer());
        }

        public static explicit operator long(Pointer32 p)
        {
            return p.p_ptr;
        }
        public static explicit operator ulong(Pointer32 p)
        {
            return p.p_ptr;
        }
        public static explicit operator int(Pointer32 p)
        {
            return (int)p.p_ptr;
        }
        public static explicit operator uint(Pointer32 p)
        {
            return p.p_ptr;
        }

        public static bool operator <(Pointer32 p1, Pointer32 p2)
        {
            return p1.p_ptr < p2.p_ptr;
        }

        public static bool operator >(Pointer32 p1, Pointer32 p2)
        {
            return p1.p_ptr > p2.p_ptr;
        }

        public static bool operator <=(Pointer32 p1, Pointer32 p2)
        {
            return p1.p_ptr <= p2.p_ptr;
        }

        public static bool operator >=(Pointer32 p1, Pointer32 p2)
        {
            return p1.p_ptr >= p2.p_ptr;
        }

        public static bool operator ==(Pointer32 p1, Pointer32 p2)
        {
            return p1.p_ptr == p2.p_ptr;
        }

        public static bool operator !=(Pointer32 p1, Pointer32 p2)
        {
            return p1.p_ptr != p2.p_ptr;
        }


        #endregion

        #region 操作

        /// <summary>
        /// 将指针解引用并设置为新值
        /// </summary>
        /// <typeparam name="T">要访问内存所在的结构类型</typeparam>
        /// <param name="value">要设置的值</param>
        public void Defer<T>(T value) where T : unmanaged
        {
            *((T*)p_ptr) = value;
        }

        /// <summary>
        /// 将指针解引用并返回
        /// </summary>
        /// <typeparam name="T">要访问内存所在的结构类型</typeparam>
        /// <returns>返回指针指向的地址的值</returns>
        public T Defer<T>() where T : unmanaged
        {
            return *((T*)p_ptr);
        }

        /// <summary>
        /// 将指针解引用并返回引用对象
        /// </summary>
        /// <typeparam name="T">要访问内存所在的结构类型</typeparam>
        /// <returns>返回指针指向的地址引用</returns>
        public ref T DeRef<T>() where T : unmanaged
        {
            return ref *((T*)p_ptr);
        }

        /// <summary>
        /// 将指针以指针为操作内存解引用
        /// </summary>
        /// <returns>指针指向的地址值</returns>
        public Ptr DefPtr()
        {
            return *((Pointer32*)p_ptr);
        }

        /// <summary>
        /// 将指针以指针为操作内存解引用
        /// </summary>
        /// <param name="ptr">要在指针指向的地址设置的新值</param>
        public void DefPtr(Ptr ptr)
        {
            *((Pointer32*)p_ptr) = ptr;
        }

        /// <summary>
        /// 当前指针是否是空指针
        /// </summary>
        public bool IsEmpty
        {
            get => p_ptr == 0;
        }

        /// <summary>
        /// 返回当前指针的4字节值
        /// </summary>
        /// <returns></returns>
        public uint ToValue
        {
            get => p_ptr;
        }

        /// <summary>
        /// 返回当前指针的值
        /// </summary>
        /// <returns></returns>
        public void* ToPtr
        {
            get => (void*)p_ptr;
        }

        /// <summary>
        /// 返回系统通用句柄指针
        /// </summary>
        public IntPtr ToIntPtr
        {
            get => new IntPtr(p_ptr);
        }

        /// <summary>
        /// 返回添加特定类型的偏移后的新指针
        /// </summary>
        /// <typeparam name="T">偏移类型</typeparam>
        /// <param name="value">偏移量，每单位偏移字节按类型大小计算</param>
        /// <returns>新的指针</returns>
        public Ptr AddOffset<T>(int value) where T : unmanaged
        {
            return new Ptr(p_ptr + (uint)(sizeof(T) * value));
        }

        #endregion

        #endregion

        #region 派生

        public bool Equals(Ptr other)
        {
            return p_ptr == other.p_ptr;
        }

        public override bool Equals(object obj)
        {
            if(obj is Pointer32 p)
            {
                return p_ptr == p.p_ptr;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)p_ptr;
        }

        /// <summary>
        /// 以大写16进制形式返回地址值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            char* crp = stackalloc char[8]; //0xFFFFFFFF
            Cheng.Texts.TextManipulation.ValueToFixedX16Text(p_ptr, true, crp);
            return new string(crp, 0, 8);
            //uint value = this.p_ptr;
            //uint b;
            //StringBuilder sb = new StringBuilder(8);

            //Loop:
            //b = (value % 16);
            //char c;
            //if (b < 10)
            //{
            //    c = (char)('0' + b);
            //}
            //else
            //{
            //    b -= 10;
            //    c = (char)('A' + b);
            //}

            //sb.Insert(0, c);

            //if (value < 16)
            //{
            //    return sb.ToString();
            //}

            //value /= 16;
            //goto Loop;
        }

        /// <summary>
        /// 使用指定的格式将此实例的数值转换为它的等效字符串表示形式
        /// </summary>
        /// <param name="format">一个数值格式字符串</param>
        /// <returns>此实例的值的字符串表示形式，由<paramref name="format"/>指定</returns>
        /// <exception cref="FormatException">格式参数无效</exception>
        public string ToString(string format)
        {
            return p_ptr.ToString(format);
        }

        public int CompareTo(Ptr other)
        {
            return p_ptr < other.p_ptr ? -1 : (p_ptr == other.p_ptr ? 0 : 1);
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.UInt32;
        }

        #region
        public string ToString(IFormatProvider provider)
        {
            return p_ptr.ToString(provider);
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToBoolean(provider);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToChar(provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToSByte(provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToByte(provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToInt16(provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToUInt16(provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToInt32(provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToUInt32(provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToInt64(provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToUInt64(provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToSingle(provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToDouble(provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToDecimal(provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToDateTime(provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)p_ptr).ToType(conversionType, provider);
        }
        #endregion

        #endregion

    }
}
