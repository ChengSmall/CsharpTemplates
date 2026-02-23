using Cheng.Memorys;
using System;

namespace Cheng.Algorithm.HashCodes
{

    #region 接口

    /// <summary>
    /// 定义对象的64位HashCode的公共实现接口
    /// </summary>
    public interface IHashCode64
    {
        /// <summary>
        /// 获取此实例的64位哈希值
        /// </summary>
        /// <returns>实例的64位哈希值</returns>
        long GetHashCode64();
    }

    /// <summary>
    /// 定义获取对象的64位HashCode公共方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseHashCode64<T>
    {
        /// <summary>
        /// 获取指定对象的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>对象的64位哈希值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        long GetHashCode64(T value);
    }

    #endregion


    /// <summary>
    /// 获取对象的64位HashCode方法的公共基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseHashCode64<T> : IBaseHashCode64<T>
    {

        /// <summary>
        /// 获取指定对象的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>对象的64位哈希值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public abstract long GetHashCode64(T value);

        /// <summary>
        /// 获取 <typeparamref name="T"/> 类型的默认实现的64位HashCode
        /// </summary>
        /// <returns>
        /// <para>当 <typeparamref name="T"/> 类型作为<see cref="HashCode64"/>内实现的类型时，使用其方法获取值</para>
        /// <para>当 <typeparamref name="T"/> 类型是枚举类型时，使用其基础整数获取HashCode</para>
        /// <para>当 <typeparamref name="T"/> 类型是派生于<see cref="IHashCode64"/>接口时，使用接口方法获取HashCode</para>
        /// <para>参数为null时，始终返回0</para>
        /// <para>除此之外的类型，调用<see cref="object.GetHashCode"/>获取值</para>
        /// </returns>
        public static BaseHashCode64<T> Default
        {
            get => LazyObj.cp_def;
        }

        #region 封装

        private static class LazyObj
        {
            public readonly static BaseHashCode64<T> cp_def = f_createDefault();
        }

        private static BaseHashCode64<T> f_createDefault()
        {
            Type type = typeof(T);
            BaseHashCode64<T> hash = null;
            try
            {

                if (hash == null) hash = f_createInterface(type);

                if (hash == null) hash = f_creafeBaseType(type);

                if (hash == null) hash = f_createEnumValueType(type);

                if (hash == null) hash = f_createNullableValueType(type);

                return hash ?? f_createDefType(type);
            }
            catch (Exception)
            {
                return new DefaultHashCode();
            }
        }

        private static BaseHashCode64<T> f_createDefType(Type type)
        {
            if (type.IsValueType) return new DefaultHashCodeValue();
            return new DefaultHashCode();
        }

        private static BaseHashCode64<T> f_createInterface(Type type)
        {

            if(typeof(IHashCode64).IsAssignableFrom(type))
            {
                return f_createTypeInterface(type);
            }
            return null;
        }

        private static BaseHashCode64<T> f_createTypeInterface(Type otype)
        {
            //获取泛型实例类
            Type type;
         
            //type = typeof(TypeHashCode64<T>);

            try
            {
                bool isValue = otype.IsValueType;
                if (isValue)
                {
                    type = typeof(TypeHashCode64Value<>).MakeGenericType(otype);
                }
                else
                {
                    type = typeof(TypeHashCode64<>).MakeGenericType(otype);
                }
                
                return Activator.CreateInstance(type) as BaseHashCode64<T>;
            }
            catch (Exception)
            {
                return null;
                //return new DefaultHashCode();
            }
            
        }

        static BaseHashCode64<T> f_creafeBaseType(Type type)
        {

            if (type == typeof(int))
            {
                return (BaseHashCode64<T>)((object)new Int32HashCode64());

            }
            if (type == typeof(long))
            {
                return (BaseHashCode64<T>)((object)new Int64HashCode64());
            }

            if (type == typeof(string))
            {
                return (BaseHashCode64<T>)((object)new StringHashCode64());
            }


            if (type == typeof(float))
            {
                return (BaseHashCode64<T>)((object)new FloatHashCode64());
            }
            if (type == typeof(double))
            {
                return (BaseHashCode64<T>)((object)new DoubleHashCode64());
            }


            if (type == typeof(ulong))
            {
                return (BaseHashCode64<T>)((object)new UInt64HashCode64());
            }
            if (type == typeof(uint))
            {
                return (BaseHashCode64<T>)((object)new UInt32HashCode64());
            }
            if (type == typeof(byte))
            {
                return (BaseHashCode64<T>)((object)new ByteHashCode64());

            }
            if (type == typeof(sbyte))
            {
                return (BaseHashCode64<T>)((object)new SByteHashCode64());
            }

            if (type == typeof(short))
            {
                return (BaseHashCode64<T>)((object)new Int16HashCode64());

            }
            if (type == typeof(ushort))
            {
                return (BaseHashCode64<T>)((object)new UInt16HashCode64());
            }

            if (type == typeof(bool))
            {
                return (BaseHashCode64<T>)((object)new BooleanHashCode64());
            }
            if (type == typeof(char))
            {
                return (BaseHashCode64<T>)((object)new CharHashCode64());
            }
            if (type == typeof(decimal))
            {
                return (BaseHashCode64<T>)((object)new DecHashCode64());
            }

            if (type == typeof(DateTime))
            {
                return (BaseHashCode64<T>)((object)new DateTimeHashCode64());
            }
            if (type == typeof(TimeSpan))
            {
                return (BaseHashCode64<T>)((object)new TimeSpanHashCode64());
            }
            if(type == typeof(Guid))
            {
                return (BaseHashCode64<T>)((object)new GuidHashCode64());
            }

            return null;
        }

        private sealed class DefaultHashCode : BaseHashCode64<T>
        {
            public DefaultHashCode()
            {
            }
            public sealed override long GetHashCode64(T value)
            {
                return (value == null) ? throw new ArgumentNullException(nameof(value)) : value.GetHashCode();
            }
        }

        private sealed class DefaultHashCodeValue : BaseHashCode64<T>
        {
            public DefaultHashCodeValue()
            {
            }

            public sealed override long GetHashCode64(T value)
            {
                return value.GetHashCode();
            }
        }

        /// <summary>
        /// 创建可空值类型哈希获取类实例
        /// </summary>
        /// <param name="type">可空值类型的泛型具体类型</param>
        /// <returns></returns>
        private static BaseHashCode64<T> f_createNullableValueType(Type type)
        {

            try
            {
                //是值类型
                bool isValueType = type.IsValueType;
                //是基础泛型模板type
                bool isGenDef = type.IsGenericTypeDefinition;
                bool isGen = type.IsGenericType;
                if (isValueType && isGen && (!isGenDef))
                {
                    //是值类型且是实体泛型类

                    //获取泛型定义的类型实例
                    Type[] genTypes = type.GetGenericArguments();

                    //不是1个泛型定义参数判定失败
                    if (genTypes.Length != 1) return null;

                    //获取唯一type的泛型类型定义参数
                    Type genType = genTypes[0];

                    //泛型定义不是值类型
                    isValueType = genType.IsValueType;
                    if (!isValueType) return null;

                    //将判断类型泛型值定义到可空值类型类型实例
                    Type nullType;

                    nullType = typeof(Nullable<>).MakeGenericType(genTypes);

                    //判断拿到的反射类等同于T本身
                    if (nullType.Equals(type))
                    {
                        //type等于可空值类型

                        //构造可空值类型的哈希实例
                        type = typeof(NullableValueTypeHashCode<>).MakeGenericType(genTypes);
                        return Activator.CreateInstance(type) as BaseHashCode64<T>;
                    }

                }

            }
            catch (Exception)
            {
            }

            return null;
        }

        private static BaseHashCode64<T> f_createEnumValueType(Type type)
        {

            if (type.IsEnum)
            {
                type = typeof(EnumValueHashCode<>).MakeGenericType(type);
                return Activator.CreateInstance(type) as BaseHashCode64<T>;
            }
            return null;

        }

        #endregion

    }


