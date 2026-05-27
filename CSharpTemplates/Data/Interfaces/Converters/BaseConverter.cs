using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cheng.DataStructure
{

    /// <summary>
    /// 对象转换的基类
    /// </summary>
    /// <typeparam name="TIn">要输入的对象类型</typeparam>
    /// <typeparam name="TOut">转换后的对象类型</typeparam>
    public abstract class BaseConverter<TIn, TOut> : IConverter<TIn, TOut>, IConverterByCondition<TIn, TOut>
    {

        public abstract TOut Convert(TIn obj);

        public abstract bool Convert(TIn obj, out TOut result);

        #region 单例

        /// <summary>
        /// 获取一个默认实现的对象转换器
        /// </summary>
        /// <remarks>
        /// <para>当<typeparamref name="TIn"/>类型对象派生于<see cref="IConverToOther{TOut}"/>接口时，默认将此接口作为转换器实现</para>
        /// <para>当<typeparamref name="TOut"/>类型属于<typeparamref name="TIn"/>类型的父类时，将以C#多态机制作为转换器实现</para>
        /// <para>当<typeparamref name="TIn"/>属于枚举类型，且<typeparamref name="TOut"/>是基元整数类型时，作为枚举值整数直接转换</para>
        /// <para>当<typeparamref name="TIn"/>或<typeparamref name="TOut"/>与另一类型互为可空值类型时，将以此作为转换器实现</para>
        /// </remarks>
        public static BaseConverter<TIn, TOut> Default
        {
            get => LazyObj.p_conver;
        }

        #region 封装

        private sealed class LazyObj
        {
            public readonly static BaseConverter<TIn, TOut> p_conver = f_createConver();
        }

        static BaseConverter<TIn, TOut> f_createConver()
        {
            var type = typeof(TIn);
            try
            {
                var outype = typeof(TOut);
                BaseConverter<TIn, TOut> res;

                if(type == outype)
                {
                    res = (new c_Convert_ValAssmEq<TIn>()) as BaseConverter<TIn, TOut>;
                    if (res != null) return res;
                }

                if (typeof(IConverToOther<TOut>).IsAssignableFrom(type))
                {
                    //存在派生
                    res = Activator.CreateInstance(typeof(c_Convert_InterFace<,>).MakeGenericType(type, outype)) as BaseConverter<TIn, TOut>;
                    if (res != null) return res;
                }

                if (outype == typeof(object))
                {
                    res = (new c_Convert_ValEqObj<TIn>()) as BaseConverter<TIn, TOut>;
                    if (res != null) return res;
                }

                res = f_createEnumVal(type, outype);
                if (res != null) return res;

                res = f_createNullable(type, outype);
                if (res != null) return res;

                if (outype.IsAssignableFrom(type))
                {
                    res = Activator.CreateInstance(typeof(c_Convert_ValAssm<,>).MakeGenericType(type, outype)) as BaseConverter<TIn, TOut>;
                    if (res != null) return res;
                }

                if(type == typeof(object))
                {
                    res = (new c_Convert_ObjTo<TOut>()) as BaseConverter<TIn, TOut>;
                    if (res != null) return res;
                }

                return new c_DefConver();
            }
            catch (Exception)
            {
                return new c_DefConver();
            }

        }

        private sealed class c_DefConver : BaseConverter<TIn, TOut>
        {
            public override TOut Convert(TIn obj)
            {
                return default;
            }

            public override bool Convert(TIn obj, out TOut result)
            {
                result = default;
                return false;
            }
        }

        static BaseConverter<TIn, TOut> f_createEnumVal(Type type, Type outype)
        {
            if (type.IsEnum)
            {
                if (outype.IsEnum || outype == typeof(int) || outype == typeof(long)
                    || outype == typeof(uint) || outype == typeof(ulong)
                    || outype == typeof(short) || outype == typeof(ushort)
                    || outype == typeof(byte) || outype == typeof(sbyte))
                {
                    return Activator.CreateInstance(typeof(c_Convert_Enum<,>).MakeGenericType(type, outype)) as BaseConverter<TIn, TOut>;
                }
            }
            else if (outype.IsEnum)
            {

                if (type == typeof(int) || type == typeof(long)
                    || type == typeof(uint) || type == typeof(ulong)
                    || type == typeof(short) || type == typeof(ushort)
                    || type == typeof(byte) || type == typeof(sbyte))
                {
                    return Activator.CreateInstance(typeof(c_Convert_Enum<,>).MakeGenericType(type, outype)) as BaseConverter<TIn, TOut>;
                }
            }

            return null;
        }

        static BaseConverter<TIn, TOut> f_createNullable(Type type, Type outype)
        {
            var nt = Nullable.GetUnderlyingType(type);
            if(nt == outype)
            {
                return Activator.CreateInstance(typeof(c_Convert_NullableTo<>).MakeGenericType(nt)) as BaseConverter<TIn, TOut>;
            }
            nt = Nullable.GetUnderlyingType(outype);
            if(nt == type)
            {
                return Activator.CreateInstance(typeof(c_Convert_ToNullable<>).MakeGenericType(nt)) as BaseConverter<TIn, TOut>;
            }
            return null;
        }

        #endregion

        #endregion

    }

    internal sealed class c_Convert_InterFace<TIn, TOut> : BaseConverter<TIn, TOut> where TIn : IConverToOther<TOut>
    {
        public c_Convert_InterFace()
        {
            p_isValue = typeof(TIn).IsValueType;
        }

        private readonly bool p_isValue;

        public override TOut Convert(TIn obj)
        {
            if(p_isValue) return obj.ConvertTo();
            else
            {
                if (obj == null) return default;
                return obj.ConvertTo();
            }
        }

        public override bool Convert(TIn obj, out TOut result)
        {
            if (p_isValue)
            {
                result = obj.ConvertTo();
            }
            else
            {
                if(obj == null)
                {
                    result = default;
                    return false;
                }
                result = obj.ConvertTo();
            }
            return true;
        }
    }

    internal sealed class c_Convert_ValEqObj<T> : BaseConverter<T, object>
    {
        public override object Convert(T obj)
        {
            return obj;
        }

        public override bool Convert(T obj, out object result)
        {
            result = obj; return true;
        }
    }

    internal sealed class c_Convert_ValAssm<TIn, TOut> : BaseConverter<TIn, TOut> where TIn : TOut
    {
        public override TOut Convert(TIn obj)
        {
            return obj;
        }

        public override bool Convert(TIn obj, out TOut result)
        {
            result = obj; return true;
        }
    }

    internal sealed class c_Convert_ValAssmEq<T> : BaseConverter<T, T>
    {
        public override T Convert(T obj)
        {
            return obj;
        }

        public override bool Convert(T obj, out T result)
        {
            result = obj; return true;
        }
    }

    internal sealed unsafe class c_Convert_Enum<T, TR> : BaseConverter<T, TR> where T : unmanaged where TR : unmanaged
    {

        public override TR Convert(T obj)
        {
            var tsize = sizeof(T);
            var trsize = sizeof(TR);

            //void* ptr = &obj;

            ulong tv;

            if (tsize > 2)
            {
                if (tsize == 4)
                {
                    // 4 byte
                    tv = *(uint*)&obj;
                }
                else
                {
                    // 8 byte
                    tv = *(ulong*)&obj;
                }
            }
            else
            {
                if (tsize == 2)
                {
                    // 2 byte
                    tv = *(ushort*)&obj;
                }
                else
                {
                    // 1 byte
                    tv = *(byte*)&obj;
                }
            }

            if (trsize > 2)
            {
                if(trsize == 4)
                {
                    // 4 byte
                    uint rei = (uint)tv;
                    return *(TR*)&rei;
                }
                else
                {
                    // 8 byte
                    return *(TR*)&tv;
                }
            }
            else
            {
                if(trsize == 2)
                {
                    // 2 byte
                    var rei = (ushort)tv;
                    return *(TR*)&rei;
                }
                else
                {
                    // 1 byte
                    var rei = (byte)tv;
                    return *(TR*)&rei;
                }
            }

        }

        public override bool Convert(T obj, out TR result)
        {
            result = Convert(obj);
            return true;
        }
    }

    internal sealed unsafe class c_Convert_NullableTo<T> : BaseConverter<T?, T> where T : struct
    {
        public override T Convert(T? obj)
        {
            return obj.GetValueOrDefault();
        }

        public override bool Convert(T? obj, out T result)
        {
            result = obj.GetValueOrDefault();
            return obj.HasValue;
        }
    }

    internal sealed unsafe class c_Convert_ToNullable<T> : BaseConverter<T, T?> where T : struct
    {
        public override T? Convert(T obj)
        {
            return obj;
        }

        public override bool Convert(T obj, out T? result)
        {
            result = obj; return true;
        }
    }

    internal sealed unsafe class c_Convert_ObjTo<T> : BaseConverter<object, T>
    {
        public override T Convert(object obj)
        {
            if (obj is T t) return t;
            return default;
        }

        public override bool Convert(object obj, out T result)
        {
            bool flag = obj is T;
            if (flag) result = (T)obj;
            else result = default;
            return flag;
        }
    }

}