    /// <summary>
    /// 基本类型的HashCode64获取扩展方法
    /// </summary>
    public unsafe static class HashCode64
    {

        #region 计算基础类型的HashCode

        #region

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this long value)
        {
            return value;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this ulong value)
        {
            return (long)value;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this int value)
        {
            return (long)value;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this uint value)
        {
            return (long)value;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this short value)
        {
            return (long)value;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this ushort value)
        {
            return (long)value;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this byte value)
        {
            return (long)value;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this sbyte value)
        {
            return (long)value;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this char value)
        {
            return value;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this bool value)
        {
            return value ? 1 : 0;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this float value)
        {
            return *(int*)&value;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this double value)
        {
            return *(long*)&value;
        }

        /// <summary>
        /// 获取实例的默认64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的默认64位哈希值</returns>
        public static long GetHashCode64(this decimal value)
        {
            int* ip = (int*)&value;

            long re;

            re = (((long)ip[1] | ((long)((ulong)ip[2] << 32))) ^ (long)ip[3]);

            re |= ((*(uint*)ip) >> 16);

            return re;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this DateTime value)
        {
            return value.Ticks;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this TimeSpan value)
        {
            return value.Ticks;
        }

        /// <summary>
        /// 获取实例的64位哈希值
        /// </summary>
        /// <param name="guid">要获取的对象</param>
        /// <returns>实例的64位哈希值</returns>
        public static long GetHashCode64(this Guid guid)
        {
            long* re = (long*)&guid;
            return re[0] ^ re[1];
        }
        #endregion

        #region 字符串

        /// <summary>
        /// 获取字符串的默认64位哈希值
        /// </summary>
        /// <param name="str">要获取的字符串</param>
        /// <returns>字符串的64位哈希值</returns>
        public static long GetHashCode64(this string str)
        {
            if (str is null) return 0;
            int length = str.Length;

            fixed (char* p = str)
            {
                return GetHashCode64ByPointer(p, length);
            }
        }

        /// <summary>
        /// （不安全代码）获取字符串的默认64位哈希值
        /// </summary>
        /// <param name="charPointer">指向字符串的起始地址</param>
        /// <param name="count">字符串的字符数量</param>
        /// <returns>字符串的64位哈希值</returns>
        public static long GetHashCode64ByPointer(char* charPointer, int count)
        {
            if (count == 0) return 1;

            const ulong FNV_OFFSET_BASIS = 14695981039346656037UL;
            const ulong FNV_PRIME = 1099511628211UL;

            ulong hash = FNV_OFFSET_BASIS;
            for (int i = 0; i < count; i++)
            {
                hash = (hash ^ charPointer[i]) * FNV_PRIME;
            }
            return (long)hash;
        }

        #endregion

        #endregion

    }


    

}
